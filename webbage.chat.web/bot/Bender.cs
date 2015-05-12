using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using webbage.chat.model;
using webbage.chat.web.hub;

namespace webbage.chat.web.bot.interpreters {
    public static class Bender {
        private static Dictionary<string, CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>>> commands;
        private static ChatHub hub;

        static Bender() {
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
                        Desc = CommandDescriptions.LMGTFY,
                        Parser = new Func<string, string[]>(ParseHelper.ParseDoubleQuotes),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.LMGTFY)
                    }
                },

                {
                    "!google",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.GOOGLE,
                        Parser = new Func<string, string[]>(ParseHelper.ParseDoubleQuotes),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.LMGTFY)
                    }
                },

                {
                    "!duck",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.DUCK,
                        Parser = new Func<string, string[]>(ParseHelper.ParseDoubleQuotes),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.Duck)
                    }
                },

                {
                    "!moab",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.MOAB,
                        Parser = new Func<string, string[]>(ParseHelper.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.MOAB)
                    }
                },

                {
                    "!kick",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.MOAB,
                        Parser = new Func<string, string[]>(ParseHelper.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.Kick)
                    }
                },

                {
                    "!guid",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.MOAB,
                        Parser = new Func<string, string[]>(ParseHelper.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.Guid)
                    }
                },

                {
                    "!youtube",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.YOUTUBE,
                        Parser = new Func<string, string[]>(ParseHelper.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.YouTube)
                    }
                },

                {
                    "!roll",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, ChatHub, bool>> {
                        Desc = CommandDescriptions.ROLL,
                        Parser = new Func<string, string[]>(ParseHelper.ParseNormal),
                        Action = new Func<Command, ChatHub, bool>(CommandExecutor.Roll)
                    }
                }
            };            
        }

        public static async Task DoWork(ChatHub chatHub, Message message) {
            Command cmd = new Command(message);
            hub = chatHub;

            if (!(validateCommand(cmd))) {
                await hub.Clients.Caller.receiveMessage(BotHelper.INVALID_COMMAND);
                return;
            }

            if (!(parseCommand(cmd))) {
                await hub.Clients.Caller.receiveMessage(BotHelper.INVALID_PARSE_COMMAND);
                return;
            }

            if (!(execCommand(cmd))) {
                await hub.Clients.Caller.receiveMessage(BotHelper.INVALID_EXEC_COMMAND);
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


    // move these to database when implemented????
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

        #region Normal Commands
        public static bool Help(Command cmd, ChatHub hub) {
            //// TODO: implement
            //hub.Clients.Group(hub.room.RoomID).receiveMessage(BotHelper.NOT_YET_IMPLEMENTED);
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;

            string message;
            switch(cmd.Args[0].ToString().ToLower())
            {
                case "duck":
                    message = CommandDescriptions.DUCK;
                    break;
                case "youtube":
                    message = CommandDescriptions.YOUTUBE;
                    break;
                case "roll":
                    message = CommandDescriptions.ROLL;
                    break;
                default:
                    message = null;
                    break;
            }
            if (message == null)
                return false;
            Message msg = BotHelper.GetMessage(message);
            hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            return true;
        }

        private static Random random;
        private static List<string> insults;
        public static bool Insult(Command cmd, ChatHub hub) {
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;

            Message msg = BotHelper.GetMessage(string.Format(insults[random.Next(0, insults.Count())], cmd.Args[0]));
            hub.Clients.Group(hub.room.RoomID).receiveMessage();
            return true;
        }

        public static bool LMGTFY(Command cmd, ChatHub hub) {
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;

            Message msg = BotHelper.GetMessage(string.Format("http://lmgtfy.com/?q={0}", HttpUtility.UrlEncode(cmd.Args[0])));
            hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            return true;
        }

        public static bool Duck(Command cmd, ChatHub hub)
        {
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;

            Message msg = BotHelper.GetMessage(string.Format("http://duckduckgo.com/?q={0}", HttpUtility.UrlEncode(cmd.Args[0])));
            hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            return true;
        }

        public static bool Roll(Command cmd, ChatHub hub)
        {
            Message msg;
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;
            int value;
            if (!int.TryParse(cmd.Args[0], out value))
                return false;
            if (value >= int.MaxValue)
            {
                msg = BotHelper.GetMessage("That number's too fucking big. Try again sucka.");
                hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            }
            else if(value <= 0)
            {
                msg = BotHelper.GetMessage("Number must be positive between 1 and 2147483646.");
                hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            }
            else
            {
                random = new Random();
                msg = BotHelper.GetMessage(random.Next(1, value + 1).ToString());
                hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            }

            
            return true;
        }

        public static bool Guid(Command cmd, ChatHub hub) {
            // TODO: implement
            hub.Clients.Group(hub.room.RoomID).receiveMessage(BotHelper.NOT_YET_IMPLEMENTED);
            return true;
        }
        #endregion

        #region Admin Commands
        public static bool Kick(Command cmd, ChatHub hub) {
            // TODO: implement
            if (cmd.CallerIsAdmin) {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotHelper.NOT_YET_IMPLEMENTED);
            } else {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotHelper.INVALID_PRIVILEGES);
            }
            return true;
        }

        public static bool YouTube(Command cmd, ChatHub hub)
        {
            // TODO: implement
            if (cmd.CallerIsAdmin)
            {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotHelper.NOT_YET_IMPLEMENTED);
            }
            else
            {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotHelper.INVALID_PRIVILEGES);
            }
            return true;
        }

        public static bool MOAB(Command cmd, ChatHub hub) {
            // TODO: implement
            if (cmd.CallerIsAdmin) {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotHelper.NOT_YET_IMPLEMENTED);
            } else {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotHelper.INVALID_PRIVILEGES);
            }
            return true;
        }
        #endregion

    }

    class BotHelper {
        public static User BENDER {
            get {
                return new User {
                    Name = "ಠ_ಠ",
                    Picture = "content/img/bender.jpg"
                };
            }
        }

        public static Message INVALID_COMMAND {
            get {
                return GetMessage("Unable to validate command. Make sure you typed a valid command name");
            }
        }
        public static Message INVALID_PARSE_COMMAND {
            get {
                return GetMessage("Unable to parse this command. Make sure you typed in valid parameters for it. If you need help, type in !help {commandName}");
            }
        }
        public static Message INVALID_EXEC_COMMAND {
            get {
                return GetMessage("Unable to execute this command. Make sure you typed in valid parameters for it. If you need help, type in !help {commandName}");
            }
        }
        public static Message INVALID_PRIVILEGES {
            get {
                return GetMessage("You don't have permission to do that");
            }
        }
        public static Message NOT_YET_IMPLEMENTED {
            get {
                return GetMessage("This hasn't been implemented yet");
            }
        }

        public static Message GetMessage(string content) {
            return new Message {
                Sender = BENDER,
                Content = content,
                Sent = DateTime.Now.ToString(),
                IsCode = false
            };
        }
    }


}
