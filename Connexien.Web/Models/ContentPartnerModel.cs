using Connexien.Lib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Connexien.Web.Models
{
    public class ContentPartnerModel
    {
        #region Properties

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<ProfileModel> Profiles { get; set; }

        #endregion

        public static ContentPartnerModel Get(long id)
        {
            var contentPartner = new ContentPartnerModel();

            using (var db = new ConnexienEntities())
            {

                var user = db.Users.FirstOrDefault(x => x.Id == id && x.Deleted == null);

                if (user == null)
                {
                    return new ContentPartnerModel();
                }

                contentPartner.FirstName = user.FirstName;
                contentPartner.LastName = user.LastName;
                contentPartner.Email = user.Email;
                contentPartner.Profiles = user.Profiles.Select(x => new ProfileModel(x)).ToList();
            }

            return contentPartner;
        }

        public static ContentPartnerModel Get(string id)
        {
            var contentPartner = new ContentPartnerModel();

            using (var db = new ConnexienEntities())
            {
                long tmpId;
                var profile = Int64.TryParse(id, out tmpId) ? db.Profiles.FirstOrDefault(x => x.Id == tmpId && x.Deleted == null) : db.Profiles.FirstOrDefault(x => (x.Vanity == id || x.Guid == id) && x.Deleted == null);

                if (profile == null)
                {
                    return new ContentPartnerModel();
                }

                contentPartner.FirstName = profile.User.FirstName;
                contentPartner.LastName = profile.User.LastName;
                contentPartner.Email = profile.User.Email;
                contentPartner.Profiles = new List<ProfileModel> { new ProfileModel(profile) };
            }

            return contentPartner;
        }
    }
}