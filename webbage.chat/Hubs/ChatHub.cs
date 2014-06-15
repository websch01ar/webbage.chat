using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Diagnostics;

namespace webbage.chat.Hubs {
    public class ChatHub : Hub {

        public void Send(string name, string message) {
            Clients.All.addNewMessageToPane(name, message);
        }

        public override Task OnConnected() {           
            return base.OnConnected();
        }

        public override Task OnDisconnected() {
            return base.OnDisconnected();
        }

    }
}