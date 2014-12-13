using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace webbage.chat.web.api.authentication {
    public class ApiAuth : AuthorizationFilterAttribute {
        public override void OnAuthorization(HttpActionContext context) {
            if (context.ActionDescriptor.GetCustomAttributes<DontAuthenticate>().Any())
                return;

            var authHeader = context.Request.Headers.Authorization;
            var valid = true;
            if (authHeader != null) {
                if (authHeader.Scheme.Equals("bearer", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(authHeader.Parameter)) {
                    if (authHeader.Parameter != ConfigurationManager.AppSettings.Get("OAuthToken"))
                        valid = false;
                } else valid = false;
            } else valid = false;

            if (!valid)
                context.Response = context.Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
    }
}