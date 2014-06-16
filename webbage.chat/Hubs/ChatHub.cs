using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web.Script.Serialization;
using webbage.chat.bot;
using System.Threading;

namespace webbage.chat.Hubs {
    public class ChatHub : Hub {

        /// TODO Feature List
        /// 
        ///     Implement op'd users that can do room commands (might need to wait for db to do that, for now just disallow room commands
        ///     Finish webbage.chat.bot
        ///         -- Google command (lmgtfy.com)
        ///         -- Wiki command
        ///     Notifications for @[userName] messages
        ///     Change tab title on new message in room

        // currently online user list
        public static Dictionary<string, string> onlineUsers = new Dictionary<string, string>();        

        // command interpreters
        private static BotCommandInterpreter botInterpreter = new BotCommandInterpreter();
        private static RoomCommandInterpreter roomInterpreter = new RoomCommandInterpreter();

        #region Server-to-Client Actions
        public void Send(string name, string message) {
            Clients.All.addNewMessageToPane(name, message);
            checkCommands(message);
        }
        public void UpdateOnlineUsers() {
            var jsonUsers = (new JavaScriptSerializer()).Serialize(onlineUsers);
            Clients.All.updateOnlineUsers(jsonUsers);
        }
        public void RemoveDisconnectedUser(string name) {
            Clients.All.removeDisconnectedUser(name);
        }
        #endregion

        #region Hub Overrides
        public override Task OnConnected() {
            // get the userName from the querystring set in jquery, send a notification
            // from room that the user has joined
            string user = getUser();
            Send("room", string.Format("{0} has joined the room", user));           

            onlineUsers.Add(Context.ConnectionId, user);

            UpdateOnlineUsers();

            return base.OnConnected();
        }
        public override Task OnDisconnected() {
            string user = getUser();
            
            // find them in the dictionary
            var disconnectedUser = onlineUsers.Where(u => u.Key == user || u.Value == user).FirstOrDefault();
            Send("room", string.Format("{0} has left the building", disconnectedUser.Value));

            onlineUsers.Remove(disconnectedUser.Key);
            RemoveDisconnectedUser(disconnectedUser.Value);
            return base.OnDisconnected();
        }
        #endregion

        #region Action Helpers
        private string getUser() {
            string clientId = "";
            if (!(Context.QueryString["userName"] == null))
                clientId = Context.QueryString["userName"].ToString();

            if (clientId.Trim() == "")
                clientId = Context.ConnectionId;

            return clientId;
        }
        private void checkCommands(string message) {
            if (message.StartsWith("!")) {
                (new Thread(() => { interpretBotCommand(message); })).Start();                
            } else if (message.StartsWith(".")) {
                (new Thread(() => { interpretRoomCommand(message); })).Start();
            }
        }
        private void interpretBotCommand(string message) {
            Command cmd = new Command(message);
            if (botInterpreter.DoCommand(cmd)) {
                Clients.All.addNewMessageToPane("bot", cmd.Response);
            } else {
                Clients.All.addNewMessageToPane("bot", "does not compute.");
            }
        }
        private void interpretRoomCommand(string message) {
            Command cmd = new Command(message);
            if (userIsAuthorized) {
                if (roomInterpreter.DoCommand(cmd)) {
                    Clients.All.addNewMessageToPane("room", cmd.Response);
                } else {
                    Clients.All.addNewMessageToPane("room", "does not compute.");
                }
            } else {
                Clients.All.addNewMessageToPane("room", "access denied.");
            }
        }
        private bool userIsAuthorized {
            get { return false; }
        }
        #endregion

    }
}