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
        public async void Connect(User user, Room room) {
            // add the user to the users list of the approriate room in GlobalData.Rooms
            Room globalRoom = GlobalData.Rooms.Where(r => r.RoomKey == room.RoomKey).FirstOrDefault();
            globalRoom.Users.Add(user);

            // add them to the SignalR group and then distribute a message everyone else
            // to say they connected also distribute a message to the entire group to 
            // update their online users list
            await Groups.Add(Context.ConnectionId, room.RoomID);
            await Clients.OthersInGroup(room.RoomID).userConnected(user);
            await Clients.Group(room.RoomID).updateOnlineUsers(globalRoom);
        }
    }
}