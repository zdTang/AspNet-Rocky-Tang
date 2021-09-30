using Braintree;

namespace Rocky_Utility.BrainTree
{
    interface IBrainTreeGate
    {

        IBraintreeGateway CreateGateway();
        IBraintreeGateway GetGateway();
    }
}
