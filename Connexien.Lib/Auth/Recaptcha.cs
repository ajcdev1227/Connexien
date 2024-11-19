using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;

namespace Connexien.Lib.Auth
{
    public class ReCaptcha
    {

        public static bool IsValid(string recaptcha)
        {
            var secret = ConfigurationManager.AppSettings["ReCaptcha"];

            using (var client = new WebClient())
            {
                var reply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, recaptcha));
                var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

                return captchaResponse.Success;
            }
        }

        private abstract class CaptchaResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }
    }

}
