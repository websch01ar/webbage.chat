using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(webbage.chat.Startup))]
namespace webbage.chat {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            app.MapSignalR();
        }
    }
}