using Microsoft.AspNet.SignalR;

using webbage.chat.model;

namespace webbage.chat.web.Hubs {
    public class ChatHub : Hub {
        public void SendMessage(ChatMessage message) {
            Clients.All.broadcastMessage("test");
        }
    }
}