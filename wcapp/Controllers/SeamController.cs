using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WCAPP.Controllers
{
    public class SeamController : Controller
    {
        public ActionResult ModifyParams(int id)
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult SubmitReport(int id)
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}