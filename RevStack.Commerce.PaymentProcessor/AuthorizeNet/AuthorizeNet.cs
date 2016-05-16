using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevStack.Payment;

namespace RevStack.Commerce.PaymentProcessor
{
    public class AuthorizeNet : IPaymentProvider

    {
       public bool Approved { get; set; }

       public AuthorizeNet()
       {
            Approved = false;
       }

       public IEnumerable<KeyValueItem> Charge(IAddress billingAddress,IAddress shippingAddress,Dictionary<string,string> fields,decimal total)
        {
            var responseFields = new List<KeyValueItem>();
            var gatewayInfo = new GatewayInfo();
            gatewayInfo.GatewayType = GatewayType.AuthorizeDotNet;
            ServiceMode serviceMode = fields["Mode"] == "live" ? ServiceMode.Live : ServiceMode.Test;

            var auth = new GatewayAuth();
            auth.Username = fields["ApiLogin"];
            auth.Password = fields["ApiKey"];

            string currency = "USD";
            if (fields.ContainsKey("Currency"))
                currency = fields["Currency"];

            var gateway = new Gateway(gatewayInfo.GatewayType, auth, serviceMode);
            var request = gateway.CreateRequest();
            request.AddCurrency(currency);
            request.AddCustomer("", billingAddress.FirstName, billingAddress.LastName, billingAddress.Street + " " + billingAddress.Street2, billingAddress.City, billingAddress.State, billingAddress.ZipCode, billingAddress.PhoneNumber, billingAddress.Email, "");
            request.AddShipping(shippingAddress.FirstName, shippingAddress.LastName, shippingAddress.Street + " " + shippingAddress.Street2, shippingAddress.City, shippingAddress.State, shippingAddress.ZipCode, shippingAddress.PhoneNumber, shippingAddress.Email, "");
            request.Sale(fields["CardNumber"], fields["CardExpirationMonth"] + fields["CardExpirationYear"], fields["CvvNumber"], total);

            var response = gateway.Send(request);
             
            if (response.Approved)
            {
                responseFields.Add(new KeyValueItem
                {
                    Key = "AuthorizationCode",
                    Value = response.AuthorizationCode
                });
                responseFields.Add(new KeyValueItem
                {
                    Key = "TransactionId",
                    Value = response.TransactionId
                });
                Approved = true;
            }
            else
            {
                responseFields.Add(new KeyValueItem
                {
                    Key = "Message",
                    Value = response.Message
                });
            }

            return responseFields;
        }

        public Task<IEnumerable<KeyValueItem>> ChargeAsync(IAddress billingAddress, IAddress shippingAddress, Dictionary<string, string> fields, decimal total)
        {
            return Task.FromResult(Charge(billingAddress, shippingAddress, fields, total));
        }
    }
}