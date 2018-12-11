using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WCAPP.Models.Database;
using WCAPP.Models.Home;
using WCAPP.Models.HomeModels;
using WCAPP.Utils;

namespace WCAPP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Empty()
        {
            return View();
        }

        public ActionResult Index()
        {
            if (Session.GetSessionUser() != null)
                return RedirectToAction("Index", "Process");

            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var db = new Context();

            var usr = db.Users.Include(x => x.AuthorityRs).SingleOrDefault(x => x.Id == model.UserId);
            if (usr != null && usr.Password == model.Password)
            {
                Session.SetSessionUser(new SessionUser {Id = usr.Id, Name = usr.Name, Authorities = usr.Authorities});
                return RedirectToAction("Index", "Process");
            }
            ModelState.AddModelError("", "用户名或密码不正确!");
            return View(); 
        }

        public ActionResult Logoff()
        {
            ViewBag.Title = "Home Page";
            Session.RemoveAll();

            return RedirectToAction("Index", "Home");
        }
    }
}