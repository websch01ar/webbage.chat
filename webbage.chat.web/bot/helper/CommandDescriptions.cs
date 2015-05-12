using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webbage.chat.web.bot.helper {
    class CommandDescriptions {
        public const string HELP = "";
        public const string INSULT = "";
        public const string LMGTFY = "";
        public const string MOAB = "";
        public const string KICK = "";
        public const string GUID = "";
        public const string GOOGLE = "Alias of lmgtfy - Usage: !google {phrase} - returns lmgtfy results for the given phrase.";
        public const string YOUTUBE = "Plays a YouTube video in the stage - Not Yet Implemented";
        public const string DUCK = "Provides a link to Duck Duck Go search of the provided string.";
        public const string ROLL = "Provides a random number between 1 and the number specified (maximum of 2147483646).  Example: !roll 100";
    }
}
