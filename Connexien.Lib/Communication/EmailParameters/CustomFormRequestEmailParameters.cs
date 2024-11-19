using System.Collections.Generic;
using System.Text;

namespace Connexien.Lib.Communication
{
    public class CustomFormRequestEmailParameters : IEmailParameters
    {
        public string Subject { get; set; }

        public string EmailAddress { get; set; }

        public string BodyText { get; set; }

        public List<string> Attachments { get; set; }

        private readonly Profile _profile;


        public CustomFormRequestEmailParameters(Profile profile, List<string> attachments = null)
        {
            this._profile = profile;
            this.EmailAddress = GetEmailAddressForUser();
            this.Subject = CreateSubject();
            this.BodyText = CreateBodyText();
            this.Attachments = attachments;
        }

        private string GetEmailAddressForUser()
        {
            return _profile.Email;
        }

        private string CreateSubject()
        {
            return "Connexien Custom Form Program";
        }

        private string CreateBodyText()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Thank you for your submission! We’re currently reviewing your content and we’ll notify you when we post your work!");
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("Thanks again for helping us build a Community of Insight.");

            return sb.ToString();
        }
    }
}
