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

        private static List<User> onlineUsers = new List<User>();

        // command interpreters
        private static BotCommandInterpreter botInterpreter = new BotCommandInterpreter();
        private static RoomCommandInterpreter roomInterpreter = new RoomCommandInterpreter();

        // remove user from onlineUsers based on ConnectionId, notify everyone else they left
        public override Task OnDisconnected() {
            return base.OnDisconnected();
        }

        #region Client-to-Server Actions
        #region Public
        public Task SendMessage(string message, bool isCodeMessage, string roomId) {
            string validatedMessage = null;
            // filter the message
            if (validateMessage(message, out validatedMessage)) {
                // determine if this is a private message
                if (validatedMessage.StartsWith("*")) {
                    string recipientName = validatedMessage.Split(' ')[0].Replace("*", ""); // get the recipient's name
                    validatedMessage = validatedMessage.Replace(recipientName, "").Replace("*", ""); // update the message
                    return sendToUser(recipientName, validatedMessage, isCodeMessage, roomId);
                } else {
                    return sendToRoom(validatedMessage, isCodeMessage, roomId);
                }
            }
            return null;
        }
        public async Task JoinRoom(string roomId) {
            User user = new User { UserName = Context.QueryString["userName"], ConnectionId = Context.ConnectionId, RoomId = roomId };
            onlineUsers.Add(user);
            
            await Groups.Add(Context.ConnectionId, roomId);
            await Clients.OthersInGroup(roomId).userConnected(user.UserName);
            updateOnlineUsers(roomId);
        }
        public async Task LeaveRoom(string roomId) {
            User disconnectedUser = onlineUsers.First(u => u.ConnectionId == Context.ConnectionId && u.RoomId == roomId);
            onlineUsers.Remove(disconnectedUser);

            await Groups.Remove(Context.ConnectionId, roomId);
            await Clients.OthersInGroup(roomId).userDisconnected(disconnectedUser.UserName);
            updateOnlineUsers(roomId);
        }
        #endregion

        #region Private
        private async Task sendToRoom(string message, bool isCodeMessage, string roomId) {
            User user = onlineUsers.First(u => u.ConnectionId == Context.ConnectionId && u.RoomId == roomId);

            await Clients.Group(roomId).addNewMessageToPane(user.UserName, message, false, isCodeMessage, roomId);
            await determineBotActions(user, message);
        }
        private Task determineBotActions(User user, string message) {
            if (message.StartsWith("!")) {
                Command cmd = new Command(message);
                botInterpreter.DoWork(cmd);

                if (cmd.Response != "invalid command")
                    return Clients.All.addNewMessageToPane("bot", cmd.Response, false);
                else
                    return Clients.Client(user.ConnectionId).addNewMessageToPane("bot", cmd.Response, true);

            } else if (message.StartsWith("/")) {
                if (userIsAuthorized) {
                    Command cmd = new Command(message);
                    roomInterpreter.DoWork(cmd);

                    if (cmd.Response != "invalid command")
                        return Clients.All.addNewMessageToPane("room", cmd.Response, false);
                    else
                        return Clients.Client(user.ConnectionId).addNewMessageToPane("room", cmd.Response, true);

                } else {
                    return Clients.Client(user.ConnectionId).addNewMessageToPane("room", "you don't have the necessary permissions to do that", true);
                }
            }
            return null;
        }        

        private async Task sendToUser(string recipient, string message, bool isCodeMessage, string roomId) {
            User user = onlineUsers.First(u => u.ConnectionId == Context.ConnectionId && u.RoomId == roomId);
            User receiver = onlineUsers.FirstOrDefault(u => u.UserName.ToLower() == recipient.ToLower() && u.RoomId == roomId);

            if (receiver != null && user.ConnectionId != receiver.ConnectionId) {
                await Clients.Client(receiver.ConnectionId).addNewMessageToPane(user.UserName, message, true, isCodeMessage, roomId);
                await Clients.Client(user.ConnectionId).addNewMessageToPane(user.UserName, message, true, isCodeMessage, roomId);
            } else {
                await Clients.Client(user.ConnectionId).addNewMessageToPane("room", "user not found", true, false, roomId);
            }
        }        
        #endregion
        #endregion

        #region Server-to-Client Actions
        private void updateOnlineUsers(string roomId) {
            var jsonUsers = (new JavaScriptSerializer()).Serialize(onlineUsers.Where(u => u.RoomId == roomId));
            Clients.Group(roomId).updateOnlineUsers(jsonUsers);
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