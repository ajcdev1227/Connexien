using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Connexien.Lib.Communication
{
    public class UserValidationEmailParameters : IEmailParameters
    {
        public string Subject { get; set; }

        public string EmailAddress { get; set; }

        public string BodyText { get; set; }

        public List<string> Attachments { get; set; }

        private readonly UserValidation _userValidation;


        public UserValidationEmailParameters(UserValidation userValidation, List<string> attachments = null)
        {
            this._userValidation = userValidation;
            this.EmailAddress = GetEmailAddressForUser();
            this.Subject = CreateSubject();
            this.BodyText = CreateBodyText();
            this.Attachments = attachments;
        }

        private string GetEmailAddressForUser()
        {
            using (var db = new ConnexienEntities())
            {
                return db.Users.Where(x => x.Id == _userValidation.UserId).Select(x => x.Email).FirstOrDefault();
            }
        }

        private string CreateSubject()
        {
            return "Connexien Password Reset";
        }

        private string CreateBodyText()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("We have received a request to reset the password for your login: " + EmailAddress);
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("You can reset your password by clicking the link below: ");
            sb.AppendLine();
            sb.Append("https://app.connexien.com/account/reset/" + _userValidation.Token);
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("Your password reset link is valid for one use only and expires in " + _userValidation.HoursValid + " hours.");
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("If you do not wish to reset your password or did not submit this request, " +
                      "simply disregard this message and nothing will be changed.");

            return sb.ToString();
        }

    }
}
