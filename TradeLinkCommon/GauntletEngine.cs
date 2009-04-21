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
        HistSim _h;
        public HistSim Engine { get { return _h; } }
        public TradeLink.API.Response response { get { return _r; } }
        public GauntletEngine(TradeLink.API.Response r, TickFileFilter tff)
        {
            _r = r;
            _r.SendOrder += new OrderDelegate(_r_SendOrder);
            _r.SendCancel += new UIntDelegate(_r_SendCancel);
            _tff = tff;
            _h = new HistSim(_tff);
            _h.GotTick += new TickDelegate(_h_GotTick);
            _h.SimBroker.GotOrderCancel += new OrderCancelDelegate(SimBroker_GotOrderCancel);
            _h.SimBroker.GotOrder += new OrderDelegate(_r.GotOrder);
            _h.SimBroker.GotFill += new FillDelegate(_r.GotFill);

        }

        public void Go() { _h.PlayTo(HistSim.ENDSIM); }

        void SimBroker_GotOrderCancel(string sym, bool side, uint id)
        {
            _r.GotOrderCancel(id);
        }

        void SimBroker_GotOrder(Order o)
        {
            _r.GotOrder(o);
        }

        void _r_SendOrder(Order o)
        {
            _h.SimBroker.sendOrder(o);
        }

        void _r_SendCancel(uint number)
        {
            _h.SimBroker.CancelOrder(number);
        }

        void _h_GotTick(Tick t)
        {
            _r.GotTick(t);
        }


    }
}