using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace webbage.chat.Hubs {
    public class ChatHub : Hub {
        static Dictionary<string, string> onlineUsers = new Dictionary<string, string>();

        public void Send(string name, string message) {
            Clients.All.addNewMessageToPane(name, message);
        }

        public void UpdateOnlineUsers() {
            var jsonUsers = (new JavaScriptSerializer()).Serialize(onlineUsers);
            Clients.All.updateOnlineUsers(jsonUsers);
        }

        public void RemoveDisconnectedUser(string name) {
            Clients.All.removeDisconnectedUser(name);
        }

        public override Task OnConnected() {
            // get the userName from the querystring set in jquery, send a notification
            // from room that the user has joined
            string user = GetUser();
            Send("room", string.Format("{0} has joined the room", user));           

            onlineUsers.Add(Context.ConnectionId, user);

            // TODO: Update #onlineUserList
            UpdateOnlineUsers();

            return base.OnConnected();
        }

        public override Task OnDisconnected() {
            string user = GetUser();
            
            // find them in the dictionary
            var disconnectedUser = onlineUsers.Where(u => u.Key == user || u.Value == user).FirstOrDefault();
            Send("room", string.Format("{0} has left the building", disconnectedUser.Value));

            onlineUsers.Remove(disconnectedUser.Key);
            RemoveDisconnectedUser(disconnectedUser.Value);
            return base.OnDisconnected();
        }

        private string GetUser() {
            string clientId = "";
            if (!(Context.QueryString["userName"] == null))
                clientId = Context.QueryString["userName"].ToString();

            if (clientId.Trim() == "")
                clientId = Context.ConnectionId;

            return clientId;
        }

    }
}