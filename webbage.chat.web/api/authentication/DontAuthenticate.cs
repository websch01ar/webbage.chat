using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace webbage.chat.web.api.authentication {
    public class DontAuthenticate : ActionFilterAttribute {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext) {
            base.OnActionExecuting(actionContext);
        }
    }
}