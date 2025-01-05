using dmake.Util;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace dmake.CLI {
    class Program {
        public static void Main(string[] args) {
            string verCli = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine("dmake-core {0}    dmake-cli {1}   Copyright 2022 By Tsccai", AssemblyGetter.AssemblyVersion, verCli);
            Console.WriteLine("Life is short, you need dmake.");


            CLIArgs command = CLIArgsReader.ReadArgs(args);
            switch (command.Instruction) {
                case CLIInsutruction.Install:
                    InstallMyself();
                    break;
                case CLIInsutruction.Uninstall:
                    UninstallMyself();
                    break;
                case CLIInsutruction.Make:
                    try {
                        Make();
                    }
                    catch (Exception ex) {
                        ConsoleWarning.Warn(ex.Message);
                    }
                    break;
                case CLIInsutruction.NewProject:
                    InitProject(command);
                    break;
                case CLIInsutruction.AddTemplate:
                    AddTemplate(command);
                    break;
                case CLIInsutruction.RemoveTemplate:
                    RemoveTemplate(command);
                    break;
                case CLIInsutruction.ListTemplate:
                    ListTemplate(command);
                    break;
                case CLIInsutruction.Unknown:
                case CLIInsutruction.Help:
                default:
                    if (command.Instruction == CLIInsutruction.Unknown) {
                        Console.WriteLine("无效的参数");
                    }
                    ShowHelpInfo();
                    break;

            }



        }


        static void Make() {
            if (!DirectoryHelper.IsDmakeDirectory()) {
                Console.WriteLine("找不到makefile，请确认当前目录是一个dmake项目目录");
                ShowHelpInfo();
                return;
            }
            Console.WriteLine("读取makefile...");
            YamlConfigure config = YamlConfigureManager.ReadMakefile();
            JObject jo = new JObject();
            //MarkdownAdapter mda = new MarkdownAdapter();
            //mda.LoadMarkdownDocument("md/plan.md");
            MarkdownReader mdr = null;
            foreach (var i in config.Source) {
                mdr = new MarkdownReader(@"md\" + i);
                jo = mdr.MergeJson(jo, mdr.ReadToEnd().FieldValuePairs);
            }

            MarkdownReader.ReplaceInternalAnchor(jo);

            DocumentGenerator gen = new DocumentGenerator();
            // TODO: clean the tmp files
            // DMakeCleaner.Clean();

            int success = 0, fail = 0;
            foreach (var i in config.Files) {
                try {
                    FileInfo fi = new FileInfo(i.Key);
                    if (!Directory.Exists(fi.DirectoryName)) {
                        Directory.CreateDirectory(fi.DirectoryName);
                    }

                    Console.WriteLine("文档生成中，当前文档：{0}", i.Key);
                    var warns = gen.CreateDocument(@"template\" + i.Value, i.Key, jo);
                    foreach (var warn in warns) {
                        ConsoleWarning.WarnLine(warn);
                    }
                    success++;
                }
                catch (Exception ex) {
                    fail++;
                    ConsoleWarning.WarnLine(ex.Message);
                    continue;
                }

            }
            Console.WriteLine("文档生成完毕，共生成 {0} 个文档，失败 {1} 个文档", success, fail);
        }

        static void InitProject(CLIArgs command) {
            if (!DirectoryHelper.IsEmptyDirectory(Environment.CurrentDirectory)) {
                Console.Write("当前目录不是空目录，是否要在此初始化dmake项目: (y/n，默认为n)");
                string input = Console.ReadLine().ToLower();
                if (input != "y") {
                    return;
                }
            }
            string source = AppDomain.CurrentDomain.BaseDirectory + "NewProjectTemplate/" + command.Parameter["Type"];
            var dest = Environment.CurrentDirectory;
            if (!Directory.Exists(source)) {
                ConsoleWarning.Warn($"模板目录“{command.Parameter["Type"]}”不存在。");
                return;
            }
            if (!Directory.Exists(dest)) {
                ConsoleWarning.Warn($"目的工程目录不存在。");
                return;
            }
            DirectoryHelper.CopyDirectory(source, dest);
            Console.WriteLine("{0} 类型项目已创建", command.Parameter["Type"]);
        }

        static void AddTemplate(CLIArgs command) {
            string source = AppDomain.CurrentDomain.BaseDirectory + "NewProjectTemplate/individual/" + command.Parameter["Template"];
            string dest = Environment.CurrentDirectory;
            if (!DirectoryHelper.IsDmakeDirectory(dest)) {
                Console.WriteLine("当前目录不是dmake项目目录，是否要添加dmake模板: (y/n，默认为n)");
                string input = Console.ReadLine().ToLower();
                if (input != "y") {
                    return;
                }
            }
            DirectoryHelper.CopyDirectory(source, dest);
            Console.WriteLine("模板 {0} 已添加，请自行在makefile中添加相应的代码", command.Parameter["Template"]);
        }

        static void RemoveTemplate(CLIArgs command) {
            Console.WriteLine("该功能尚未实现");
        }

        static void ListTemplate(CLIArgs command) {
            Console.WriteLine("已安装的模板：");

            string path = AppDomain.CurrentDomain.BaseDirectory + "NewProjectTemplate/individual/";
            var dirs = DirectoryHelper.EnumerateSubDirectories(path);
            foreach (var d in dirs) {
                Console.WriteLine(d.Name);
            }
            if (dirs.Count() == 0) {
                Console.WriteLine("无");
            }
            Console.WriteLine("\n已安装的项目模板：");

            path = AppDomain.CurrentDomain.BaseDirectory + "NewProjectTemplate";

            dirs = DirectoryHelper.EnumerateSubDirectories(path);
            foreach (var d in dirs) {
                if (d.Name != "individual") {
                    Console.WriteLine(d.Name);
                }
            }
            if (dirs.Count() == 0) {
                Console.WriteLine("无");
            }

        }

        static void ShowHelpInfo() {
            Console.WriteLine("dmake命令行用法：dmake [command] [argment]");
            Console.WriteLine("dmake 或 dmake make: 编译文档");
            Console.WriteLine("dmake init [c3t1|c4t1|c3t2|c4t2|c5t2]: 在当前目录初始化一个dmake项目，参数列表如下：");
            Console.WriteLine("    c3t1: III级风险，第一种工作票");
            Console.WriteLine("    c4t1: IV级风险，第一种工作票");
            Console.WriteLine("    c3t2: III级风险，第二种工作票");
            Console.WriteLine("    c4t2: IV级风险，第二种工作票");
            Console.WriteLine("    c5t2: V级风险，第二种工作票");
            Console.WriteLine("dmake add template [template_name]: 添加指定模板至当前项目目录");
            Console.WriteLine("dmake list template: 列出已安装的模板");
            Console.WriteLine("dmake install: 安装dmake（将dmake.exe所在目录添加到PATH环境变量）");
            Console.WriteLine("dmake uninstall: 卸载dmake（仅将dmake.exe所在目录从PATH环境变量中移除）");
            Console.WriteLine("dmake help: 显示帮助信息");
        }

        static void InstallMyself() {
            string dmakePath = AppDomain.CurrentDomain.BaseDirectory + ";";
            string ENV_PATH = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
            ENV_PATH = ENV_PATH.EndsWith(";") ? ENV_PATH : ENV_PATH + ";";
            if (!ENV_PATH.Contains(dmakePath)) {
                Environment.SetEnvironmentVariable("PATH", ENV_PATH + dmakePath, EnvironmentVariableTarget.User);
            }
            Console.WriteLine("dmake 已安装，请在重启控制台后执行dmake命令。");
        }

        static void UninstallMyself() {
            string dmakePath = AppDomain.CurrentDomain.BaseDirectory + ";";
            string ENV_PATH = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
            if (ENV_PATH.Contains(dmakePath)) {
                string newPath = ENV_PATH.Replace(dmakePath, "");
                Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.User);
            }
            Console.WriteLine("dmake 已卸载，请删除安装目录下的文件");

        }

    }
}
