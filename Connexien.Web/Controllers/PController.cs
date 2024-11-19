using Connexien.Web.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Connexien.Web.Controllers
{
    public class PController : Controller
    {
        public ActionResult Index(string id)
        {
            var provider = ProviderModel.Get(id);

            if (!provider.Profiles.Any())
                return RedirectToAction("Index", "Home");

            //Eventually there may be need for multiple profiles per user... for now just default to one
            Lib.Utilities.LogView(provider.Profiles.First().ProfileGuid, "profile", UserModel.GetUserId(), UserModel.GetIp());
            return View(provider.Profiles.First());
        }

        public ActionResult Articles(string id)
        {
            var provider = ProviderModel.Get(id);

            if (!provider.Profiles.Any())
                return RedirectToAction("Index", "P", new { id });

            //Eventually there may be need for multiple profiles per user... for now just default to one
            Lib.Utilities.LogView(provider.Profiles.First().ProfileGuid, "profile", UserModel.GetUserId(), UserModel.GetIp());
            return View(provider.Profiles.First());
        }
    }
}
