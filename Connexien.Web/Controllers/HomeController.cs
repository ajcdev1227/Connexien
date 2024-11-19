using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Connexien.Web.Models;

namespace Connexien.Web.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            if (!string.IsNullOrWhiteSpace((string)TempData["Msg"]))
            {
                if (!string.IsNullOrWhiteSpace((string)TempData["MsgType"]) && (string)TempData["MsgType"] == "success")
                {
                    ViewBag.SuccessMsg = TempData["Msg"];
                }
                else
                {
                    ViewBag.ErrorMsg = TempData["Msg"];
                }
            }

            //if (User.Identity.IsAuthenticated)
            //    return RedirectToAction("RedirectToDefault", "Account");

            return View(new SearchModel());
        }

        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View("Terms");
        }


        public ActionResult Error()
        {
            return View("Error");
        }
    }
}
