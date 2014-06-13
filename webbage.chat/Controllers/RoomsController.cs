using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace webbage.chat.Controllers {
    public class RoomsController : Controller {

        [Route("rooms")]
        public ActionResult Index() {
            return View();
        }

        [Route("rooms/{roomNumber:int}/{roomName}")]
        public ActionResult GetRoom(int roomNumber, string roomName) {
            ViewBag.Title = roomName;

            return View();
        }

    }
}