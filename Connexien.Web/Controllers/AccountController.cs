using Connexien.Lib;
using Connexien.Web.Models;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Connexien.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        #region Login / Logout

        [AllowAnonymous]
        public ActionResult Login(int id = 0, string ReturnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("RedirectToDefault", "Account");
            }

            if (id == 1)
            {
                ViewBag.Alert = "Session Timed-out";
            }

            if (TempData["SuccessMsg"] != null)
            {
                ViewBag.SuccessMsg = TempData["SuccessMsg"].ToString();
                TempData["SuccessMsg"] = null;
            }

            var model = new LoginModel { ReturnUrl = ReturnUrl };
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public ActionResult Login(LoginModel model)
        {


            if (!UserModel.IsUserDeleted(model.Email) && Membership.ValidateUser(model.Email, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.Email, true);

                //SecurityModel.Log("login", "success", model.UserName);
                //LoginModel.SendLoginAlert(model.Email);

                if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
                {
                    return RedirectPermanent(model.ReturnUrl);
                }

                return RedirectToAction("RedirectToDefault", "Account");
            }

            //SecurityModel.Log("login", "fail", model.UserName);

            ViewBag.ErrorMsg = "Email or password is incorrect.";
            return View(model);
        }

        public ActionResult RedirectToDefault()
        {
            var roles = Roles.GetRolesForUser(UserModel.GetUserName());

            if (roles.Contains("setup"))
            {
                return RedirectToAction("Setup", "Account");
            }

            // if (roles.Contains("admin"))
            // {
            //     return RedirectToAction("Index", "Admin");
            // }

            if (roles.Contains("provider"))
            {
                return RedirectToAction("Dashboard", "Provider");
            }

            if (roles.Contains("contentpartner"))
            {
                return RedirectToAction("Dashboard", "ContentPartner");
            }

            if (roles.Contains("member"))
            {
                return RedirectToAction("Index", "Member");
            }

            return RedirectToAction("Logout", "Account");
        }

        public ActionResult Logout(int id = 0)
        {
            Response.Expires = 0;
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");

            #region Clear Authentication Cookies

            var cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "") { Expires = DateTime.Now.AddYears(-1) };
            Response.Cookies.Set(cookie1);

            var cookie2 = new HttpCookie("ASP.NET_SessionId", "") { Expires = DateTime.Now.AddYears(-1) };
            Response.Cookies.Set(cookie2);

            #endregion

            Session.Abandon();
            FormsAuthentication.SignOut();


            return RedirectToAction("Login", new { id });
        }

        #endregion

        #region Register

        [AllowAnonymous]
        public ActionResult Register(string id = "")
        {
            RegisterModel model = new RegisterModel();

            if (id == "contentpartner")
            {
                model.RegistrationType = "contentpartner";
            }

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("RedirectToDefault", "Account");
            }

            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public ActionResult Register(RegisterModel model)
        {
            MembershipCreateStatus status;

            Membership.CreateUser(UserModel.GetIp(), model.Password, model.Email, model.Firstname, model.Lastname, true, out status);

            if (status == MembershipCreateStatus.Success)
            {
                FormsAuthentication.SetAuthCookie(model.Email, true);

                switch (model.RegistrationType)
                {
                    case "contentpartner":
                        Roles.AddUsersToRoles(new[] { model.Email }, new[] { "contentpartner" });
                        return RedirectToAction("Setup", "ContentPartner");
                    default:
                        return RedirectToAction("Setup", "Account");
                }
            }

            ViewBag.ErrorMsg = status.ToString();

            return View(model);
        }

        public ActionResult Setup()
        {
            var roles = Roles.GetRolesForUser(UserModel.GetUserName());

            if (!roles.Contains("setup"))
            {
                return RedirectToAction("RedirectToDefault", "Account");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Setup(string id)
        {


            if (id != "provider" && id != "member" && id != "contentpartner")
            {
                ViewBag.ErrorMsg = "Invalid role.";
                return View();
            }

            Roles.AddUsersToRoles(new[] { UserModel.GetUserName() }, new[] { id });
            return RedirectToAction("RedirectToDefault", "Account");
        }

        #endregion

        public ActionResult Settings()
        {
            return View(RegisterModel.Get());
        }

        [HttpPost]
        public ActionResult Settings(RegisterModel model)
        {

            if (AccountModel.Update(model))
            {
                ViewBag.SuccessMsg = "Your account has been successfully updated.";
            }
            else
            {
                ViewBag.ErrorMsg = "Your account could not be updated.";
            }

            return View(RegisterModel.Get());
        }

        public ActionResult CloseAccount()
        {
            var success = AccountModel.CloseAccount();
            return RedirectToAction("Logout");
        }

        #region Forgot

        [AllowAnonymous]
        public ActionResult Forgot()
        {
            return View();
        }

        [HttpPost, AllowAnonymous]
        public ActionResult Forgot(ForgotPasswordModel model)
        {
            if (ModelState.IsValid && Membership.GetUserNameByEmail(model.Email) != String.Empty)
            {
                ForgotPasswordModel.SendReset(model);
            }

            TempData["SuccessMsg"] = "An email with further instructions has been sent if there was an account found.";
            return RedirectToAction("Login");
        }


        [AllowAnonymous]
        public ActionResult Reset(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Login");
            }

            PasswordResetModel passwordResetModel = PasswordResetModel.GetEmailFromToken(id);
            if (passwordResetModel == null)
            {
                ViewBag.ErrorMsg = "The reset link used is either expired or invalid, please try to login or reset your password again";
                return RedirectToAction("Login");
            }

            return View(passwordResetModel);
        }


        [HttpPost, AllowAnonymous]
        public ActionResult Reset(PasswordResetModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            MembershipUser membershipUser = Membership.GetUser(model.Email, false);

            bool isSuccessful = membershipUser.ChangePassword("oldPassword", model.Password);

            if (isSuccessful)
            {
                TempData["SuccessMsg"] = "Your password has successfully been reset.  Please login.";
                PasswordResetModel.DisposeOfToken(model.UserId);
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.ErrorMsg = "An error occurred whie changing your password. Please try again.";
                return View(model);
            }
        }

        #endregion

    }
}
