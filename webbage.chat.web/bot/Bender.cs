using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using webbage.chat.model;
using webbage.chat.web.bot.helper;
using webbage.chat.web.hub;

namespace webbage.chat.web.bot {
    public static class Bender {
        private delegate void RefCommandAction(ref Command cmd);

        private static Dictionary<string, CommandStruct<string, RefCommandAction, RefCommandAction, RefCommandAction>> commands;
        private static ChatHub hub;

        static Bender() {
            commands = new Dictionary<string, CommandStruct<string, RefCommandAction, RefCommandAction, RefCommandAction>> {
                {
                    "!help",
                    new CommandStruct<string, RefCommandAction, RefCommandAction, RefCommandAction> {
                        Desc = CommandDescriptions.HELP,
                        Parser = new RefCommandAction(CommandParser.ParseNormal),
                        ArgParser = new RefCommandAction(ArgumentParser.Help),
                        Action = new RefCommandAction(CommandExecutors.Help)
                    }
                },

                {
                    "!insult",
                    new CommandStruct<string, RefCommandAction, RefCommandAction, RefCommandAction> {
                        Desc = CommandDescriptions.INSULT,
                        Parser = new RefCommandAction(CommandParser.ParseNormal),
                        ArgParser = new RefCommandAction(ArgumentParser.Insult),
                        Action = new RefCommandAction(CommandExecutors.Insult)
                    }
                },

                {
                    "!lmgtfy",
                    new CommandStruct<string, RefCommandAction, RefCommandAction, RefCommandAction> {
                        Desc = CommandDescriptions.LMGTFY,
                        Parser = new RefCommandAction(CommandParser.ParseSearchEngine),
                        ArgParser = new RefCommandAction(ArgumentParser.LMGTFY),
                        Action = new RefCommandAction(CommandExecutors.LMGTFY)
                    }
                },

                {
                    "!google",
                    new CommandStruct<string, RefCommandAction, RefCommandAction, RefCommandAction> {
                        Desc = CommandDescriptions.GOOGLE,
                        Parser = new RefCommandAction(CommandParser.ParseSearchEngine),
                        ArgParser = new RefCommandAction(ArgumentParser.Google),
                        Action = new RefCommandAction(CommandExecutors.Google)
                    }
                },

                {
                    "!duck",
                    new CommandStruct<string, RefCommandAction, RefCommandAction, RefCommandAction> {
                        Desc = CommandDescriptions.DUCK,
                        Parser = new RefCommandAction(CommandParser.ParseSearchEngine),
                        ArgParser = new RefCommandAction(ArgumentParser.Duck),
                        Action = new RefCommandAction(CommandExecutors.Duck)
                    }
                },

                {
                    "!moab",
                    new CommandStruct<string, RefCommandAction, RefCommandAction, RefCommandAction> {
                        Desc = CommandDescriptions.MOAB,
                        Parser = new RefCommandAction(CommandParser.ParseNone),
                        ArgParser = new RefCommandAction(ArgumentParser.MOAB),
                        Action = new RefCommandAction(CommandExecutors.MOAB)
                    }
                },

                {
                    "!kick",
                    new CommandStruct<string, RefCommandAction, RefCommandAction, RefCommandAction> {
                        Desc = CommandDescriptions.KICK,
                        Parser = new RefCommandAction(CommandParser.ParseNormal),
                        ArgParser = new RefCommandAction(ArgumentParser.Kick),
                        Action = new RefCommandAction(CommandExecutors.Kick)
                    }
                },

                {
                    "!guid",
                    new CommandStruct<string, RefCommandAction, RefCommandAction, RefCommandAction> {
                        Desc = CommandDescriptions.GUID,
                        Parser = new RefCommandAction(CommandParser.ParseNone),
                        ArgParser = new RefCommandAction(ArgumentParser.GUID),
                        Action = new RefCommandAction(CommandExecutors.GUID)
                    }
                },

                {
                    "!youtube",
                    new CommandStruct<string, RefCommandAction, RefCommandAction, RefCommandAction> {
                        Desc = CommandDescriptions.YOUTUBE,
                        Parser = new RefCommandAction(CommandParser.ParseNormal),
                        ArgParser = new RefCommandAction(ArgumentParser.YouTube),
                        Action = new RefCommandAction(CommandExecutors.YouTube)
                    }
                },

                {
                    "!roll",
                    new CommandStruct<string, RefCommandAction, RefCommandAction, RefCommandAction> {
                        Desc = CommandDescriptions.ROLL,
                        Parser = new RefCommandAction(CommandParser.ParseNormal),
                        ArgParser = new RefCommandAction(ArgumentParser.Roll),
                        Action = new RefCommandAction(CommandExecutors.Roll)
                    }
                }
            };            
        }

        // TODO: add some sort of logging in these functions
        public static Command DoWork(ChatHub chatHub, Message message) {
            Command cmd = new Command(message);
            hub = chatHub;

            if (!(validateCommand(cmd))) {
                cmd.Response = BotMessenger.INVALID_COMMAND;                
            }

            if (cmd.Response == null && !(parseCommand(cmd))) {
                cmd.Response = BotMessenger.INVALID_PARSE_COMMAND;
            }

            if (cmd.Response == null && !(parseArguments(cmd))) {
                // don't worry about setting response here, the RefCommandAction does it
            }

            if (cmd.Response == null && !(execCommand(cmd))) {
                cmd.Response = BotMessenger.INVALID_EXEC_COMMAND;
            }

            return cmd;
        }

        public static string GetCommandDescription(string cmdName) {
            string desc = "";

            if (!cmdName.StartsWith("!")) {
                cmdName = "!" + cmdName;
            }

            if (validateCommand(cmdName)) {
                desc = commands[cmdName].Desc;
            } else {
                desc = "Invalid command name";
            }

            return desc;
        }

        private static bool validateCommand(string cmdName) {
            return commands.Keys.Contains(cmdName);
        }
        private static bool validateCommand(Command cmd) {
            return commands.Keys.Contains(cmd.Name);
        }

        private static bool parseCommand(Command cmd) {
            try {
                commands[cmd.Name].Parser.DynamicInvoke(new object[] { cmd });
                return cmd.Args != null;
            } catch {
                return false;
            }
        }

        private static bool parseArguments(Command cmd) {
            try {
                commands[cmd.Name].ArgParser.DynamicInvoke(new object[] { cmd });
                return cmd.Dynamic != null;
            } catch {
                return false;
            }
        }

        private static bool execCommand(Command cmd) {
            try {
                commands[cmd.Name].Action.DynamicInvoke(new object[] { cmd });
                return cmd.Response != null;
            } catch {
                return false;
            }
        }
    }
}
