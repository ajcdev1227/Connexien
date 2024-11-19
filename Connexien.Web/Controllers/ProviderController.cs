using Connexien.Web.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Connexien.Web.Controllers
{

	public class ProviderController : Controller
	{
		[Authorize(Roles = "provider")]
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard", "Provider");
		}

		[Authorize(Roles = "provider")]
		public ActionResult Dashboard()
		{
			ViewBag.Nav = (int)Menu.Overview;

			var provider = ProviderModel.Get(UserModel.GetUserId());

			if (!provider.Profiles.Any())
			{
				return RedirectToAction("Setup", "Provider");
			}

			//Eventually there may be need for multiple profiles per user... for now just default to one
			return View(provider.Profiles.First());
		}

		[Authorize(Roles = "provider")]
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

			var provider = ProviderModel.Get(UserModel.GetUserId());

			if (!provider.Profiles.Any())
			{
				return RedirectToAction("Setup", "Provider");
			}

			return View(id, provider.Profiles.First());
		}

		[Authorize(Roles = "provider")]
		public ActionResult Resources()
		{
			ViewBag.Nav = (int)Menu.Resources;

			if (!User.IsInRole("provider"))
			{
				return RedirectToAction("Setup", "Provider");
			}

			var provider = ProviderModel.Get(UserModel.GetUserId());

			//Eventually there may be need for multiple profiles per user... for now just default to one
			return View(provider.Profiles.First());
		}

		public ActionResult Setup()
		{
			var provider = ProviderModel.Get(UserModel.GetUserId());

			ProfileModel model = new ProfileModel();

			if (provider.Profiles.Any())
			{
				model = provider.Profiles.First();
			}

			ViewBag.HasProfile = false;
			return View(model);
		}

		[HttpPost]
		public ActionResult Setup(ProfileModel model)
		{

			if (ModelState.IsValid && ProfileModel.Save(model, ProfileType.Provider))
			{
				Roles.AddUsersToRoles(new[] { UserModel.GetUserName() }, new[] { "provider" });
				return RedirectToAction("Index");
			}

			ViewBag.ErrorMsg = "There is an error in your profile, please check your entries and try again.";
			return View(model);
		}

		[HttpPost, Authorize(Roles = "provider")]
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

		[HttpPost, Authorize(Roles = "provider")]
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

		[HttpPost, Authorize(Roles = "provider")]
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

		//---------------------------

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

		[HttpGet]
		public ActionResult ContentPartner()
		{
			return PartialView("Modals/_ContentPartner");
		}
	}
}
