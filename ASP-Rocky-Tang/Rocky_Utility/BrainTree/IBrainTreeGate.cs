using Braintree;

namespace Rocky_Utility.BrainTree
{
    public interface IBrainTreeGate
    {
        // Need add package "Braintree"
        IBraintreeGateway CreateGateway();
        IBraintreeGateway GetGateway();
    }
}
