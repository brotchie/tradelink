using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// lightweight backtesting class that includes binding of data and simulation components.
    /// </summary>
    public class GauntletEngine
    {
        TradeLink.API.Response _r;
        TickFileFilter _tff;
        MultiSimImpl _h;
        public MultiSimImpl Engine { get { return _h; } }
        public TradeLink.API.Response response { get { return _r; } }
        Broker SimBroker = new Broker();
        public GauntletEngine(TradeLink.API.Response r, TickFileFilter tff)
        {
            _r = r;
            _r.SendOrderEvent += new OrderSourceDelegate(_r_SendOrder);
            _r.SendCancelEvent += new LongSourceDelegate(_r_SendCancel);
            _tff = tff;
            _h = new MultiSimImpl(_tff);
            _h.GotTick += new TickDelegate(_h_GotTick);
            SimBroker.GotOrderCancel += new OrderCancelDelegate(SimBroker_GotOrderCancel);
            SimBroker.GotOrder += new OrderDelegate(_r.GotOrder);
            SimBroker.GotFill += new FillDelegate(_r.GotFill);

        }

        public void Go() { _h.PlayTo(MultiSimImpl.ENDSIM); }

        void SimBroker_GotOrderCancel(string sym, bool side, long id)
        {
            _r.GotOrderCancel(id);
        }

        void SimBroker_GotOrder(Order o)
        {
            _r.GotOrder(o);
        }

        void _r_SendOrder(Order o, int id)
        {
            SimBroker.SendOrderStatus(o);
        }

        void _r_SendCancel(long number, int id)
        {
            SimBroker.CancelOrder(number);
        }

        void _h_GotTick(Tick t)
        {
            SimBroker.Execute(t);
            _r.GotTick(t);
        }


    }
}