using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webbage.chat.model;
using webbage.chat.web.hub;

namespace webbage.chat.web.bot.helper {
    class CommandExecutors {

        static CommandExecutors() {
            
        }

        #region Normal Commands
        internal static bool Help(Command cmd, ChatHub hub) {
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
        
        internal static bool Insult(Command cmd, ChatHub hub) {
            try {
                Message msg = BotMessenger.GetMessage(string.Format(cmd.Dynamic.Insult, cmd.Dynamic.Recipient));
                hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
                return true;
            } catch {
                return false;
            }
        }

        internal static bool LMGTFY(Command cmd, ChatHub hub) {
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;

            Message msg = BotMessenger.GetMessage(string.Format("http://lmgtfy.com/?q={0}", HttpUtility.UrlEncode(cmd.Args[0])));
            hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            return true;
        }

        internal static bool Roll(Command cmd, ChatHub hub) {
            //TODO: implement figuring out the random number here using cmd.Dynamic.Number and cmd.Dynamic.MaxNumber

            Message msg = BotMessenger.GetMessage("Not implemented");
            hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            return true;
        }

        internal static bool GUID(Command cmd, ChatHub hub) {
            Message msg = BotMessenger.GetMessage(cmd.Dynamic.GUID);
            hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            return true;
        }

        internal static bool Duck(Command cmd, ChatHub hub) {
            if (cmd.Args.Length != 1 || cmd.Args[0] == "")
                return false;

            Message msg = BotMessenger.GetMessage(string.Format("http://duckduckgo.com/?q={0}", HttpUtility.UrlEncode(cmd.Args[0])));
            hub.Clients.Group(hub.room.RoomID).receiveMessage(msg);
            return true;
        }
        #endregion

        #region Admin Commands
        internal static bool Kick(Command cmd, ChatHub hub) {
            // TODO: implement
            if (cmd.CallerIsAdmin) {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotMessenger.NOT_YET_IMPLEMENTED);
            } else {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotMessenger.INVALID_PRIVILEGES);
            }
            return true;
        }

        internal static bool YouTube(Command cmd, ChatHub hub) {
            // TODO: implement
            if (cmd.CallerIsAdmin) {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotMessenger.NOT_YET_IMPLEMENTED);
            } else {
                hub.Clients.Group(hub.room.RoomID).receiveMessage(BotMessenger.INVALID_PRIVILEGES);
            }
            return true;
        }

        internal static bool MOAB(Command cmd, ChatHub hub) {
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
