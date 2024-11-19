using Connexien.Lib.Models;
using Stripe;
using System.Collections.Generic;
using System.Configuration;

namespace Connexien.Lib.Payment
{
    public class PaymentManager
    {
        public PaymentResult ChargeCustomer(string stripeToken, string stripeEmail, string firstName,
                                            string lastName, string chargeDescription)
        {
            // Secret Key
            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["Stripe:ClientSecret"];

            int chargeAmount = Utilities.GetPublicationCost();

            var customers = new CustomerService();
            var charges = new ChargeService();

            Dictionary<string, string> CustomerMetaData = new Dictionary<string, string>();

            CustomerMetaData.Add("FirstName", firstName);
            CustomerMetaData.Add("LastName", lastName);

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Description = $"{firstName} {lastName}",
                Source = stripeToken,
                Metadata = CustomerMetaData
            });

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = chargeAmount,
                Description = chargeDescription,
                Currency = "usd",
                Customer = customer.Id,
                ReceiptEmail = stripeEmail
            });

            return new PaymentResult()
            {
                Id = charge.Id,
                Status = charge.Status,
                ChargeAmount = chargeAmount
            };
        }
    }
}
