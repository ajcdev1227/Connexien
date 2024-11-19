using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Connexien.Web.Models;

namespace Connexien.Web.Controllers
{
    [Authorize(Roles = "member")]
    public class MemberController : Controller
    {

        public ActionResult Index()
        {
            return RedirectToAction("Dashboard", "Member");
        }

        public ActionResult Dashboard()
        {
            ViewBag.Nav = (int) Menu.Overview;

            var member = MemberModel.Get(UserModel.GetUserId());

            if (!member.Profiles.Any())
                return RedirectToAction("Setup", "Member");

            //Eventually there may be need for multiple profiles per user... for now just default to one
            return View(member.Profiles.First());
        }

        public ActionResult Section(string id)
        {
            ViewBag.Nav = (int) Enum.Parse(typeof(Menu), id);

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

            var member = MemberModel.Get(UserModel.GetUserId());

            if (!member.Profiles.Any())
                return RedirectToAction("Setup", "Member");

            return View(id, member.Profiles.First());
        }

        public ActionResult Publications()
        {
            ViewBag.Nav = (int) Menu.Publications;

            var member = MemberModel.Get(UserModel.GetUserId());

            if (!member.Profiles.Any())
                return RedirectToAction("Setup", "Member");

            //Eventually there may be need for multiple profiles per user... for now just default to one
            return View(member.Profiles.First());
        }

        public ActionResult Setup()
        {
            var member = MemberModel.Get(UserModel.GetUserId());

            if (member.Profiles.Any())
                return RedirectToAction("Dashboard", "Member");

            ViewBag.HasProfile = false;
            return View(new ProfileModel());
        }

        [HttpPost]
        public ActionResult Setup(ProfileModel model)
        {

            if (ProfileModel.Save(model, ProfileType.Member))
            {
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMsg = "There is an error in your profile, please check your entries and try again.";
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveProfile(string id, long profileId, ProfileModel model)
        {
            model.Id = profileId;
            if (ProfileModel.Update(id, model))
            {
                TempData["MsgType"] = "success";
                TempData["Msg"] = "Your profile was successfully updated.";
            }
            else
            {
                TempData["MsgType"] = "error";
                TempData["Msg"] = "Your profile could not be updated.";
            }

            return RedirectToAction("Section", new { id });
        }

        [HttpPost]
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

            return RedirectToAction("Section", new { id = "Profile" });
        }

        [HttpPost]
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

            return RedirectToAction("Section", new { id = "Profile" });
        }

        //---------------------------

        public enum Menu
        {
            Overview,
            Publications,
            Edit,
            Personal,
            Company,
            Blank,
            Profile
        }

    }
}
