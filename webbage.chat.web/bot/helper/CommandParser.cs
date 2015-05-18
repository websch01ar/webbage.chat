using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webbage.chat.model;

namespace webbage.chat.web.bot.helper {
    class CommandParser {
        public static void ParseNone(ref Command cmd) {
            cmd.Args = new string[0];
        }
        public static void ParseNormal(ref Command cmd) {
            cmd.Args = cmd.Text.Split(' ').Where(a => a.ToString() != string.Empty).ToArray();
        }

        public static void ParseDoubleQuotes(ref Command cmd) {
            cmd.Args = cmd.Text.Split('"').Select(p => p.Trim()).Where(p => p != "").ToArray(); // get the middle argument, things going through here should only have one argument
        }

        public static void ParseSearchEngine(ref Command cmd) {
            cmd.Args = new string[1];
            cmd.Args[0] = cmd.Text;
        }
    }
}
