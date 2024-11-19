using Connexien.Lib;
using System.Linq;
using System.Web;

namespace Connexien.Web.Models
{
    using System;
    using System.Collections.Generic;

    public class UserModel
    {
        public static long GetUserId()
        {

            if (HttpContext.Current.Session["userId"] == null || (long)HttpContext.Current.Session["userId"] == 0)
            {
                HttpContext.Current.Session["userId"] = (long)0;

                using (var db = new ConnexienEntities())
                {
                    var email = HttpContext.Current.User.Identity.Name;

                    var user = db.Users.FirstOrDefault(x => x.Email == email && x.Deleted == null);
                    if (user == null)
                    {
                        return 0;
                    }

                    HttpContext.Current.Session["userId"] = user.Id;
                }
            }

            return (long)HttpContext.Current.Session["userId"];
        }

        public static long GetUserId(string email)
        {
            using (var db = new ConnexienEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.Email == email && x.Deleted == null);
                if (user == null)
                {
                    return 0;
                }

                return user.Id;
            }
        }

        public static string GetUserName()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        public static string GetIp()
        {

            if (HttpContext.Current.Session["ip"] == null || (string)HttpContext.Current.Session["ip"] == "")
            {
                HttpContext.Current.Session["ip"] = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return (string)HttpContext.Current.Session["ip"];
        }

        public static bool IsUserDeleted(string email)
        {
            using (var db = new ConnexienEntities())
            {
                return db.Users.Any(x => x.Email.ToLower() == email.ToLower() && x.Deleted != null);
            }
        }

        public static List<ProfileModel> GetProfiles()
        {
            using (var db = new ConnexienEntities())
            {
                long userId = GetUserId();
                var user = db.Users.FirstOrDefault(x => x.Id == userId && x.Deleted == null);

                if (user == null)
                {
                    return new List<ProfileModel>();
                }

                return user.Profiles.Select(x => new ProfileModel(x)).ToList();
            }
        }
    }
}
