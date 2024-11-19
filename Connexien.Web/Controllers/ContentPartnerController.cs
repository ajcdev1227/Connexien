using Connexien.Web.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Connexien.Web.Controllers
{
    /// <summary>
    /// Useful for testing scenarios:  https://stripe.com/docs/testing#cards
    /// </summary>
    public class ContentPartnerController : Controller
    {
        [HttpGet, Authorize(Roles = "contentpartner")]
        public ActionResult Dashboard()
        {
            ViewBag.Nav = (int)Menu.Overview;

            var contentPartner = ContentPartnerModel.Get(UserModel.GetUserId());

            if (!contentPartner.Profiles.Any())
            {
                return RedirectToAction("Setup", "ContentPartner");
            }
            else if (contentPartner.Profiles.First().PaymentAccount == null)
            {
                //return RedirectToAction("ConnectPayment", "ContentPartner");
            }

            //Eventually there may be need for multiple profiles per user... for now just default to one
            return View(contentPartner.Profiles.First());
        }


        public ActionResult Description()
        {
            return View();
        }

        #region Content

        [HttpGet, Authorize(Roles = "contentpartner")]
        public ActionResult ViewContent(string id)
        {
            var contentItem = ContentItemModel.Get(id);

            if (contentItem == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(contentItem);
        }

        [HttpGet, Authorize(Roles = "contentpartner")]
        public ActionResult EditContent(string id)
        {
            var contentPartner = ContentPartnerModel.Get(UserModel.GetUserId());
            var contentItem = ContentItemModel.Get(id);

            if (!contentPartner.Profiles.Any(x => x.Id == contentItem.ProfileId))
            {
                TempData["Msg"] = $"You do not have permissions to edit that content item";
                TempData["MsgType"] = "error";
                ViewBag.ErrorMsg = TempData["Msg"];
                return RedirectToAction("Dashboard", "ContentPartner");
            }

            contentItem.Updated = DateTime.UtcNow;

            return PartialView("_ContentItemEdit", contentItem);
        }

        [HttpPost, Authorize(Roles = "contentpartner")]
        [ValidateAntiForgeryToken]
        public ActionResult EditContent(ContentItemModel model)
        {
            if (ContentItemModel.Update(model))
            {
                TempData["Msg"] = "Content item successfully updated.";
                TempData["MsgType"] = "success";
                return Json(new { statusText = "Content item successfully updated." });
            }
            else
            {
                TempData["Msg"] = "Content item could not be updated. Please try again.";
                TempData["MsgType"] = "error";
                return Json(new { statusText = "Content item could not be updated. Please try again." });
            }
        }

        [HttpGet, Authorize(Roles = "contentpartner")]
        public ActionResult CreateContent()
        {
            var profile = ContentPartnerModel.Get(UserModel.GetUserId()).Profiles.FirstOrDefault();
            ContentItemModel contentItem = new ContentItemModel { ProfileId = profile.Id, ProfileGuid = profile.ProfileGuid };

            contentItem.Created = DateTime.UtcNow;
            contentItem.Updated = DateTime.UtcNow;

            return PartialView("_ContentItemCreate", contentItem);
        }

        [HttpPost, Authorize(Roles = "contentpartner")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateContent(ContentItemModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                TempData["Msg"] = $"Content item could not be updated due to the following reasons: {messages}";
                TempData["MsgType"] = "error";
                ViewBag.ErrorMsg = TempData["Msg"];
                return Json(new { statusText = $"Content item could not be updated due to the following reasons: {messages}" });
            }

            string guid = string.Empty;
            if (ContentItemModel.Submit(model, out guid))
            {
                TempData["Msg"] = "Content item successfully created.";
                TempData["MsgType"] = "success";
                ViewBag.SuccessMsg = TempData["Msg"];
                return Json(new { statusText = "Content item successfully created." });
            }
            else
            {
                TempData["Msg"] = "Content item could not be updated. Please try again.";
                TempData["MsgType"] = "error";
                ViewBag.ErrorMsg = TempData["Msg"];
                return Json(new { statusText = "Content item could not be updated. Please try again." });
            }
        }

        [HttpGet, Authorize(Roles = "contentpartner")]
        public ActionResult DeleteContentItem(long id)
        {
            var isDeleted = ContentItemModel.Delete(id);

            if (isDeleted)
            {
                TempData["Msg"] = "Content item was successfully deleted.";
                TempData["MsgType"] = "success";
                ViewBag.SuccessMsg = TempData["Msg"];
            }
            else
            {
                TempData["Msg"] = "Content item could not be deleted.";
                TempData["MsgType"] = "error";
                ViewBag.ErrorMsg = TempData["Msg"];
            }

            return RedirectToAction("Dashboard", "ContentPartner");
        }

        [HttpGet, Authorize(Roles = "contentpartner")]
        public ActionResult TermsAndConditions()
        {
            var contentPartner = ContentPartnerModel.Get(UserModel.GetUserId());

            if (!contentPartner.Profiles.Any())
            {
                return RedirectToAction("Setup", "ContentPartner");
            }

            //Eventually there may be need for multiple profiles per user... for now just default to one
            return View(contentPartner.Profiles.First());
        }

        [HttpGet, Authorize(Roles = "contentpartner")]
        public ActionResult Download(string id)
        {
            var guid = id.Split('.')[0];

            var contentItem = ContentItemModel.Get(guid);
            if (contentItem == null)
            {
                return RedirectToAction("Dashboard", "ContentPartner", new { guid });
            }

            Response.AppendHeader("Content-Disposition", "inline; filename=" + id);
            return File(Lib.Utilities.GetContentItem(contentItem.Filename), contentItem.MimeType);
        }

        #endregion

        #region Profile

        [Authorize(Roles = "contentpartner")]
        public ActionResult Section(string id)
        {

            ViewBag.Nav = (int)Enum.Parse(typeof(Menu), id);

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

            var contentPartner = ContentPartnerModel.Get(UserModel.GetUserId());

            if (!contentPartner.Profiles.Any())
            {
                return RedirectToAction("Setup", "ContentPartner");
            }

            return View(id, contentPartner.Profiles.First());
        }

        [Authorize(Roles = "contentpartner")]
        public ActionResult PreviewProfile()
        {
            ViewBag.Nav = (int)Menu.Preview;

            var contentPartner = ContentPartnerModel.Get(UserModel.GetUserId());

            if (!contentPartner.Profiles.Any())
            {
                return RedirectToAction("Setup", "ContentPartner");
            }
            else if (contentPartner.Profiles.First().PaymentAccount == null)
            {
                //return RedirectToAction("ConnectPayment", "ContentPartner");
            }

            //Eventually there may be need for multiple profiles per user... for now just default to one
            return View(contentPartner.Profiles.First());
        }

        [Authorize]
        public ActionResult Setup()
        {
            var contentPartner = ContentPartnerModel.Get(UserModel.GetUserId());

            if (!User.IsInRole("contentpartner"))
            {
                Roles.AddUsersToRoles(new[] { UserModel.GetUserName() }, new[] { "contentpartner" });
            }

            if (!contentPartner.Profiles.Any())
            {
                ViewBag.HasProfile = false;
                return View(new ProfileModel());
            }
            //else if (String.IsNullOrEmpty(contentPartner.Profiles.First().PaymentAccount))
            //{
            //    return RedirectToAction("ConnectPayment");
            //}
            else
            {
                ViewBag.HasProfile = true;
                return View(contentPartner.Profiles.First());
            }
        }

        [HttpPost, Authorize]
        public ActionResult Setup(ProfileModel model)
        {
            if (ProfileModel.Save(model, ProfileType.ContentPartner))
            {
                //return RedirectToAction("ConnectPayment");
            }

            ViewBag.ErrorMsg = "There is an error in your profile, please check your entries and try again.";
            return View(model);
        }

        [Authorize]
        public ActionResult ConnectPayment()
        {
            string state = Guid.NewGuid().ToString("N");  // Used to prevent CSRF attacks
            SaveState(state);

            string stripeClient = ConfigurationManager.AppSettings["Stripe:ClientKey"];

            return Redirect("https://connect.stripe.com/express/oauth/authorize?client_id=" + stripeClient + "&state=" + state);
            //try
            //{
            //	StripeConfiguration.ApiKey = "sk_test_51P9Zof00CMtKYIO8Q3SM5dnwVREpv5XvcLc3ci443z6ZguHqBlQ2SCNWoHfhxhwkKHANzbwdfM0wGjXU040h1Kql00klwQ1iGT";
            //	var connectedAccountId = ConfigurationManager.AppSettings["Stripe:ClientKey"];
            //	var service = new AccountLinkService();

            //	AccountLink accountLink = service.Create(
            //		new AccountLinkCreateOptions
            //		{
            //			Account = connectedAccountId,
            //			ReturnUrl = $"https://localhost:31394/return/{connectedAccountId}",
            //			RefreshUrl = $"https://localhost:31394/return/{connectedAccountId}",
            //			Type = "account_onboarding",
            //		}
            //	);

            //	return Redirect(accountLink.Url);
            //}
            //catch (Exception ex)
            //{
            //	Console.Write("An error occurred when calling the Stripe API to create an account link:  " + ex.Message);
            //	Response.StatusCode = 500;
            //	return RedirectToAction("Error");
            //}
        }

        public ActionResult Error()
        {
            return View();
        }

        [Authorize]
        public ActionResult OAuth()
        {
            string authorizationCode = Request.Params["code"];
            string state = Request.Params["state"];

            if (Request.Params["error"] != null || Request.Params["error_description"] != null)
            {
                TempData["MsgType"] = "error";
                TempData["Msg"] = "An error occurred while connecting your Stripe account.";
                return RedirectToAction("Error");
            }

            ValidateState(state);

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            RestClient client = new RestClient("https://connect.stripe.com/oauth/token");
            RestRequest restRequest = new RestRequest();
            restRequest.AddHeader("Cache-Control", "no-cache");
            restRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            restRequest.Method = Method.Post;

            restRequest.AddParameter("undefined", $"client_secret={ConfigurationManager.AppSettings["Stripe:ClientSecret"]}&code={authorizationCode}&grant_type=authorization_code", ParameterType.RequestBody);
            var response = client.Execute(restRequest);

            //var urlParameters = new Dictionary<string, string>
            //{
            //   { "client_secret", ConfigurationManager.AppSettings["Stripe:ClientSecret"] },
            //   { "code", authorizationCode },
            //   { "grant_type", "authorization_code" }
            //};

            //var encodedUrlContent = new FormUrlEncodedContent(urlParameters);

            //var response = Task.Run(() => client.PostAsync(, encodedUrlContent)).Result;

            //var responseString = Task.Run(() => response.Content.ReadAsStringAsync()).Result;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                TempData["MsgType"] = "error";
                TempData["Msg"] = "An error occurred while connecting your Stripe account.";
                return RedirectToAction("Error");
            }

            var json = JObject.Parse(response.Content);

            var user = Request.RequestContext.HttpContext.User;

            var authCookie = FormsAuthentication.GetAuthCookie(user.Identity.Name, true);

            var contentPartner = ContentPartnerModel.Get(UserModel.GetUserId(user.Identity.Name));

            ProfileModel profile = contentPartner.Profiles.First();

            profile.PaymentAccount = json["stripe_user_id"].ToString();
            profile.PaymentAccountRefreshToken = json["refresh_token"].ToString();

            ProfileModel.Update("ContentPartner", profile);

            if (!Roles.IsUserInRole("contentpartner"))
            {
                Roles.AddUsersToRoles(new[] { UserModel.GetUserName() }, new[] { "contentpartner" });
            }

            return RedirectToAction("Dashboard", "ContentPartner");
        }

        [HttpPost, Authorize(Roles = "contentpartner")]
        public ActionResult SaveProfile(string id, long profileId, ProfileModel model)
        {
            model.Id = profileId;

            if (id == "Profile" && ProfileModel.VanityCheck(model.Vanity, profileId))
            {
                TempData["MsgType"] = "error";
                TempData["Msg"] = "Entered vanity is already taken. Please enter a different vanity url.";
            }
            else if (ProfileModel.Update(id, model))
            {
                TempData["MsgType"] = "success";
                TempData["Msg"] = "Your profile was successfully updated.";
            }
            else
            {
                TempData["MsgType"] = "error";
                TempData["Msg"] = "Your profile could not be updated.";
            }

            var nav = Session["nav"];
            return RedirectToAction("Section", new { id, nav });
        }

        [HttpPost, Authorize(Roles = "contentpartner")]
        public ActionResult SaveSettings(HttpPostedFileBase file, long id, string type)
        {

            if (ProfileModel.SavePic(file, id, type))
            {
                TempData["MsgType"] = "success";
                TempData["Msg"] = "Your profile picture was successfully updated.";
            }
            else
            {
                TempData["MsgType"] = "error";
                TempData["Msg"] = "Your profile picture could not be saved.";
            }

            var nav = Session["nav"];
            return RedirectToAction("Section", new { id = "Profile", nav });
        }

        [HttpPost, Authorize(Roles = "contentpartner")]
        public ActionResult RemovePic(string id)
        {

            if (ProfileModel.RemovePic(id))
            {
                TempData["MsgType"] = "success";
                TempData["Msg"] = "Your profile picture was successfully removed.";
            }
            else
            {
                TempData["MsgType"] = "error";
                TempData["Msg"] = "Your profile picture could not be removed.";
            }

            var nav = Session["nav"];
            return RedirectToAction("Section", new { id = "Profile", nav });
        }

        #endregion

        #region Helpers

        private void SaveState(string state)
        {
            this.Response.Cookies.Set(new System.Web.HttpCookie("TempCookie", state) { Expires = DateTime.Now.AddMinutes(15) });
        }

        private bool ValidateState(string state)
        {
            //Retrieve state value from TempCookie
            string cookieState = this.Request.Cookies["TempCookie"].Value;

            if (cookieState == null)
            {
                throw new InvalidOperationException("No temp cookie");
            }

            if (state != cookieState)
            {
                ClearState();
                throw new InvalidOperationException("invalid state");
            }

            ClearState();
            return true;
        }

        private void ClearState()
        {
            var stateCookie = new System.Web.HttpCookie("TempCookie");
            stateCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(stateCookie);
        }

        #endregion

        public enum Menu
        {
            Overview,
            Edit,
            Preview,
            Personal,
            Company,
            Blank,
            Profile,
            Publications,
            Resources
        }
    }
}
