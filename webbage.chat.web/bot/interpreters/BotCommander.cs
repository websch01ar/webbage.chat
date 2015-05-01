using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using webbage.chat.model;
using webbage.chat.web.bot.model;
using webbage.chat.web.hub;

namespace webbage.chat.web.bot.interpreters {
    public static class BotCommander {
        private static Dictionary<string, CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>>> commands;
        private static ChatHub hub;

        static BotCommander() {
            commands = new Dictionary<string, CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>>> {
                {
                    "!help",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.HELP,
                        Parser = new Func<string, string[]>(ParseHelper.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.Help)
                    }
                },
                {
                    "!insult",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.INSULT,
                        Parser = new Func<string, string[]>(ParseHelper.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.Insult)
                    }
                },
                {
                    "!lmgtfy",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.INSULT,
                        Parser = new Func<string, string[]>(ParseHelper.ParseDoubleQuotes),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.LMGTFY)
                    }
                },
                {
                    "!moab",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.MOAB,
                        Parser = new Func<string, string[]>(ParseHelper.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.MOAB)
                    }
                }
            };            
        }

        public static async Task DoWork(ChatHub chatHub, Message message) {
            Command cmd = new Command(message.Content);
            hub = chatHub;

            if (!(validateCommand(cmd))) {
                await hub.Clients.Caller.receiveMessage(new Message {
                    Sender = hub.bender,
                    Content = "Unable to validate command. Make sure you typed a valid command name",
                    Sent = DateTime.Now.ToString("MMM d, h:mm tt"),
                    IsCode = false
                });
                return;
            }

            if (!(parseCommand(cmd))) {
                await hub.Clients.Caller.receiveMessage(new Message {
                    Sender = hub.bender,
                    Content = "Unable to parse this command. Make sure you typed in valid parameters for it. If you need help, type in !help {commandName}",
                    Sent = DateTime.Now.ToString("MMM d, h:mm tt"),
                    IsCode = false
                });
                return;
            }

            if (!(execCommand(cmd))) {
                await hub.Clients.Caller.receiveMessage(new Message {
                    Sender = hub.bender,
                    Content = "Unable to execute this command. Make sure you typed in valid parameters for it. If you need help, type in !help {commandName}",
                    Sent = DateTime.Now.ToString("MMM d, h:mm tt"),
                    IsCode = false
                });
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

    class CommandDescriptions {
        public const string HELP = "";
        public const string INSULT = "";
        public const string LMGTFY = "";
        public const string MOAB = "";
    }

    class ParseHelper {
        public static string[] ParseNormal(string text) {
            return text.Split(' ');
        }

        public static string[] ParseDoubleQuotes(string text) {
            return text.Split('"').Select(p => p.Trim()).Where(p => p != "").ToArray<string>(); // get the middle argument, things going through here should only have one argument
        }
    }

    class CommandExecutor {
        static CommandExecutor() {
            random = new Random();
            insults = new List<string>() {
                "@{0}, your mom is like a race car driver; she burns a lot of rubbers",
                "Your mom is like a doorknob @{0}; everybody gets a turn",
                "@{0}, your mom is like an ice cream cone; everyone gets a lick",
                "Your mom is like a bowling ball @{0}; you can fit three fingers in",
                "@{0}, your mom is like a bowling ball; she always winds up in the gutter",
                "Your mom is like a bowling ball @{0}; she always comes back for more",
                "@{0}, your mom is like McDonalds; Billions and Billions served",
                "Your mom is like Denny's @{0}; open 24 hours",
                "@{0}, your mom is like a shotgun; give her a cock and she blows",            
                "Your mom is like a railroad track @{0}; she gets laid all over the country.",
                "@{0}, your mom is like a T.V.; a two year old could turn her on.",
                "Your mom is like a goalie @{0}; she changes her pads after three periods.",            
                "If I had change for a buck I could have been your dad @{0}!",
                "@{0}, is that an accent or is your mouth just full of cum?",
                "When you pass away and people ask me what the cause of death was, @{0}, I'll say it was your stupidity.",
                "When you get run over by a car, @{0}, it shouldn't be listed under accidents.",
                "@{0}, you  were  born  because  your  mother  didn't believe in abortion; now she believes in infanticide.",
                "@{0}, I heard your parents took you to a dog show and you won.",
                "You must have been born on a highway because that's where most accidents happen, @{0}",
                "You are proof that God has a sense of humor, @{0}",
                "@{0}, your birth certificate is an apology letter from the condom factory.",
                "I'd like to see things from your point of view, @{0}, but I can't seem to get my head that far up my ass.",
                "What are you doing here @{0}? Did someone leave your cage open?",
                "Shut up @{0}, you'll never be the man your mother is.",
                "Well, @{0}, I could agree with you, but then we'd both be wrong.",
                "So, a thought crossed your mind, @{0}? Must have been a long and lonely journey.",
                "Everyone who ever loved you was wrong, @{0}.",
                "Learn from your parents' mistakes @{0} - use birth control!",
                "@{0}, why don't you slip into something more comfortable -- like a coma."
            };
        }

        public static bool Help(Command cmd, ChatHub hub) {            
            return true;
        }

        private static Random random;
        private static List<string> insults;
        public static bool Insult(Command cmd, ChatHub hub) {
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;

            hub.Clients.Group(hub.room.RoomID).receiveMessage(new Message {
                Sender = hub.bender,
                Content = string.Format(insults[random.Next(0, insults.Count())], cmd.Args[0]),
                Sent = DateTime.Now.ToString("MMM d, h:mm tt"),
                IsCode = false
            });
            return true;
        }

        public static bool LMGTFY(Command cmd, ChatHub hub) {
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;

            hub.Clients.Group(hub.room.RoomID).receiveMessage(new Message {
                Sender = hub.bender,
                Content = string.Format("http://lmgtfy.com/?q={0}", HttpUtility.UrlEncode(cmd.Args[0])),
                Sent = DateTime.Now.ToString("MMM d, h:mm tt"),
                IsCode = false
            });
            return true;
        }

        public static bool MOAB(Command cmd, ChatHub hub) {
            hub.Clients.Group(hub.room.RoomID).receiveMessage(new Message {
                Sender = hub.bender,
                Content = "This hasn't been implemented yet",
                Sent = DateTime.Now.ToString("MMM d, h:mm tt"),
                IsCode = false
            });
            return true;
        }
    }


}
