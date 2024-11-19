using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using Connexien.Lib;
using Connexien.Lib.Communication;
using Connexien.Web.Providers;


namespace Connexien.Web.Models
{

    public class AccountModel
    {
        /*
        public static bool Setup(RegisterModel model)
        {
            try
            {
                var userId = UserModel.GetUserId();

                using (var db = new ConnexienEntities())
                {
                    var user = db.Users.FirstOrDefault(x => x.id == userId && x.Deleted == null);
                    if (user == null) return false;

                    if (db.Users.Any(x => x.UserName.ToLower() == model.UserName.ToLower()) || db.Users.Any(x => x.user_name.ToLower() == model.UserName.ToLower()))
                        return false;

                    if (string.IsNullOrWhiteSpace(model.Password) || model.UserName == model.Password || string.IsNullOrWhiteSpace(model.ConfirmPassword) || model.Password != model.ConfirmPassword || model.Password.Length < 6)
                        return false;

                    var p = UserModel.Encryption.EncryptText(model.Password);

                    if (user.User != null)
                    {
                        var u = user.User;
                        u.password = p;
                        db.SaveChanges();
                    }
                    else
                    {
                        user.UserName = model.UserName;
                        user.Password = p;
                        db.SaveChanges();
                    }

                    HttpContext.Current.Session["Setup"] = false;

                    #region Send Welcome Email

                    if (!string.IsNullOrWhiteSpace(user.Email))
                    {
                        var url = (HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://") + HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
                        var subject = VariableModel.Get("email_welcome_subject");
                        var body = VariableModel.Get("email_welcome_body");
                        body = body.Replace("*|url|*", url);

                        string msg;
                        var email = new EmailModel { ToAddress = user.Email, Subject = subject, Body = body };
                        email.Send(out msg);

                    }

                    #endregion

                    return true;
                    
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        */


        public static bool Update(RegisterModel model)
        {
            try
            {
                var userId = UserModel.GetUserId();

                using (var db = new ConnexienEntities())
                {
                    var user = db.Users.FirstOrDefault(x => x.Id == userId && x.Deleted == null);
                    if (user == null) return false;

                    if (string.IsNullOrWhiteSpace(model.Password) || model.Password == "nulled")
                    {

                        if (db.Users.Any(x => x.Id != user.Id && x.Deleted == null && x.Email.ToLower() == model.Email.ToLower())) //Duplicate Email
                            return false;

                        user.FirstName = !string.IsNullOrWhiteSpace(model.Firstname) ? model.Firstname : user.FirstName;
                        user.LastName = !string.IsNullOrWhiteSpace(model.Lastname) ? model.Lastname : user.LastName;
                        user.Email = !string.IsNullOrWhiteSpace(model.Email) ? model.Email : user.Email;

                        db.SaveChanges();

                        return true;
                    }

                    if (string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.ConfirmPassword) || model.Password != model.ConfirmPassword) // || model.Password.Length < 8)
                        return false;

                    if (Membership.ValidateUser(model.Email, model.OldPassword))
                    {
                        var p = ConnexienMembershipProvider.Encryption.EncryptText(model.Password);

                        user.Password = p;
                        db.SaveChanges();

                        return true;
                    }

                }

                return false;
            }
            catch (Exception ex)
            {
                var ip = UserModel.GetIp();
                Exceptions.Log(ex, ip, "UpdateAccountSettings");
                return false;
            }
        }

