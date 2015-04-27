using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using webbage.chat.model;
using webbage.chat.context;

namespace webbage.chat.web.hub {
    public class ChatHub : Hub {
        private Room room {
            get {
                return GlobalData.Rooms.Where(r => r.RoomID == Context.QueryString["roomId"] && r.RoomKey == int.Parse(Context.QueryString["roomKey"])).FirstOrDefault();
            }
        }
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

        private User bender {
            get {
                return new User {
                    Name = "Bender",
                    Picture = "content/img/bender.jpg"
                };
            }
        }
        private IHubContext roomHub {
            get {
                return GlobalHost.ConnectionManager.GetHubContext<RoomHub>();
            }
        }

        #region Dis/Connection events
        public override Task OnConnected() {
            // check to see if the user is already connected on another connection id, if they are, update their connection id
            User previousUser = room.Users.Where(u => u.Name == user.Name && u.Picture == user.Picture).FirstOrDefault();
            if (previousUser != null) {
                Clients.User(previousUser.ConnectionId).disconnect();
                previousUser.ConnectionId = Context.ConnectionId;

                BroadcastMessage(new Message {
                    Sender = roomNotifier,
                    Content = string.Format("{0} has swittched to a new connection", user.Name),
                    Sent = DateTime.Now.ToString("MMM d, h:mm tt")
                });
            } else {
                room.Users.Add(user);
                Groups.Add(Context.ConnectionId, room.RoomID);
                BroadcastMessage(new Message {
                    Sender = roomNotifier,
                    Content = string.Format("{0} has connected", user.Name),
                    Sent = DateTime.Now.ToString("MMM d, h:mm tt")
                });
                Clients.Group(room.RoomID).updateOnlineUsers(room.Users);
                
                // broadcast it from the RoomHub as well, real-time list there of online users
                roomHub.Clients.All.userConnected(room);
            }            

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled) {
            room.Users.Remove(roomUser);
            BroadcastMessage(new Message {
                Sender = roomNotifier,
                Content = string.Format("{0} has disconnected", user.Name),
                Sent = DateTime.Now.ToString("MMM d, h:mm tt")
            });
            Clients.Group(room.RoomID).updateOnlineUsers(room.Users);           

            // broadcast it from the RoomHub as well, real-time list there of online users            
            roomHub.Clients.All.userDisconnected(room);

            return base.OnDisconnected(stopCalled);
        }

        public async Task UserDisconnect() {
            await OnDisconnected(true);
        }

        public async Task RemoveUser(User user) {
            User userToRemove = room.Users.Where(u => u.Name == user.Name && u.Picture == user.Picture).FirstOrDefault();

            await Clients.User(userToRemove.ConnectionId).disconnect();
        }
        #endregion

        public async Task BroadcastMessage(Message message) {
            message.Sent = DateTime.Now.ToString("MMM d, h:mm tt");
            room.Messages.Add(message);

            await Clients.All.receiveMessage(message);
        }
    }
}