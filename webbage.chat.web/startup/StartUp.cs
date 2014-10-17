using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(webbage.chat.web.startup.StartUp))]

namespace webbage.chat.web.startup {
    public class StartUp {
        public void Configuration(IAppBuilder app) {
            app.MapSignalR();
        }
    }
}
