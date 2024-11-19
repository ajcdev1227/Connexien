using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Connexien.Web.Controllers
{
    using Connexien.Web.Models;

    public class CustomFormsController : Controller
    {
        [HttpGet]
        public ActionResult Description()
        {
            return this.View();
        }

        [HttpGet, Authorize(Roles = "contentpartner, provider, member")]
        public ActionResult SubmitRequest()
        {
            var profile = UserModel.GetProfiles().FirstOrDefault();
            return this.PartialView("_CustomFormRequestCreate", new CustomFormRequestModel() { ProfileGuid = profile.ProfileGuid, ProfileId = profile.Id });
        }

        [HttpPost, Authorize(Roles = "contentpartner, provider, member")]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRequest(CustomFormRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));

                var concatMessage = $"Custom form request could not be sent due to the following reasons: {messages}";

                TempData["Msg"] = concatMessage;
                TempData["MsgType"] = "error";
                ViewBag.ErrorMsg = TempData["Msg"];
                return Json(new { statusText = concatMessage });
            }

            if (CustomFormRequestModel.Submit(model))
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
    }
}