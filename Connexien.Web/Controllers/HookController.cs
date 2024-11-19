using Connexien.Lib;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Connexien.Web.Controllers
{
    public class HookController : Controller
    {
        [HttpPost]
        public async Task<HttpStatusCodeResult> OrderPayment()
        {
            try
            {
                var requestHeader = Request["HTTP_X_SHOPIFY_HMAC_SHA256"];
                var message = await new StreamReader(Request.InputStream).ReadToEndAsync();

                Exceptions.Info($"OrderPayment : Message from Shopify :: {message}");

                if (VerifyWebhook(ConfigurationManager.AppSettings["Shopify:SharedKey"], requestHeader, message))
                {
                    var connexienStorageQueue = new ConnexienStorageQueue();

                    await connexienStorageQueue.AddMessageAsync("orderpayment", message);

                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                Exceptions.Log(ex, null, "OrderPayment : Error during processing");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        #region Shopify

        public bool VerifyWebhook(string sharedSecretKey, string requestHeader, string message)
        {
            Encoding encoding = Encoding.UTF8;

            var hmac = new HMACSHA256(encoding.GetBytes(sharedSecretKey));
            var hash = hmac.ComputeHash(encoding.GetBytes(message));

            string computedHashString = Convert.ToBase64String(hash);

            return computedHashString == requestHeader;
        }

        private string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2"); /* hex format */
            return sbinary;
        }

        #endregion
    }
}
