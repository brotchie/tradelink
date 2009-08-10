using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// instructions used to control offset amounts and distances.
    /// </summary>
    public class OffsetInfo
    {
        /// <summary>
        /// copy an existing offset to this one
        /// </summary>
        /// <param name="copy"></param>
        public OffsetInfo(OffsetInfo copy) : this(copy.ProfitDist, copy.StopDist, copy.ProfitPercent, copy.StopPercent, copy.NormalizeSize, copy.MinimumLotSize) { }
        /// <summary>
        /// create an offset instruction
        /// </summary>
        /// <param name="profitdist">in cents</param>
        /// <param name="stopdist">in cents</param>
        /// <param name="profitpercent">in percent (eg .1 = 10%)</param>
        /// <param name="stoppercent">in percent (eg .1 = 10%)</param>
        /// <param name="NormalizeSize">true or false</param>
        /// <param name="MinSize">minimum lot size when normalize size is true</param>
        public OffsetInfo(decimal profitdist, decimal stopdist, decimal profitpercent, decimal stoppercent, bool NormalizeSize, int MinSize)
        {
            ProfitDist = profitdist;
            StopDist = stopdist;
            ProfitPercent = profitpercent;
            StopPercent = stoppercent;
            this.NormalizeSize = NormalizeSize;
            MinimumLotSize = MinSize;
        }
        public bool NormalizeSize = false;
        public int MinimumLotSize = 1;
        public OffsetInfo(decimal profitdist, decimal stopdist) : this(profitdist, stopdist, 1, 1, false, 1) { }
        public OffsetInfo() : this(0, 0, 1, 1, false, 1) { }
        public uint ProfitId = 0;
        public uint StopId = 0;
        public decimal ProfitDist = 0;
        public decimal StopDist = 0;
        public decimal ProfitPercent = 1;
        public decimal StopPercent = 1;
        public int SentProfitSize = 0;
        public int SentStopSize = 0;
        public bool isOffsetCurrent(Position p)
        {
            Order l = Calc.PositionProfit(p, this);
            Order s = Calc.PositionStop(p, this);
            return (l.size == SentProfitSize) && (s.size == SentStopSize);
        }
        public bool hasProfit { get { return ProfitId != 0; } }
        public bool hasStop { get { return StopId != 0; } }
        public bool StopcancelPending = false;
        public bool ProfitcancelPending = false;
        public override string ToString()
        {
            return string.Format("p{0:n2}/{1:p0} s{2:n2}/{3:p0}", ProfitDist, ProfitPercent, StopDist, StopPercent);
        }

    }
}