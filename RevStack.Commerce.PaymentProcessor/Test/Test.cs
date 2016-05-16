using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using RevStack.Mvc;

namespace RevStack.Commerce.PaymentProcessor
{
    public class Test : IPaymentProvider
    {
        private bool _approve
        {
            get
            {
                string config = ConfigurationManager.AppSettings["RevStack.Commerce.PaymentProcessor.Test.Approve"];
                if (string.IsNullOrEmpty(config)) return true;
                else
                {
                    return Convert.ToBoolean(config);
                }
            }
        }
        public bool Approved { get; set; }
       
        public Test()
        {
            Approved = false;
        }

        public IEnumerable<KeyValueItem> Charge(IAddress billingAddress, IAddress shippingAddress, Dictionary<string, string> fields, decimal total)
        {
            Approved = _approve;
            var responseFields = new List<KeyValueItem>();
            if(Approved)
            {
                responseFields.Add(new KeyValueItem
                {
                    Key = "AuthorizationCode",
                    Value = Utils.GenerateRandomString(6)
                });
                responseFields.Add(new KeyValueItem
                {
                    Key = "TransactionId",
                    Value = Utils.GenerateRandomString(8)
                });
            }
            else
            {
                responseFields.Add(new KeyValueItem
                {
                    Key = "Message",
                    Value = "Test Transaction mode was set to decline"
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