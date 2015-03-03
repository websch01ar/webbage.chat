using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webbage.chat.model;

namespace webbage.chat.context {
    public class GlobalData {
        public GlobalData() {

        }

        static GlobalData() {
            rooms = new List<Room>{
                new Room { RoomKey = 1, RoomID = "sandbox", Name = "Sandbox", Description = "A test room to play around in", Users = new List<User> {
                    new User { Name = "Ben", Picture = "https://avatars.githubusercontent.com/u/3428699?v=3" },
                    new User { Name = "Ben", Picture = "https://avatars.githubusercontent.com/u/3428699?v=3" },
                    new User { Name = "Ben", Picture = "https://avatars.githubusercontent.com/u/3428699?v=3" },
                    new User { Name = "Ben", Picture = "https://avatars.githubusercontent.com/u/3428699?v=3" }
                }, Messages = null }
            };
        }

        private static List<Room> rooms { get; set; }
        public static List<Room> Rooms { get { return rooms; } }
    }
}
