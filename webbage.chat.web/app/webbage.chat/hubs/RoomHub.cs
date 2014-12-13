using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using webbage.chat.model;

namespace webbage.chat.web.app.webbage.chat.hubs {
    public class RoomHub : Hub {
        private static List<ChatRoom> rooms = new List<ChatRoom>();

        public void GetRooms() {
            Clients.Caller.updateRooms(rooms);
        }
    }
}