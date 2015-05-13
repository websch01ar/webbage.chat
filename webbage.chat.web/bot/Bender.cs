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
        private static Dictionary<string, CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>>> commands;
        private static ChatHub hub;

        static Bender() {
            commands = new Dictionary<string, CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>>> {
                {
                    "!help",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.HELP,
                        Parser = new Func<string, string[]>(Parser.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.Help)
                    }
                },

                {
                    "!insult",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.INSULT,
                        Parser = new Func<string, string[]>(Parser.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.Insult)
                    }
                },

                {
                    "!lmgtfy",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.LMGTFY,
                        Parser = new Func<string, string[]>(Parser.ParseDoubleQuotes),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.LMGTFY)
                    }
                },

                {
                    "!google",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.GOOGLE,
                        Parser = new Func<string, string[]>(Parser.ParseDoubleQuotes),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.LMGTFY)
                    }
                },

                {
                    "!duck",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.DUCK,
                        Parser = new Func<string, string[]>(Parser.ParseDoubleQuotes),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.Duck)
                    }
                },

                {
                    "!moab",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.MOAB,
                        Parser = new Func<string, string[]>(Parser.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.MOAB)
                    }
                },

                {
                    "!kick",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.KICK,
                        Parser = new Func<string, string[]>(Parser.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.Kick)
                    }
                },

                {
                    "!guid",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.GUID,
                        Parser = new Func<string, string[]>(Parser.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.GUID)
                    }
                },

                {
                    "!youtube",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.YOUTUBE,
                        Parser = new Func<string, string[]>(Parser.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.YouTube)
                    }
                },

                {
                    "!roll",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.ROLL,
                        Parser = new Func<string, string[]>(Parser.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.Roll)
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

        private static bool execCommand(Command cmd) {
            return (bool)commands[cmd.Name].Action.DynamicInvoke(new object[] { (object)cmd, (object)hub });
        }
    }
}
