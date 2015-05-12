using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webbage.chat.model;
using webbage.chat.web.hub;

namespace webbage.chat.web.bot.helper {
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
            //hub.Clients.Group(hub.room.RoomID).receiveMessage(BotMessenger.NOT_YET_IMPLEMENTED);
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;

            string message;
            switch (cmd.Args[0].ToString().ToLower()) {
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
            Message msg = BotMessenger.GetMessage(message);
            hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            return true;
        }

        private static Random random;
        private static List<string> insults;
        public static bool Insult(Command cmd, ChatHub hub) {
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;

            Message msg = BotMessenger.GetMessage(string.Format(insults[random.Next(0, insults.Count())], cmd.Args[0]));
            hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            return true;
        }

        public static bool LMGTFY(Command cmd, ChatHub hub) {
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;

            Message msg = BotMessenger.GetMessage(string.Format("http://lmgtfy.com/?q={0}", HttpUtility.UrlEncode(cmd.Args[0])));
            hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            return true;
        }

        public static bool Duck(Command cmd, ChatHub hub) {
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;

            Message msg = BotMessenger.GetMessage(string.Format("http://duckduckgo.com/?q={0}", HttpUtility.UrlEncode(cmd.Args[0])));
            hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            return true;
        }

        public static bool Roll(Command cmd, ChatHub hub) {
            Message msg;
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;

            int value;
            if (!int.TryParse(cmd.Args[0], out value))
                return false;
            if (value >= int.MaxValue) {
                msg = BotMessenger.GetMessage("That number's too fucking big. Try again sucka.");
            } else if (value <= 0) {
                msg = BotMessenger.GetMessage("Number must be positive between 1 and 2147483646.");
            } else {
                random = new Random();
                msg = BotMessenger.GetMessage(random.Next(1, value + 1).ToString());
            }

            hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            return true;
        }

        public static bool GUID(Command cmd, ChatHub hub) {
            Message msg = BotMessenger.GetMessage(Guid.NewGuid().ToString());
            hub.Clients.Group(hub.room.RoomID).receiveMessage(BotMessenger.NOT_YET_IMPLEMENTED);
            return true;
        }
        #endregion

        #region Admin Commands
        public static bool Kick(Command cmd, ChatHub hub) {
            // TODO: implement
            if (cmd.CallerIsAdmin) {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotMessenger.NOT_YET_IMPLEMENTED);
            } else {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotMessenger.INVALID_PRIVILEGES);
            }
            return true;
        }

        public static bool YouTube(Command cmd, ChatHub hub) {
            // TODO: implement
            if (cmd.CallerIsAdmin) {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotMessenger.NOT_YET_IMPLEMENTED);
            } else {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotMessenger.INVALID_PRIVILEGES);
            }
            return true;
        }

        public static bool MOAB(Command cmd, ChatHub hub) {
            // TODO: implement
            if (cmd.CallerIsAdmin) {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotMessenger.NOT_YET_IMPLEMENTED);
            } else {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotMessenger.INVALID_PRIVILEGES);
            }
            return true;
        }
        #endregion

    }
}
