﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace webbage.chat.web {
    public class ChatHub : Hub {
        public void Send(string user, string message) {
            Clients.All.broadcastMessage("test", "test");
        }
    }
}