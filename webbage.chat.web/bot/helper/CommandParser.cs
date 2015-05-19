using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webbage.chat.model;

namespace webbage.chat.web.bot.helper {
    class CommandParser {
        /// <summary>
        /// Use ParseNone if you don't expect any arguments to be included with the command. An example would be !guid or !moab
        /// </summary>
        public static void ParseNone(ref Command cmd) {
            cmd.Args = new string[0];
        }
        /// <summary>
        /// Use ParseNormal if you want the entire cmd.Text (everything after the !{commandName} part of the message) to be interpreted as a parameter
        /// </summary>        
        public static void ParseNormal(ref Command cmd) {
            cmd.Args = new string[1] { cmd.Text };
        }
        /// <summary>
        /// Use ParseSpaces if you want to separate cmd.Text (everything after the !{commandName} part of the message) to be split into parameters based on spaces
        /// </summary>
        public static void ParseSpaces(ref Command cmd) {
            cmd.Args = cmd.Text.Split(' ').Where(a => a.ToString() != string.Empty).ToArray();
        }
        /// <summary>
        /// Use ParseDoubleQuotes if you want to separate cmd.Text (everything after the !{commandName} part of the message) to be split into parameters based on double-quotes
        /// </summary>
        public static void ParseDoubleQuotes(ref Command cmd) {
            cmd.Args = cmd.Text.Split('"').Select(p => p.Trim()).Where(p => p != "").ToArray(); // get the middle argument, things going through here should only have one argument
        }
    }
}
