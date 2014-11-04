using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using webbage.chat.bot;
using webbage.chat.model.ef;
using webbage.chat.utils;

namespace webbage.chat.Hubs {
    public class ChatHub : Hub {

        private static List<User> onlineUsers = new List<User>();

        // command interpreters
        private static BotCommandInterpreter botInterpreter = new BotCommandInterpreter();
        private static RoomCommandInterpreter roomInterpreter = new RoomCommandInterpreter();

        // remove user from onlineUsers based on ConnectionId, notify everyone else they left
        public override async Task OnConnected() {
            // get the room id
            string roomId = Context.QueryString["roomId"];
            // add them to onlineUsers
            User user = new User { UserName = (Context.QueryString["userName"].ToString() == "Colin" ? "Collins" : Context.QueryString["userName"].ToString()), ConnectionId = Context.ConnectionId, RoomId = roomId };
            onlineUsers.Add(user);

            // add them to this room's group
            await Groups.Add(Context.ConnectionId, roomId);
            // send out notification that they joined
            await Clients.OthersInGroup(roomId).userConnected(user.UserName, Guid.NewGuid());
            updateOnlineUsers(roomId);
        }        
        public override async Task OnDisconnected() {
            // get the room id
            string roomId = Context.QueryString["roomId"];
            // remove them from online users
            User disconnectedUser = onlineUsers.First(u => u.ConnectionId == Context.ConnectionId && u.RoomId == roomId);
            onlineUsers.Remove(disconnectedUser);

            // no need to remove from group here, should be already removed in OnDisconnected()
            // send out notification that they've left
            await Clients.OthersInGroup(roomId).userDisconnected(disconnectedUser.UserName, Guid.NewGuid());
            updateOnlineUsers(roomId);
        }

        #region Client-to-Server Actions
        #region Public
        public Task SendMessage(string message, bool isCodeMessage, string roomId) {
            string validatedMessage = null;
            // filter the message
            if (validateMessage(message, out validatedMessage)) {
                // determine if this is a private message
                if (validatedMessage.StartsWith("*")) {
                    // get the recipient's name
                    string recipientName = validatedMessage.Split(' ')[0].Replace("*", "");
                    // get the actual message
                    validatedMessage = validatedMessage.Replace("*" + recipientName, "");
                    return sendToUser(recipientName, validatedMessage, isCodeMessage, roomId);
                } else {
                    return sendToRoom(validatedMessage, isCodeMessage, roomId);
                }
            }
            return null;
        }
        #endregion
        #region Private
        // send the message out to the room
        private async Task sendToRoom(string message, bool isCodeMessage, string roomId) {
            User user = onlineUsers.First(u => u.ConnectionId == Context.ConnectionId && u.RoomId == roomId);

            await Clients.Group(roomId).addNewMessageToPane(user.UserName, message, false, isCodeMessage, Context.ConnectionId, Guid.NewGuid());
            await determineBotActions(user, message);
        }
        // see if we need to do any bot actions based on the message sent
        private Task determineBotActions(User user, string message) {
            if (message.StartsWith("!")) { // bot command
                Command cmd = new Command(message);
                botInterpreter.DoWork(cmd);

                string validatedMessage = null;
                if (cmd.Response != "invalid command") {
                    if (validateMessage(cmd.Response, out validatedMessage)) {
                        return Clients.All.addNewMessageToPane("bot", validatedMessage, false, null, Guid.NewGuid());
                    } else {
                        return Clients.Client(user.ConnectionId).addNewMessageToPane("bot", cmd.Response, true, null, Guid.NewGuid());
                    }
                } else {
                    return Clients.Client(user.ConnectionId).addNewMessageToPane("bot", cmd.Response, true, null, Guid.NewGuid());
                }

            } else if (message.StartsWith("/")) { // room command
                if (userIsAuthorized) {
                    Command cmd = new Command(message);
                    roomInterpreter.DoWork(cmd);

                    if (cmd.Response != "invalid command")
                        return Clients.All.addNewMessageToPane("room", cmd.Response, false, "0", Guid.NewGuid());
                    else
                        return Clients.Client(user.ConnectionId).addNewMessageToPane("room", cmd.Response, true, "0", Guid.NewGuid());

                } else {
                    return Clients.Client(user.ConnectionId).addNewMessageToPane("room", "you don't have the necessary permissions to do that", true, "0", Guid.NewGuid());
                }
            }
            return null;
        }        

        // send the message out to a particular user
        private async Task sendToUser(string recipient, string message, bool isCodeMessage, string roomId) {
            User user = onlineUsers.First(u => u.ConnectionId == Context.ConnectionId && u.RoomId == roomId);
            User receiver = onlineUsers.FirstOrDefault(u => u.UserName.ToLower() == recipient.ToLower() && u.RoomId == roomId);

            // if we found the person to send it to, append it to the sender and reciever's message panes
            // TODO: Figure out one day if we can do this with jquery tabs
            if (receiver != null && user.ConnectionId != receiver.ConnectionId) {
                await Clients.Client(receiver.ConnectionId).addNewMessageToPane(user.UserName, message, true, isCodeMessage, Context.ConnectionId, Guid.NewGuid());
                await Clients.Client(user.ConnectionId).addNewMessageToPane(user.UserName, message, true, isCodeMessage, Context.ConnectionId, Guid.NewGuid());
            } else {
                await Clients.Client(user.ConnectionId).addNewMessageToPane("room", "user not found", true, false, "0", Guid.NewGuid());
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

            validatedMessage = HtmlEncoder.Sanitize(message);
            validatedMessage = HtmlEncoder.EncodeUrl(validatedMessage);
            return true;
        }
        #endregion

    }
}
