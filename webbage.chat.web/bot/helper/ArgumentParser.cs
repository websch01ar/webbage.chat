using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webbage.chat.model;
using webbage.chat.web.hub;

namespace webbage.chat.web.bot.helper {
    /// <summary>
    /// Lays out commands and how their arguments should be parsed. In the comments above each method there should be an example
    /// of how the command should look. Parameters in {} are required, parameters in [] are optional.
    /// </summary>
    public class ArgumentParser {
        private static bool checkParameters(string[] args, int minNoOfArgs, int maxNoOfArgs) {
            return minNoOfArgs == maxNoOfArgs ? args.Length == minNoOfArgs : args.Length >= minNoOfArgs && args.Length <= maxNoOfArgs;
        }

        /// Layout should be !help [commandName]
        internal static dynamic Help(Command cmd, ChatHub hub) {
            return null;
        }

        /// Layout should be !insult {recipient}
        internal static dynamic Insult(Command cmd, ChatHub hub) {
            if (!checkParameters(cmd.Args, 1, 1))
                return null;
            
            return new {
                Recipient = cmd.Args[0],
                Insult = Insulter.GetInsult()
            };
        }

        /// Layout should be !lmgtfy {"some query string"}
        internal static dynamic LMGTFY(Command cmd, ChatHub hub) {
            return null;
        }

        /// Layout should be !google {"some query string"}
        internal static dynamic Google(Command cmd, ChatHub hub) {
            return null;
        }

        /// Layout should be !moab
        internal static dynamic MOAB(Command cmd, ChatHub hub) {
            return null;
        }

        /// Layout should be !guid
        internal static dynamic GUID(Command cmd, ChatHub hub) {
            if (!checkParameters(cmd.Args, 0, 0))
                return null;

            return new {
                Guid = Guid.NewGuid().ToString()
            };
        }

        /// Layout should be !youtube {videoId} [-force]
        internal static dynamic YouTube(Command cmd, ChatHub hub) {
            return null;
        }

        /// Layout should be !roll {someNumber} [maxNumber]
        internal static dynamic Roll(Command cmd, ChatHub hub) {
            if (!checkParameters(cmd.Args, 1, 2))
                return null;

            int number = 0;
            int maxNumber = int.MaxValue;

            if (!int.TryParse(cmd.Args[0], out number))
                return null;

            if (cmd.Args.Length == 2) {
                if (!int.TryParse(cmd.Args[1], out maxNumber))
                    return null;
            }

            Message msg = null;
            if (number >= int.MaxValue || maxNumber >= int.MaxValue || number <= 0) { // both have to be less than the int.MaxValue
                msg = BotMessenger.GetMessage(string.Format("The supplied number(s) must be between 1 and {0}.", (int.MaxValue - 1).ToString()));
            } else if (number >= maxNumber) {
                msg = BotMessenger.GetMessage("The first supplied number must be smaller than the second supplied number.");
            }

            if (msg != null) {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            }

            return new {
                Number = number,
                MaxNumber = maxNumber
            };
        }

        /// Layout should be !kick {userName} [-ban]
        internal static dynamic Kick(Command cmd, ChatHub hub) {
            return null;
        }

        /// Layout should be !duck {"some query string"}
        internal static dynamic Duck(Command cmd, ChatHub hub) {
            return null;
        }
    }
}