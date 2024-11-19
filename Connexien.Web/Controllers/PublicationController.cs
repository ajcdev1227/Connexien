using Connexien.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Connexien.Web.Controllers
{
    public class PublicationController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Dashboard", "Publication");
        }

        // GET: Publication
        [HttpGet, Authorize]
        public ActionResult Dashboard(string session_id)
        {
            ViewBag.Nav = (int)Menu.Publications;

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

            var provider = ProviderModel.Get(UserModel.GetUserId());
            var contentPartner = ContentPartnerModel.Get(UserModel.GetUserId());

            if (!provider.Profiles.Any() && !contentPartner.Profiles.Any())
            {
                return RedirectToAction("Setup", "Provider");
            }

            //Eventually there may be need for multiple profiles per user... for now just default to one
            return View(provider.Profiles.First());
        }

        [HttpGet, Authorize]
        public ActionResult DeletePublication(string id)
        {
            var profile = UserModel.GetProfiles().FirstOrDefault();
            var publication = PublicationModel.Get(id);

            if (profile.Id != publication.ProfileId)
            {
                TempData["Msg"] = $"You do not have permissions to delete that publication item";
                TempData["MsgType"] = "error";
                ViewBag.ErrorMsg = TempData["Msg"];
                return RedirectToAction("Dashboard", "Publication");
            }

            PublicationModel.Delete(publication.Id);

            return RedirectToAction("Dashboard", "Publication");
        }

        [HttpGet, Authorize]
        public ActionResult CreatePublication()
        {
            var profile = UserModel.GetProfiles().FirstOrDefault();
            PublicationModel publication = new PublicationModel { ProfileId = profile.Id, ProfileGuid = profile.ProfileGuid };

            return PartialView("_PublicationCreate", publication);
        }

        [HttpGet, Authorize]
        public ActionResult EditPublication(string id)
        {
            var profile = UserModel.GetProfiles().FirstOrDefault();
            var publication = PublicationModel.Get(id);

            if (profile.Id != publication.ProfileId)
            {
                TempData["Msg"] = $"You do not have permissions to edit that publication item";
                TempData["MsgType"] = "error";
                ViewBag.ErrorMsg = TempData["Msg"];
                return RedirectToAction("Dashboard", "Publication");
            }

            return PartialView("_PublicationEdit", publication);
        }

        
		[HttpPost]
		public ActionResult CreateCheckoutSession()
		{
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                UiMode = "embedded",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Connexien",
                                Description = "Published Article",
                                Images = new List<string>
                                {
                                    "https://connexien.blob.core.windows.net/content/images/Ball_75.png"
                                },
                            },
                            UnitAmount = 999, // amount in cents
                        },
                        Quantity = 1,
                    },
                },
                SubmitType = "auto",
                Mode = "payment",
                RedirectOnCompletion = "never",
                //ReturnUrl = domain + "/Publication/Dashboard?session_id={CHECKOUT_SESSION_ID}",
            };
			var service = new SessionService();
			Session session = service.Create(options);

			return Json(new { clientSecret = session.ClientSecret });
		}

        [HttpGet]
        public ActionResult SessionStatus(string session_id)
        {
            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);

            return Json(new { status = session.Status, customer_email = session.CustomerDetails.Email }, JsonRequestBehavior.AllowGet);
        }
    }

    public enum Menu
    {
        Overview,
        Edit,
        Personal,
        Company,
        Experience,
        Industry,
        Expertise,
        Blank,
        Profile,
        Publications,
        Resources,
        ContentPartner
    }
}