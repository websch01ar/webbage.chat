using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using webbage.chat.model;
using webbage.chat.context;
using webbage.chat.web.bot;

namespace webbage.chat.web.hub {
    public class ChatHub : Hub {        
        private User user {
            get {
                return new User {
                    ConnectionId = Context.ConnectionId,
                    Name = Context.QueryString["userName"],
                    Picture = Context.QueryString["userPicture"]
                };
            }
        }
        private User roomUser {
            get {
                return room.Users.Where(u => u.Name == user.Name && u.Picture == user.Picture).FirstOrDefault();
            }
        }
        private User roomNotifier {
            get {
                return new User {
                    Name = "room",
                    Picture = ""
                };
            }
        }        
        private IHubContext roomHub {
            get {
                return GlobalHost.ConnectionManager.GetHubContext<RoomHub>();
            }
        }

        public Room room {
            get {
                return GlobalData.Rooms.Where(r => r.RoomID == Context.QueryString["roomId"] && r.RoomKey == int.Parse(Context.QueryString["roomKey"])).FirstOrDefault();
            }
        }        

        #region Dis/Connection events
        public override async Task OnConnected() {
            // bot has a connection to chathub, but we don't want to show him connecting and disconnecting
            if (Context.QueryString["userName"] == "bender")
                return;

            // check to see if the user is already connected on another connection id, if they are, update their connection id
            User previousUser = room.Users.Where(u => u.Name == user.Name && u.Picture == user.Picture).FirstOrDefault();
            if (previousUser != null) {
                await Clients.Client(previousUser.ConnectionId).kill();
                // await Groups.Remove(previousUser.ConnectionId, room.RoomID);

                previousUser.ConnectionId = Context.ConnectionId;
                await Groups.Add(previousUser.ConnectionId, room.RoomID);

                await Task.Delay(1000); // wait for the old connection to send the disconnected message
                await BroadcastMessage(new Message {
                    Sender = roomNotifier,
                    Content = string.Format("{0} has switched to a new connection", user.Name),
                    Sent = DateTime.Now.ToString()
                });
            } else {
                room.Users.Add(user);
                await Groups.Add(Context.ConnectionId, room.RoomID);
                await BroadcastMessage(new Message {
                    Sender = roomNotifier,
                    Content = string.Format("{0} has connected", user.Name),
                    Sent = DateTime.Now.ToString()
                });                
            }

            if (roomUser == null) { // make sure we have the user in the array (might not be if this is a new connection switch)
                room.Users.Add(user);
            }

            await Clients.Group(room.RoomID).updateOnlineUsers(room.Users);
            await roomHub.Clients.All.userConnected(room);
            await base.OnConnected();
        }

        public override async Task OnDisconnected(bool stopCalled) {
            room.Users.Remove(roomUser);
            await BroadcastMessage(new Message {
                Sender = roomNotifier,
                Content = string.Format("{0} has disconnected", user.Name),
                Sent = DateTime.Now.ToString()
            });
            await Clients.Group(room.RoomID).updateOnlineUsers(room.Users);           

            // broadcast it from the RoomHub as well, real-time list there of online users            
            await roomHub.Clients.All.userDisconnected(room);

            await base.OnDisconnected(stopCalled);
        }

        public async Task UserDisconnect() {
            await OnDisconnected(true);
        }

        public async Task RemoveUser(User user) {
            User userToRemove = room.Users.Where(u => u.Name == user.Name && u.Picture == user.Picture).FirstOrDefault();

            // you can't kick yourself!
            if (userToRemove.ConnectionId == roomUser.ConnectionId) {
                return;
            }

            await Clients.Client(userToRemove.ConnectionId).kill();
            await BroadcastMessage(new Message {
                Sender = roomNotifier,
                Content = string.Format("{0} has been kicked by {1}", userToRemove.Name, roomUser.Name),
                Sent = DateTime.Now.ToString()
            });
        }
        #endregion

        public async Task BroadcastMessage(Message message) {
            message.Sent = DateTime.Now.ToString();
            room.Messages.Add(message);

            await Clients.Group(room.RoomID).receiveMessage(message);

            // test to see if we should do something special with this message
            if (message.Content.StartsWith(Characters.COMMAND_CHARACTER)) { // run a command
                await BroadcastCommand(message);
            }
        }

        public async Task BroadcastCommand(Message message) {
            Command cmd = Bender.DoWork(this, message);
            await Clients.Group(room.RoomID).receiveMessage(cmd.Response);            
        }
    }

    static class Characters {
        public const string COMMAND_CHARACTER = "!";        
    }
}