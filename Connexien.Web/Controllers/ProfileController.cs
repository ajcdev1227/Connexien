using Connexien.Lib;
using Connexien.Web.Models;
using System.Linq;
using System.Web.Mvc;

namespace Connexien.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {

        public ActionResult Index(string id)
        {
            //long tmpId;
            //if (Int64.TryParse(id, out tmpId))
            //return RedirectToAction("Index", "Home");

            var provider = ProviderModel.Get(id);

            if (!provider.Profiles.Any())
                return RedirectToAction("Index", "Home");

            //Eventually there may be need for multiple profiles per user... for now just default to one
            Lib.Utilities.LogView(provider.Profiles.First().ProfileGuid, "profile", UserModel.GetUserId(), UserModel.GetIp());
            return View(provider.Profiles.First());
        }

        //----------------------------------------------------

        [HttpPost]
        public JsonResult Rate(string id, int rating)
        {
            return Json(ProfileModel.Rate(id, rating));
        }

        //----------------------------------------------------

        [AllowAnonymous]
        public ActionResult Picture(string id, int w = 0, int h = 0, string t = "avatar", bool fill = true)
        {
            Response.AppendHeader("Content-Disposition", "inline; filename=" + id);
            return File(Lib.Utilities.GetSizedPic(id.Replace(".jpg", ""), w, h, t, fill, Request.Url != null ? "http://" + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port) : "../../.."), "image/jpeg");
        }

        [AllowAnonymous]
        public ActionResult Badge(string id = "")
        {
            Response.AppendHeader("Content-Disposition", "inline; filename=Badge.jpg");

            return File(ConnexienStorageBlob.DownloadBlob("images/badge.jpg", ConnexienStorageContainers.StorageContainer.SiteContent), "image/jpeg");
        }

        [AllowAnonymous]
        public ActionResult Icon(string id = "")
        {
            Response.AppendHeader("Content-Disposition", "inline; filename=Icon.png");

            return File(ConnexienStorageBlob.DownloadBlob("images/icon.png", ConnexienStorageContainers.StorageContainer.SiteContent), "image/png");
        }
    }
}
