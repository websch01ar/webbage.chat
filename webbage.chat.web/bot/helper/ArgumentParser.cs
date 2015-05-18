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
        internal static void Help(ref Command cmd) {
            if (!checkParameters(cmd.Args, 0, 1)) {
                cmd.Response = BotMessenger.INVALID_ARGUMENT_NUM;
                return;
            }

            string commandToGet = "!help";
            if (cmd.Args.Length == 1) {
                commandToGet = cmd.Args[0];
            }

            cmd.Dynamic = new {
                Cmd = commandToGet
            };
        }

        /// Layout should be !insult {recipient}
        internal static void Insult(ref Command cmd) {
            if (!checkParameters(cmd.Args, 1, 1)) {
                cmd.Response = BotMessenger.INVALID_ARGUMENT_NUM;
                return;
            }
            
            cmd.Dynamic = new {
                Recipient = cmd.Args[0],
                Insult = Insulter.GetInsult()
            };
        }

        /// Layout should be !google/!lmgtfy/!duck {some query string}
        internal static void SearchEngine(ref Command cmd) {
            if (!checkParameters(cmd.Args, 1, 1)) {
                cmd.Response = BotMessenger.INVALID_ARGUMENT_NUM;
                return;
            }

            cmd.Dynamic = new {
                QueryString = HttpUtility.UrlEncode(cmd.Args[0])
            };
        }

        // TODO: implement
        /// Layout should be !moab
        internal static void MOAB(ref Command cmd) {
            cmd.Response = BotMessenger.NOT_YET_IMPLEMENTED;
        }

        /// Layout should be !guid
        internal static void GUID(ref Command cmd) {
            if (!checkParameters(cmd.Args, 0, 0)) {
                cmd.Response = BotMessenger.INVALID_ARGUMENT_NUM;
                return;
            }

            cmd.Dynamic = new {
                GUID = Guid.NewGuid().ToString()
            };
        }

        // TODO: implement
        /// Layout should be !youtube {videoId} [-force]
        internal static void YouTube(ref Command cmd) {
            cmd.Response = BotMessenger.NOT_YET_IMPLEMENTED;
        }

        /// Layout should be !roll [someNumber] [maxNumber]
        internal static void Roll(ref Command cmd) {
            if (!checkParameters(cmd.Args, 0, 2)) {
                cmd.Response = BotMessenger.INVALID_ARGUMENT_NUM;
                return;
            }

            int number = 1;
            int maxNumber = 100;

            if (cmd.Args.Length > 0) {
                if (!int.TryParse(cmd.Args[0], out maxNumber)) {
                    cmd.Response = BotMessenger.GetMessage("Input must be a valid number. The ");
                    return;
                }
            }

            if (cmd.Args.Length == 2) {
                number = maxNumber;
                if (!int.TryParse(cmd.Args[1], out maxNumber)) {
                    cmd.Response = BotMessenger.GetMessage("Input must be a valid number");
                    return;
                }
            }
            
            if (number > int.MaxValue || maxNumber > int.MaxValue || number <= 0) { // both have to be less than the int.MaxValue
                cmd.Response = BotMessenger.GetMessage(string.Format("The supplied number(s) must be between 1 and {0}.", (int.MaxValue - 1).ToString()));
                return;
            } else if (number >= maxNumber) {
                cmd.Response = BotMessenger.GetMessage("The first supplied number must be smaller than the second supplied number.");
                return;
            }

            cmd.Dynamic = new {
                Number = number,
                MaxNumber = maxNumber
            };
        }

        // TODO: implement
        /// Layout should be !kick {userName} [-ban]
        internal static void Kick(ref Command cmd) {
            cmd.Response = BotMessenger.NOT_YET_IMPLEMENTED;
        }
    }
}
