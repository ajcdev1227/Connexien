using Connexien.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Connexien.Web.Controllers
{
    public class SearchController : Controller
    {

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Index(SearchModel model)
        {
            if ((model.ProductTypes == null || !model.ProductTypes.Any()) && (model.ServiceTypes == null || !model.ServiceTypes.Any()))
            {
                TempData["MsgType"] = "error";
                TempData["Msg"] = "Please select at least one product and/or service to search for.";
                return RedirectToAction("Index", "Home");
            }

            return View(SearchModel.Search(model));
        }

        [HttpPost]
        public PartialViewResult Filter(ProfileModel model)
        {
            return PartialView("_Results", SearchModel.Filter(model));
        }
    }
}
