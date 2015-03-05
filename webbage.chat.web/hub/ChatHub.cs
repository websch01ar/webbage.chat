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
        private RoomHub roomHub {
            get {
                return GlobalHost.ConnectionManager.GetHubContext<RoomHub>();
            }
        }

        public override Task OnConnected() {
            room.Users.Add(user);            
            return base.OnConnected();
        }

        public async Task UserConnect() {
            await Groups.Add(Context.ConnectionId, room.RoomID);
            await Clients.OthersInGroup(room.RoomID).userConnected(user);
            await Clients.Group(room.RoomID).updateOnlineUsers(room.Users);

            // broadcast it from the RoomHub as well, real-time list there of online users
            roomHub.Clients.All.userConnected(room, user);
        }

        public override Task OnDisconnected(bool stopCalled) {            
            return base.OnDisconnected(stopCalled);
        }

        public async Task UserDisconnect() {
            room.Users.Remove(roomUser);
            await Clients.OthersInGroup(room.RoomID).userDisconnected(user);
            await Clients.Group(room.RoomID).updateOnlineUsers(room.Users);

            // broadcast it from the RoomHub as well, real-time list there of online users
            roomHub.Clients.All.userDisconnected(room, user);
        }
    }
}