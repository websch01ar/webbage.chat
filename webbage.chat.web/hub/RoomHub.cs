using Microsoft.AspNet.SignalR;

using System.Collections.Generic;
using System.Threading.Tasks;

using webbage.chat.context;
using webbage.chat.model;

namespace webbage.chat.web.hub {
    public class RoomHub : Hub {
        public List<Room> GetRooms() {
            return GlobalData.Rooms;
        }

        public async Task AddRoom(Room room) {
            // room.RoomKey = soome way to figure out next RoomKey to use
            room.RoomID = room.Name.Replace(" ", string.Empty); // remove the spaces from the Name to get the RoomKey
            room.Messages = new List<Message>();
            room.Users = new List<User>();
            GlobalData.Rooms.Add(room);

            await Clients.All.getNewRoom(room);
        }
    }
}