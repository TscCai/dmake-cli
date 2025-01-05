using System.Collections.Generic;

namespace dmake.CLI
{
    public enum CLIInsutruction
    {
        Make,
        NewProject,
        AddTemplate,
        RemoveTemplate,
        ListTemplate,
        Help,
        Install,
        Uninstall,
        Unknown
    }

    public struct CLIArgs
    {
        public CLIInsutruction Instruction { get; private set; }
        public Dictionary<string, string> Parameter { get; private set; }
        public CLIArgs(CLIInsutruction instruction, Dictionary<string, string> para) {
            Instruction = instruction;
            Parameter = para;
        }
    }

    public class CLIArgsReader
    {
        /* Possible Parameter:
         * null: make document
         * make: make document
         * new [c4t1|c3t1|c2t1|c5t2|c4t2|c3t2] -n ProjectName 
         * add template template_name
         * remove template template_name
         * list template
         */

        public static CLIArgs ReadArgs(string[] args) {
            CLIArgs result;
            if (args.Length == 0 || args[0] == "make") {
                return new CLIArgs(CLIInsutruction.Make, null);
            }
            if (args.Length == 1) {
                if (args[0] == "install") {
                    return new CLIArgs(CLIInsutruction.Install, null);
                }
                else if (args[0] == "uninstall") {
                    return new CLIArgs(CLIInsutruction.Uninstall, null);
                }
                else if(args[0] == "help") {
                    return new CLIArgs(CLIInsutruction.Help, null);
                }
            }
            string instruction = args[0];
            if (instruction == "init" && args.Length == 2) {
                Dictionary<string, string> para = new Dictionary<string, string>();
                para.Add("Type", args[1]);
                return new CLIArgs(CLIInsutruction.NewProject, para);
            }
            if (args.Length > 1) {
                instruction += " " + args[1];
                Dictionary<string, string> para = null;
                CLIInsutruction finalInsturction = CLIInsutruction.Unknown;
                if (instruction == "add template") {
                    finalInsturction = CLIInsutruction.AddTemplate;
                }
                //else if (instruction == "remove template") {
                //    finalInsturction = CLIInsutruction.RemoveTemplate;
                //}
                else if (instruction == "list template") {
                    finalInsturction = CLIInsutruction.ListTemplate;
                    return new CLIArgs(finalInsturction, new Dictionary<string, string>());
                }
                else {
                    return new CLIArgs(CLIInsutruction.Unknown, null);
                }
                para = new Dictionary<string, string>();
                para.Add("Template", args[2]);
                return new CLIArgs(finalInsturction, para);
            }

            result = new CLIArgs(CLIInsutruction.Unknown, null);
            return result;
        }

    }
}