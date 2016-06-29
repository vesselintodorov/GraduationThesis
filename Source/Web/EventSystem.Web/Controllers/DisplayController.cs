using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventSystem.Web.Controllers
{
    public class DisplayController : Controller
    {
        public ActionResult Events()
        {
            return View();
        }
    }
}