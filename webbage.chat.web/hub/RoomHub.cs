using Microsoft.AspNet.SignalR;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using webbage.chat.context;
using webbage.chat.model;

namespace webbage.chat.web.hub {
    public class RoomHub : Hub {
        public List<Room> GetRooms() {
            return GlobalData.Rooms;
        }

        public async Task AddRoom(Room room) {
            room.RoomKey = GetRooms().Max(r => r.RoomKey) + 1;
            room.RoomID = room.Name.Replace(" ", string.Empty);
            room.Messages = new List<Message>();
            room.Users = new List<User>();
            GlobalData.Rooms.Add(room);

            await Clients.All.getNewRoom(room);
        }
    }
}
