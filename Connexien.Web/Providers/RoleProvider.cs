using System.Linq;
using System.Web.Security;
using Connexien.Lib;


namespace Connexien.Web.Providers
{

    public sealed class ConnexienRoleProvider : RoleProvider
    {
        public override string ApplicationName { get; set; }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (var db = new ConnexienEntities())
            {
                return db.Users.Any(x => x.Email.ToLower() == username.ToLower() && x.UserRoles.Any(y => y.Role.Name.ToLower() == roleName.ToLower()));
            }
        }

        public override string[] GetRolesForUser(string username)
        {

            using (var db = new ConnexienEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.Email == username);
                return user == null ? new[] { "" } : (!user.UserRoles.Any() ? new[] { "setup" } : user.UserRoles.Select(x => x.Role.Name.ToLower()).ToArray());
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new System.NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (var db = new ConnexienEntities())
            {
                foreach(var username in usernames)
                {
                    var roles = db.Roles.ToDictionary(x => x.Name.ToLower(), x => x.Id);
                    var user = db.Users.FirstOrDefault(x => x.Email.ToLower() == username.ToLower());
                    if (user == null) continue;

                    foreach (var roleId in from roleName in roleNames where roles.ContainsKey(roleName.ToLower()) select roles[roleName.ToLower()])
                    {
                        user.UserRoles.Add(new UserRole
                                               {
                                                   RoleId = roleId
                                               });
                    }

                    db.SaveChanges();

                }
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new System.NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new System.NotImplementedException();
        }

    }
}
