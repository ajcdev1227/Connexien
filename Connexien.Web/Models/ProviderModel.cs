using System;
using System.Collections.Generic;
using System.Linq;
using Connexien.Lib;

namespace Connexien.Web.Models
{

    public class ProviderModel
    {
        #region Properties

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<ProfileModel> Profiles { get; set; } 

        #endregion

        public static ProviderModel Get(long id)
        {
            var provider = new ProviderModel();

            using(var db = new ConnexienEntities())
            {

                var user = db.Users.FirstOrDefault(x => x.Id == id && x.Deleted == null);

                if (user == null)
                    return new ProviderModel();


                provider.FirstName = user.FirstName;
                provider.LastName = user.LastName;
                provider.Email = user.Email;
                provider.Profiles = user.Profiles.Select(x => new ProfileModel(x)).ToList();
            }

            return provider;
        }

        public static ProviderModel Get(string id)
        {
            var provider = new ProviderModel();

            using (var db = new ConnexienEntities())
            {
                long tmpId;
                var profile = Int64.TryParse(id, out tmpId) ? db.Profiles.FirstOrDefault(x => x.Id == tmpId && x.Deleted == null) : db.Profiles.FirstOrDefault(x => (x.Vanity == id || x.Guid == id) && x.Deleted == null);

                if (profile == null)
                    return new ProviderModel();


                provider.FirstName = profile.User.FirstName;
                provider.LastName = profile.User.LastName;
                provider.Email = profile.User.Email;
                provider.Profiles = new List<ProfileModel>{new ProfileModel(profile)};
            }

            return provider;
        }

    }
}
