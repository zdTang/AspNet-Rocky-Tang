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

        public BrainTreeGate(IOptions<BrainTreeSettings> options)
        {
            _options = options.Value;
        }
        public IBraintreeGateway CreateGateway()
        {
            throw new NotImplementedException();
        }

        public IBraintreeGateway GetGateway()
        {
            throw new NotImplementedException();
        }
    }
}
