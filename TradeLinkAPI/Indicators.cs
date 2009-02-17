

namespace TradeLink.API
{
    public interface TickIndicator
    {
        bool newTick(Tick t);
    }

    public interface BarListIndicator
    {
        bool newBar(BarList bl);
    }
}
