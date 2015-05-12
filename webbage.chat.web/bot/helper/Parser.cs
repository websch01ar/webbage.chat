using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webbage.chat.web.bot.helper {
    public class Parser {
        public static string[] ParseNormal(string text) {
            return text.Split(' ');
        }

        public static string[] ParseDoubleQuotes(string text) {
            return text.Split('"').Select(p => p.Trim()).Where(p => p != "").ToArray<string>(); // get the middle argument, things going through here should only have one argument
        }
    }
}
