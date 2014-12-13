using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

using webbage.chat.model;

namespace webbage.chat.web {
    public class ChatHub : Hub {
        public void Send(string user, string message) {
            Clients.All.broadcastMessage(user, message);
        }
    }
}