using Connexien.Lib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Connexien.Web.Models
{

    public class MemberModel
    {
        #region Properties

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<ProfileModel> Profiles { get; set; }

        #endregion

        public static MemberModel Get(long id)
        {
            var provider = new MemberModel();

            using (var db = new ConnexienEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.Id == id && x.Deleted == null);

                if (user == null)
                    return new MemberModel();


                provider.FirstName = user.FirstName;
                provider.LastName = user.LastName;
                provider.Email = user.Email;
                provider.Profiles = user.Profiles.Select(x => new ProfileModel(x)).ToList();
            }

            return provider;
        }

    }
}
