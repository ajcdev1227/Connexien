using System.Collections.Generic;
using System.Configuration;

namespace Connexien.Lib.Communication.EmailParameters
{
    public class ConnexienBackEndEmailParameters : IEmailParameters
    {
        public List<string> Attachments { get; set; }
        public string BodyText { get; set; }
        public string EmailAddress { get; set; }
        public string Subject { get; set; }

        public ConnexienBackEndEmailParameters(string bodyText, string emailAddress, string subject, List<string> attachments = null)
        {
            this.Attachments = attachments;
            this.BodyText = bodyText;
            this.EmailAddress = emailAddress;
            this.Subject = subject;
        }

        public ConnexienBackEndEmailParameters(string bodyText, string subject, List<string> attachments = null)
        {
            this.Attachments = attachments;
            this.BodyText = bodyText;
            this.EmailAddress = ConfigurationManager.AppSettings["Connexien:EmailAddress"];
            this.Subject = subject;
        }
    }
}
