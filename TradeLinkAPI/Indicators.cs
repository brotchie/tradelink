

namespace TradeLink.API
{
    public interface GotResetIndicator
    {
        void Reset();
    }
    public interface GotTickIndicator
    {
        void GotTick(Tick k);
    }

    public interface GotOrderIndicator
    {
        void GotOrder(Order o);
    }

    public interface GotCancelIndicator
    {
        void GotCancel(long id);
    }

    public interface GotFillIndicator
    {
        void GotFill(Trade f);
    }

    public interface GotPositionIndicator
    {
        void GotPosition(Position p);
    }

    public interface GotMessageIndicator
    {
        void GotMessage(MessageTypes type, long source, long dest, long msgid, string request, ref string response);
    }

    public interface SendBasketIndicator
    {
        event BasketDelegate SendBasketEvent;
    }

    public interface SendOrderIndicator
    {
        event OrderDelegate SendOrderEvent;
    }

    public interface SendCancelIndicator
    {
        event LongDelegate SendCancelEvent;
    }

    public interface SendMessage
    {
        event MessageDelegate SendMessageEvent;
    }

    

}
