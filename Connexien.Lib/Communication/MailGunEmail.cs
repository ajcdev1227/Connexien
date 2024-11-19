using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Configuration;
using System.IO;

namespace Connexien.Lib.Communication
{
    public class MailGunEmail<T> : ICommunication<T> where T: IEmailParameters
    {
        private readonly string _apiKey;
        private readonly string _domainName;
        private readonly string _fromAddress;

        public MailGunEmail(string apiKey, string domainName, string fromAddress)
        {
            this._apiKey = apiKey;
            this._domainName = domainName;
            this._fromAddress = fromAddress;
        }

        public MailGunEmail()
        {
            this._apiKey = ConfigurationManager.AppSettings["MailGun:ApiKey"];
            this._domainName = ConfigurationManager.AppSettings["MailGun:DomainName"];
            this._fromAddress = ConfigurationManager.AppSettings["MailGun:FromAddress"];
        }

        public void SendMessage(T emailParameters)
        {
			var options = new RestClientOptions("https://api.mailgun.net/v3")
			{
				Authenticator = new HttpBasicAuthenticator("api", _apiKey)
			};

			RestClient client = new RestClient(options);
            RestRequest request = new RestRequest();
            
            request.AddParameter("domain", _domainName, ParameterType.UrlSegment);
            request.Resource = $"{_domainName}/messages";
            request.AddParameter("from", _fromAddress);
            request.AddParameter("to", emailParameters.EmailAddress);
            request.AddParameter("subject", emailParameters.Subject);
            request.AddParameter("text", emailParameters.BodyText);

            if (emailParameters.Attachments != null && emailParameters.Attachments.Count > 0)
            {
                foreach(var filePath in emailParameters.Attachments)
                {
					if (File.Exists(filePath))
					{
						request.AddFile("attachment", filePath);
					}
                }
            }

            request.Method = Method.Post;

            var response = client.Execute(request);
        }
    }
}