        public static bool CloseAccount()
        {
            var userId = UserModel.GetUserId();

            using (var db = new ConnexienEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.Id == userId);
                if (user == null) return false;

                user.Deleted = DateTime.UtcNow;
                db.SaveChanges();
                return true;
            }
        }
    }

    public class LoginModel
    {
        #region Properties

        [Required, Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        #endregion

        public static bool SendLoginAlert(string email)
        {
            try
            {
                //var subject = VariableModel.Get("email_login_subject");
                //var body = VariableModel.Get("email_login_body");

                //var email = new EmailModel { ToAddress = email, Subject = subject, Body = body };
                return true; //email.Send(out msg);
            }
            catch (Exception ex)
            {
                var ip = UserModel.GetIp();
                Exceptions.Log(ex, ip, "SendLoginAlert");
                return false;
            }
        }
    }

    public class RegisterModel
    {
        #region Properties

        public long Id { get; set; }

        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required.")]
        //[RegularExpression(@"(\w|\.)+@\w+\.(?:\w+\.)?\w{2,6}", ErrorMessage = "Please enter a valid email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string Lastname { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Your current password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }
        
        public string Ip { get; set; }
        public string Timezone { get; set; }

        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Google { get; set; }
        public string LinkedIn { get; set; }

        public string RegistrationType { get; set; }
        #endregion

        public RegisterModel()
        {
            var state = Guid.NewGuid().ToString();
            HttpContext.Current.Session["fbState"] = state;
            HttpContext.Current.Session["liState"] = state;

            //Facebook = Lib.Auth.Facebook.GetAuthUrl(state, "login", ConfigurationManager.AppSettings["FacebookRedirect"]);
            LinkedIn = Lib.Auth.LinkedIn.GetAuthUrl(state, "login", ConfigurationManager.AppSettings["LinkedInRedirect"]);
        }

        public static RegisterModel Get(long userId = 0)
        {
            userId = userId == 0 ? UserModel.GetUserId() : userId;
            var model = new RegisterModel();

            using (var db = new ConnexienEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.Id == userId && x.Deleted == null);
                if (user == null) return model;

                return new RegisterModel
                {
                    Id = user.Id,
                    Firstname = user.FirstName,
                    Lastname = user.LastName,
                    Email = user.Email,
                    Password = "nulled",
                    ConfirmPassword = "nulled",
                    OldPassword = "nulled"
                };
            }

        }

        public static bool SendWelcomeEmail(RegisterModel model, bool rep = false)
        {
            return false;
            /*
            try
            {
                var mandrill = new MandrillApi(ConfigurationManager.AppSettings["MandrillKey"]);
                var mandrillRecipients = new List<EmailAddress> { new EmailAddress { email = model.Email } };
                var msg = new EmailMessage
                {
                    to = mandrillRecipients,
                    from_email = ConfigurationManager.AppSettings["MandrillEmail"],
                    from_name = ConfigurationManager.AppSettings["MandrillName"],
                };
                msg.AddGlobalVariable("SUBJECT", "Welcome to Connexien!");
                msg.AddGlobalVariable("EMAIL", model.Email);
                //msg.AddGlobalVariable("LINK", "Http://www.Connexien.com/Account/Activate?key=" + model.MembershipId);
                mandrill.SendMessage(msg, rep ? "welcome-rep" : "welcome-user", null);

                return true;
            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, model.Ip, "Send Welcome Email: " + model.Email);
                return false;
            }
            */
        }

    }

    public class ForgotPasswordModel
    {
        [Required, Display(Name = "Email Address")]
        public string Email { get; set; }

        public static void SendReset(ForgotPasswordModel model)
        {
            try
            {
                using (var db = new ConnexienEntities())
                {
                    var user = db.Users.FirstOrDefault(x => x.Email.ToLower() == model.Email.ToLower());
                    string userValidationType = Enum.GetName(typeof(UserValidationType), 0);

                    var excessUserValidation = db.UserValidations.FirstOrDefault(x => x.UserId == user.Id
                                                    && x.Process == userValidationType);

                    if (excessUserValidation != null)
                    {
                        db.UserValidations.Remove(excessUserValidation);
                        db.SaveChanges();
                    }

                    UserValidation userValidation = new UserValidation()
                    {
                        Token = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        Created = DateTime.Now,
                        Process = Enum.GetName(typeof(UserValidationType), 0),
                        HoursValid = 2
                    };

                    db.UserValidations.Add(userValidation);
                    db.SaveChanges();

                    IEmailParameters emailParameters = new UserValidationEmailParameters(userValidation);
                    // TODO: Create Factory for communication
                    ICommunication<IEmailParameters> communication = new MailGunEmail<IEmailParameters>();

                    communication.SendMessage(emailParameters);
                }
            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, "SendReset");
            }
        }
    }

    public enum UserValidationType
    {
        PasswordReset,
        ConfirmEmail
    }

    public class PasswordResetModel
    {
        [Required]
        public long UserId { get; set; }

        [Required, Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Validate Password")]
        [Compare(nameof(Password), ErrorMessage = "Your password entry doesn't match")]
        public string ValidatePassword { get; set; }

        public static PasswordResetModel GetEmailFromToken(string token)
        {
            try
            {
                using (var db = new ConnexienEntities())
                {
                    PasswordResetModel passwordResetModel = null;

                    var validationToken = db.UserValidations.FirstOrDefault(x => x.Token == token);

                    if (validationToken == null)
                    {
                        return null;
                    }

                    passwordResetModel = new PasswordResetModel()
                    {
                        UserId = validationToken.UserId,
                        Email = validationToken.User.Email
                    };

                    return passwordResetModel;
                }
            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, "GetEmailFromToken");
                return null;
            }
        }

        public static void DisposeOfToken(long userId)
        {
            try
            {
                using (var db = new ConnexienEntities())
                {
                    string userValidationType = Enum.GetName(typeof(UserValidationType), 0);

                    var validationToken = db.UserValidations.FirstOrDefault(x => x.UserId == userId
                                                && x.Process == userValidationType);

                    if (validationToken != null)
                    {
                        db.UserValidations.Remove(validationToken);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, "DisposeOfToken");
            }
        }
    }

    /*
    public class RegisterModel
    {
        #region Properties

        [Required(ErrorMessageResourceName = "Firstnameisrequired", ErrorMessageResourceType = typeof(Resources.Account)), Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName="Lastnameisrequired",ErrorMessageResourceType = typeof(Resources.Account)), Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceName = "Dobrequired",ErrorMessageResourceType = typeof(Resources.Account)), Display(Name = "Date of Birth")]
        public DateTime Dob { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Account), ErrorMessageResourceName = "MemberIdIsRequired"), Display(Name = "Member ID")]
        public string MemberId { get; set; }

        [Required(ErrorMessage = "Last 4 digits of SSN is required."), Display(Name = "Last 4 digits of SSN")]
        [StringLength(4, ErrorMessage = "Please enter the last 4 digits.", MinimumLength = 4)]
        public string Last4Ssn { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Account), ErrorMessageResourceName = "EmailAddressIsRequired"), Display(Name = "Email Address")]
        public string Email { get; set; }

        //--------------------------------------------

        public string Ip { get; set; }
        public string Timezone { get; set; }
        
        //--------------------------------------------

        [Required(ErrorMessageResourceName = "Usernameisrequired", ErrorMessageResourceType = typeof(Resources.Account)), Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "Passwordisrequired", ErrorMessageResourceType = typeof(Resources.Account)), Display(Name = "Password")]
        [StringLength(100, ErrorMessageResourceName = "PasswordLengthError", ErrorMessageResourceType = typeof(Resources.Account), MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessageResourceName = "Passwordsdontmatch", ErrorMessageResourceType = typeof(Resources.Account))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessageResourceName = "Currentpasswordrequired", ErrorMessageResourceType = typeof(Resources.Account)), Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        #endregion

        public static bool CreateAccount(RegisterModel model, out string msg)
        {
            msg = "";

            try
            {
                using (var db = new ConnexienEntities())
                {
                    var dateFormat = VariableModel.Get("date_format_short");
                    var idRoute = VariableModel.Get("member_id_field");

                    DateTime tmpDate;
                    if (DateTime.TryParse(model.Dob.ToString(dateFormat), out tmpDate))
                        model.Dob = tmpDate;

                    var user = db.Insureds.FirstOrDefault(x =>
                                                          x.first_name.ToLower().Contains(model.FirstName.ToLower()) &&
                                                          x.last_name.ToLower().Contains(model.LastName.ToLower()) &&
                                                          x.dob == model.Dob &&
                                                          (idRoute == "ssn" ? x.ssn.ToLower().Contains(model.MemberId.ToLower()) : x.Eligibilities.Any(y => y.subscriber_id.ToLower().Contains(model.MemberId.ToLower()))) &&
                                                          (x.ssn.Contains(model.Last4Ssn) || x.Eligibilities.Any(y => y.subscriber_id.Contains(model.Last4Ssn))));
                    if (user == null) return false;
                    
                    var role = db.web_Roles.FirstOrDefault(x => x.Name.ToLower() == "member");
                    if (role == null) return false;

                    if (db.web_Users.Any(x => x.fk_Insured == user.pk_insured && x.Deleted == null))
                    {
                        msg = "Member already registered.";
                        return false;
                    }

                    
                    //if (db.web_Users.Any(x => x.Email == model.Email && x.Deleted == null))
                    //{
                    //    msg = "Email address already registered.";
                    //    return false;
                    //}
                    

                    db.web_Users.AddObject(new web_Users
                                               {
                                                   fk_Insured = user.pk_insured,
                                                   fk_Role = role.pk_web_Role,
                                                   Created = DateTime.UtcNow,
                                                   UserName = model.MemberId,
                                                   Email = model.Email,
                                                   Password = UserModel.Encryption.EncryptText(model.MemberId),
                                                   Last_Accessed = DateTime.UtcNow
                                               });
                    db.SaveChanges();

                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
}*/
    /*
    public static bool CreateMembership(RegisterModel model, bool rep, out MembershipCreateStatus createStatus)
    {
        model.Ip = AccountModel.GetIp();

        model.Password = rep ? model.Password : new Guid().ToString().Substring(0, 8);

        System.Web.Security.Membership.CreateUser(model.Email, model.Password, model.Email, null, null, true, null, out createStatus);

        if (createStatus == MembershipCreateStatus.Success)
        {
            var member = System.Web.Security.Membership.GetUser(model.Email);
            model.MembershipId = (Guid)member.ProviderUserKey;

            if (!CreateAccount(model, rep))
            {
                System.Web.Security.Membership.DeleteUser(model.Email);
                return false;
            }

            SendWelcomeEmail(model, rep);
            Roles.AddUserToRole(model.Email, rep ? "rep" : "user");

            FormsAuthentication.SetAuthCookie(model.Email, false);
            return true;
        }

        return false;
    }

    public static bool CreateAccount(RegisterModel model, bool rep = false)
    {

        using (var db = new ConnexienEntities())
        {
            var account = new tblAccount { Created = DateTime.UtcNow };
            db.tblAccounts.AddObject(account);
            db.SaveChanges();

            try
            {
                var textInfo = new CultureInfo("en-US", false).TextInfo;

                if (rep)
                {
                    var r = new tblRep
                    {
                        MembershipId = model.MembershipId,
                        Created = DateTime.UtcNow,
                        FirstName = textInfo.ToTitleCase(model.Firstname),
                        LastName = textInfo.ToTitleCase(model.Lastname),
                        Email = model.Email.ToLower(),
                        Timezone = model.Timezone,
                        Ip = model.Ip
                    };

                    db.tblReps.AddObject(r);
                    db.SaveChanges();

                    HttpContext.Current.Session["repId"] = r.Id;
                }
                else
                {
                    var u = new tblUser
                    {
                        AccountId = account.Id,
                        MembershipId = model.MembershipId,
                        Created = DateTime.UtcNow,
                        FirstName = textInfo.ToTitleCase(model.Firstname),
                        LastName = textInfo.ToTitleCase(model.Lastname),
                        Email = model.Email.ToLower(),
                        Timezone = model.Timezone,
                        Ip = model.Ip,
                        Key = model.Password
                    };
                    db.tblUsers.AddObject(u);
                    db.SaveChanges();

                    db.tblEmailPrefs.AddObject(new tblEmailPref { UserId = u.Id, Updated = DateTime.UtcNow });
                    db.tblPrivacys.AddObject(new tblPrivacy { UserId = u.Id, Updated = DateTime.UtcNow });
                    db.SaveChanges();

                    HttpContext.Current.Session["userId"] = u.Id;

                    if (HttpContext.Current.Session["linkBrief"] != null)
                    {
                        var l = (LinkBrief)HttpContext.Current.Session["linkBrief"];
                        var link = db.tblSearchLinks.FirstOrDefault(x => x.Id == l.LinkId);

                        if (link != null)
                        {

                            if (!db.tblSearchLinkResults.Any(x => x.SearchLinkId == l.LinkId && x.UserId == u.Id))
                            {
                                db.tblSearchLinkResults.AddObject(new tblSearchLinkResult
                                {
                                    SearchLinkId = l.LinkId,
                                    UserId = u.Id,
                                    AccountCreated = DateTime.UtcNow
                                });
                                db.SaveChanges();
                            }
                        }
                    }
                }


                return true;
            }
            catch (Exception ex)
            {
                if (account.Id != 0)
                {
                    db.DeleteObject(account);
                    db.SaveChanges();
                }

                Exceptions.Log(ex, model.Ip);
                return false;
            }
        }

    }

    public static bool SendWelcomeEmail(RegisterModel model, bool rep = false)
    {

        var mandrill = new MandrillApi(ConfigurationManager.AppSettings["MandrillKey"]);
        var mandrillRecipients = new List<EmailAddress> { new EmailAddress { email = model.Email } };
        var msg = new EmailMessage
        {
            to = mandrillRecipients,
            from_email = ConfigurationManager.AppSettings["MandrillEmail"],
            from_name = ConfigurationManager.AppSettings["MandrillName"],
        };
        msg.AddGlobalVariable("SUBJECT", "Welcome to Connexien!");
        msg.AddGlobalVariable("EMAIL", model.Email);
        msg.AddGlobalVariable("LINK", "Http://www.Connexien.com/Account/Activate?key=" + model.MembershipId);
        //mandrill.SendMessage(msg, rep ? "welcome-rep" : "welcome-user", null);

        return true;
    }*/


    /*
        public class ForgotModel
        {
            #region Properties

            public string Type { get; set; }

            [Required(ErrorMessage = "Member ID is required."), Display(Name = "Member ID")]
            public string MemberId { get; set; }

            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Tax ID is required."), Display(Name = "Tax ID")]
            public string TaxId { get; set; }

            [Required(ErrorMessage = "Email address is required."), Display(Name = "Email Address")]
            public string Email { get; set; }

            //--------------------------------------------

            public string Ip { get; set; }
            public string Timezone { get; set; }

            //--------------------------------------------

            [Required, Display(Name = "Password")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; }

            #endregion

            public static string GetUserNameFromKey(string key)
            {

                try
                {

                    using (var db = new ConnexienEntities())
                    {
                        var user = db.web_Users.FirstOrDefault(x => x.Key == key && x.Deleted == null);

                        return user == null ? "" : user.UserName;
                    }

                }
                catch (Exception ex)
                {
                    return "";
                }
            }

            public static bool SendReset(ForgotModel model, out string msg)
            {
                msg = "";

                try
                {
                    string url;
                    using (var db = new ConnexienEntities())
                    {
                        var users = db.web_Users.Where(x => x.Email == model.Email && x.Deleted == null);
                        var user = new web_Users();

                        if (!users.Any())
                        {
                            msg = "Account could not be found.";
                            return false;
                        }
                    
                        if (model.Type == "member")
                        {
                            var idRoute = VariableModel.Get("member_id_field");

                            user = users.FirstOrDefault(x => x.fk_Insured != null && 
                                                            (idRoute == "ssn" ? 
                                                                x.Insured.ssn.ToLower().Trim().Contains(model.MemberId.ToLower().Trim()) : 
                                                                x.Insured.Eligibilities.Any(y => y.subscriber_id.ToLower().Trim().Contains(model.MemberId.ToLower().Trim()))));

                            if (user == null)
                            {
                                msg = "Member record could not be found.";
                                return false;
                            }
                        }

                        if (model.Type == "provider")
                        {
                            user = users.FirstOrDefault(x => x.fk_Provider != null && x.Provider.providertaxid.ToLower() == model.TaxId.ToLower());

                            if (user == null)
                            {
                                msg = "Tax ID is incorrect.";
                                return false;
                            }
                        }

                        var key = Guid.NewGuid();
                        user.Key = key.ToString();
                        db.SaveChanges();

                        url = (HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://") + HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + HttpContext.Current.Request.ServerVariables["URL"] + "/" + key;
                        url = url.ToLower().Replace("/forgot", "/reset").Replace("member/", "").Replace("provider/", "").Replace("group/", "").Replace("admin/", "");
                    }

                    var subject = VariableModel.Get("email_password_reset_subject");
                    var body = VariableModel.Get("email_password_reset_body");
                    body = body.Replace("*|url|*", url);

                    var email = new EmailModel { ToAddress = model.Email, Subject = subject, Body = body };
                    return email.Send(out msg);
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    return false;
                }
            }

            public static bool PasswordReset(ForgotModel model, out string msg)
            {
                msg = "";
                string toAddress;

                try
                {

                    using (var db = new ConnexienEntities())
                    {
                        var user = db.web_Users.FirstOrDefault(x => x.Key == model.Type && x.Deleted == null);

                        if (user == null)
                        {
                            msg = "Account could not be found.";
                            return false;
                        }

                        toAddress = user.Email;

                        user.Password = UserModel.Encryption.EncryptText(model.Password);
                        user.Key = null;
                        db.SaveChanges();
                    }

                    var subject = VariableModel.Get("email_password_changed_subject");
                    var body = VariableModel.Get("email_password_changed_body");

                    var email = new EmailModel { ToAddress = toAddress, Subject = subject, Body = body };
                    return email.Send(out msg);

                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    return false;
                }
            }
        }
        */
}
