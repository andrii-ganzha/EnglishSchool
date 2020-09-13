using EnglishSchool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnglishSchool.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Database.SetInitializer<ApplicationDbContext>(new AppDbInitializer());
            if ((User.IsInRole("Admin"))||(User.IsInRole("User")))
            {
                return RedirectToAction("Index", "Quizzes");
            }
            return RedirectToAction("Login", "Account", new { denied = true } );
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}