using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using webbage.chat.bot;
using webbage.chat.model;

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
        ///     

        // currently online user list
        private static List<User> onlineUsers = new List<User>();

        // command interpreters
        private static BotCommandInterpreter botInterpreter = new BotCommandInterpreter();
        private static RoomCommandInterpreter roomInterpreter = new RoomCommandInterpreter();

        // add user to onlineUsers
        public override Task OnConnected() {
            User user = new User { Name = Context.QueryString["userName"].ToString(), ConnectionId = Context.ConnectionId };
            onlineUsers.Add(user);
            Clients.Others.userConnected(user.Name);

            // update the online-user-list
            updateOnlineUsers();

            return base.OnConnected();
        }
        // remove user from onlineUsers based on ConnectionId, notify everyone else they left
        public override Task OnDisconnected() {
            if (onlineUsers.Any(u => u.ConnectionId == Context.ConnectionId)) {
                User user = onlineUsers.First(u => u.ConnectionId == Context.ConnectionId);
                onlineUsers.Remove(user);
                Clients.Others.userDisconnected(user.Name);
            }
            return base.OnDisconnected();
        }

        #region Client-to-Server Actions
        public void SendToRoom(string message) {
            User user = onlineUsers.First(u => u.ConnectionId == Context.ConnectionId);
            Clients.All.addNewMessageToPane(user.Name, message, false);

            determineBotActions(user, message);
        }
        public void SendToUser(string recipient, string message) {
            User user = onlineUsers.First(u => u.ConnectionId == Context.ConnectionId);
            User receiver = onlineUsers.FirstOrDefault(u => u.Name == recipient);

            if (!(receiver == null)) {
                Clients.Client(receiver.ConnectionId).addNewMessageToPane(user.Name, message, true);
                Clients.Client(user.ConnectionId).addNewMessageToPane(user.Name, message, true);
            } else {
                Clients.Client(user.ConnectionId).addNewMessageToPane("room", "user not found", true);
            }
        }
        private void determineBotActions(User user, string message) {
            if (message.StartsWith("!")) {
                Command cmd = new Command(message);
                botInterpreter.DoWork(cmd);
                
                if (cmd.Response != "invalid command")
                    Clients.All.addNewMessageToPane("bot", cmd.Response, false);
                else
                    Clients.Client(user.ConnectionId).addNewMessageToPane("bot", cmd.Response, true);

            } else if (message.StartsWith("/")) {                
                if (userIsAuthorized) {
                    Command cmd = new Command(message);
                    roomInterpreter.DoWork(cmd);

                    if (cmd.Response != "invalid command")
                        Clients.All.addNewMessageToPane("room", cmd.Response, false);
                    else
                        Clients.Client(user.ConnectionId).addNewMessageToPane("room", cmd.Response, true);

                } else {
                    Clients.Client(user.ConnectionId).addNewMessageToPane("room", "you don't have the necessary permissions to do that", true);
                }
            }
        }
        #endregion

        #region Server-to-Client Actions
        private void updateOnlineUsers() {
            var jsonUsers = (new JavaScriptSerializer()).Serialize(onlineUsers);
            Clients.All.updateOnlineUsers(jsonUsers);
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
        private bool userIsAuthorized {
            get { return false; }
        }
        #endregion

    }
}