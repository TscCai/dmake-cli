using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dmake.CLI
{
    public class ConsoleWarning
    {
        public static ConsoleColor WarningColor { get; set; }
        static ConsoleWarning() {
            WarningColor = ConsoleColor.Red;
        }
        public static void Warn(string text) {
            var former = Console.ForegroundColor;
            Console.ForegroundColor = WarningColor;
            Console.Write(text);
            Console.ForegroundColor = former;
        }

        public static void Warn(string format, params object[] arg) {
            var former = Console.ForegroundColor;
            Console.ForegroundColor = WarningColor;
            Console.Write(format, arg);
            Console.ForegroundColor = former;
        }

        public static void WarnLine(string text) {
            var former = Console.ForegroundColor;
            Console.ForegroundColor = WarningColor;
            Console.WriteLine(text);
            Console.ForegroundColor = former;
        }

        public static void WarnLine(string format, params object[] arg) {
            var former = Console.ForegroundColor;
            Console.ForegroundColor = WarningColor;
            Console.WriteLine(format, arg);
            Console.ForegroundColor = former;
        }


    }
}
