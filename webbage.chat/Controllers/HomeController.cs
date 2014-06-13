using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace webbage.chat.Controllers {
    public class HomeController : Controller {

        public ActionResult Index() {
            return View();
        }

        [Route("home/about")]
        public ActionResult About() {
            return View();
        }

        [Route("home/contact")]
        public ActionResult Contact() {
            return View();
        }
    }
}