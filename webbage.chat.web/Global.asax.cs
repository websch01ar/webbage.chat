﻿using System;
using System.Linq;
using System.Web.Http;
using System.Web.Optimization;

namespace webbage.chat.web {
    public class Global : System.Web.HttpApplication {

        protected void Application_Start(object sender, EventArgs e) {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            BundleConfig.RegisterBundles(BundleTable.Bundles);            
        }
    }
}