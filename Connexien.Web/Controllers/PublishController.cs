using Connexien.Lib.Extensions;
using Connexien.Web.Models;
using System.Web;
using System.Web.Mvc;

namespace Connexien.Web.Controllers
{

    public class PublishController : Controller
    {
        public ActionResult Index()
        {
            return View();

        }

        [HttpPost]
        public ActionResult Search(string q)
        {
            ViewBag.Search = q;
            return View(PublicationModel.Search(q));

        }

        [HttpPost]
        public PartialViewResult Filter(PublicationModel model)
        {
            return PartialView("_Results", PublicationModel.Filter(model));
        }

        [HttpPost, Authorize]
        public ActionResult Submit(PublicationModel model)
        {
            if (string.IsNullOrEmpty(model.ArticleGuid))
            {
                Guard.AgainstNull(model.File, nameof(model.File));
                Guard.AgainstNull(model.StripeToken, nameof(model.StripeToken));
               
                string guid;
                if (PublicationModel.Submit(model, out guid))
                {
                    TempData["Msg"] = "Publication successfully submitted.";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Msg"] = "Publication failed to submit.";
                    TempData["MsgType"] = "error";
                }
            }
            else
            {
                if (PublicationModel.Update(model))
                {
                    TempData["Msg"] = "Publication successfully updated.";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Msg"] = "Publication could not be updated. Please try again.";
                    TempData["MsgType"] = "error";
                }
            }

            return RedirectToAction("Dashboard", "Publication", new { nav = (int)ProviderController.Menu.Publications });
        }
    }
}
