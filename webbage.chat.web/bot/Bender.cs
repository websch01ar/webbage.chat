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
        private static Dictionary<string, CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, dynamic>, Func<Command, ChatHub, bool>>> commands;
        private static ChatHub hub;

        static Bender() {
            commands = new Dictionary<string, CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, dynamic>, Func<Command, ChatHub, bool>>> {
                {
                    "!help",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, dynamic>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.HELP,
                        Parser = new Func<string, string[]>(CommandParser.ParseNormal),
                        ArgParser = new Func<Command, ChatHub, dynamic>(ArgumentParser.Help),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutors.Help)
                    }
                },

                {
                    "!insult",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, dynamic>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.INSULT,
                        Parser = new Func<string, string[]>(CommandParser.ParseNormal),
                        ArgParser = new Func<Command, ChatHub, dynamic>(ArgumentParser.Insult),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutors.Insult)
                    }
                },

                {
                    "!lmgtfy",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, dynamic>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.LMGTFY,
                        Parser = new Func<string, string[]>(CommandParser.ParseDoubleQuotes),
                        ArgParser = new Func<Command, ChatHub, dynamic>(ArgumentParser.LMGTFY),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutors.LMGTFY)
                    }
                },

                {
                    "!google",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, dynamic>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.GOOGLE,
                        Parser = new Func<string, string[]>(CommandParser.ParseDoubleQuotes),
                        ArgParser = new Func<Command, ChatHub, dynamic>(ArgumentParser.Google),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutors.LMGTFY)
                    }
                },

                {
                    "!duck",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, dynamic>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.DUCK,
                        Parser = new Func<string, string[]>(CommandParser.ParseDoubleQuotes),
                        ArgParser = new Func<Command, ChatHub, dynamic>(ArgumentParser.Duck),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutors.Duck)
                    }
                },

                {
                    "!moab",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, dynamic>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.MOAB,
                        Parser = new Func<string, string[]>(CommandParser.ParseNormal),
                        ArgParser = new Func<Command, ChatHub, dynamic>(ArgumentParser.MOAB),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutors.MOAB)
                    }
                },

                {
                    "!kick",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, dynamic>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.KICK,
                        Parser = new Func<string, string[]>(CommandParser.ParseNormal),
                        ArgParser = new Func<Command, ChatHub, dynamic>(ArgumentParser.Kick),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutors.Kick)
                    }
                },

                {
                    "!guid",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, dynamic>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.GUID,
                        Parser = new Func<string, string[]>(CommandParser.ParseNormal),
                        ArgParser = new Func<Command, ChatHub, dynamic>(ArgumentParser.GUID),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutors.GUID)
                    }
                },

                {
                    "!youtube",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, dynamic>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.YOUTUBE,
                        Parser = new Func<string, string[]>(CommandParser.ParseNormal),
                        ArgParser = new Func<Command, ChatHub, dynamic>(ArgumentParser.YouTube),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutors.YouTube)
                    }
                },

                {
                    "!roll",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, dynamic>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.ROLL,
                        Parser = new Func<string, string[]>(CommandParser.ParseNormal),
                        ArgParser = new Func<Command, ChatHub, dynamic>(ArgumentParser.Roll),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutors.Roll)
                    }
                }
            };            
        }

        public static async Task DoWork(ChatHub chatHub, Message message) {
            Command cmd = new Command(message);
            hub = chatHub;

            if (!(validateCommand(cmd))) {
                await hub.Clients.Caller.receiveMessage(BotMessenger.INVALID_COMMAND);
                return;
            }

            if (!(parseCommand(cmd))) {
                await hub.Clients.Caller.receiveMessage(BotMessenger.INVALID_PARSE_COMMAND);
                return;
            }

            if (!(parseArguments(cmd))) {
                // don't worry about error message here, the Func<> pointer for each command should handle that
                return;
            }

            if (!(execCommand(cmd))) {
                await hub.Clients.Caller.receiveMessage(BotMessenger.INVALID_EXEC_COMMAND);
                return;
            }
        }

        private static bool validateCommand(Command cmd) {
            return commands.Keys.Contains<string>(cmd.Name);
        }

        private static bool parseCommand(Command cmd) {
            try {
                cmd.Args = (string[])commands[cmd.Name].Parser.DynamicInvoke(new object[] { (object)cmd.Text });
                return true;
            } catch {
                return false;
            }
        }

        private static bool parseArguments(Command cmd) {
            try {
                cmd.Dynamic = (dynamic)commands[cmd.Name].ArgParser.DynamicInvoke(new object[] { (object)cmd, (object)hub });
                return cmd.Dynamic != null;
            } catch {
                return false;
            }
        }

        private static bool execCommand(Command cmd) {
            return (bool)commands[cmd.Name].Action.DynamicInvoke(new object[] { (object)cmd, (object)hub });
        }
    }
}
