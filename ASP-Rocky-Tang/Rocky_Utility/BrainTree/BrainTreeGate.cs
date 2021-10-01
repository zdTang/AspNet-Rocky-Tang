using Braintree;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_Utility.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {
        public BrainTreeSettings _options { set; get; }
        public IBraintreeGateway braintreeGateway { get; set; }


        /// <summary>
        /// Work with Dependency Injection 
        /// services.Configure<BrainTreeSettings>(Configuration.GetSection("BrainTree"));
        /// </summary>
        /// <param name="options"></param>
        public BrainTreeGate(IOptions<BrainTreeSettings> options)
        {
            _options = options.Value;
        }
        public IBraintreeGateway CreateGateway()
        {
            // Create an instance of BraintreeGateway !!!
            return new BraintreeGateway(_options.Environment,_options.MerchantId,_options.PublicKey,_options.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if (braintreeGateway == null)
            {
                braintreeGateway = CreateGateway();
            }
            return braintreeGateway;
        }
    }
}
