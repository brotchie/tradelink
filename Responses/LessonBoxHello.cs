using TradeLink.API;
using TradeLink.Common;

namespace Responses
{
    public class HelloWorld : ResponseTemplate
    {
        public override void GotTick(Tick tick)
        {
            senddebug("Hello World");
        }
    }

    public class HelloWorldExtraCredit : ResponseTemplate
    {
        public override void GotTick(Tick tick)
        {
            if (!isValid) return;
            senddebug("Hello World");
            isValid = false;
        }
    }
}
