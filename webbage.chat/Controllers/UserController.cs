using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace webbage.chat.Controllers {
    public class UserController : Controller {

        [Route("user/login")]
        public ActionResult LogIn() {
            return View();
        }

        [Route("user/signup")]
        public ActionResult SignUp() {
            return View();
        }

    }
}