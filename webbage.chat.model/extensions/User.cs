﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webbage.chat.model.ef {
    public partial class User {
        public string ConnectionId { get; set; }
        public string RoomId { get; set; }
    }
}