using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using webbage.chat.bot;
using webbage.chat.model.ef;

namespace webbage.chat.Hubs {
    public class ChatHub : Hub {

        // currently online user list
        private static List<User> onlineUsers = new List<User>();

        // command interpreters
        private static BotCommandInterpreter botInterpreter = new BotCommandInterpreter();
        private static RoomCommandInterpreter roomInterpreter = new RoomCommandInterpreter();

        // add user to onlineUsers
        public override Task OnConnected() {
            User user = new User { UserName = Context.QueryString["userName"].ToString(), ConnectionId = Context.ConnectionId };
            onlineUsers.Add(user);
            Clients.Others.userConnected(user.UserName);

            // update the online-user-list
            updateOnlineUsers();

            return base.OnConnected();
        }
        // remove user from onlineUsers based on ConnectionId, notify everyone else they left
        public override Task OnDisconnected() {
            if (onlineUsers.Any(u => u.ConnectionId == Context.ConnectionId)) {
                User user = onlineUsers.First(u => u.ConnectionId == Context.ConnectionId);
                onlineUsers.Remove(user);
                Clients.Others.userDisconnected(user.UserName);
            }
            return base.OnDisconnected();
        }

        #region Client-to-Server Actions
        public void SendMessage(string message, bool isCodeMessage) {
            string validatedMessage = null;
            // filter the message
            if (validateMessage(message, out validatedMessage)) {
                // determine if this is a private message
                if (validatedMessage.StartsWith("*")) {
                    string recipientName = validatedMessage.Split(' ')[0].Replace("*", ""); // get the recipient's name
                    validatedMessage = validatedMessage.Replace(recipientName, "").Replace("*", ""); // update the message
                    SendToUser(recipientName, validatedMessage, isCodeMessage);
                } else {
                    SendToRoom(validatedMessage, isCodeMessage);
                }
            }
        }
        public void SendToRoom(string message, bool isCodeMessage) {
            User user = onlineUsers.First(u => u.ConnectionId == Context.ConnectionId);
            Clients.All.addNewMessageToPane(user.UserName, message, false, isCodeMessage);

            determineBotActions(user, message);
        }
        public void SendToUser(string recipient, string message, bool isCodeMessage) {
            User user = onlineUsers.First(u => u.ConnectionId == Context.ConnectionId);
            User receiver = onlineUsers.FirstOrDefault(u => u.UserName.ToLower() == recipient.ToLower());

            if (receiver != null && user.ConnectionId != receiver.ConnectionId) {
                Clients.Client(receiver.ConnectionId).addNewMessageToPane(user.UserName, message, true, isCodeMessage);
                Clients.Client(user.ConnectionId).addNewMessageToPane(user.UserName, message, true, isCodeMessage);
            } else {
                Clients.Client(user.ConnectionId).addNewMessageToPane("room", "user not found", true, false);
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
        private bool validateMessage(string message, out string validatedMessage) {
            validatedMessage = null;

            // make it impossible to send empty or just whitespace messages
            if (string.IsNullOrWhiteSpace(message))
                return false;

            validatedMessage = message;
            return true;
        }
        #endregion

    }
}