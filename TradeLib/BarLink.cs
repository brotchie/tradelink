using System;


namespace TradeLib
{
    /// <summary>
    /// A class that describes successive bars in a barlist, usually the most recent two bars.
    /// </summary>
    public static class BarLink
    {
        /// <summary>
        /// Determines whether the last two bars have the same high.
        /// </summary>
        /// <param name="b">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the most recent bars have the same high; otherwise, <c>false</c>.
        /// </returns>
        public static bool isH(BarList b) { int l = b.BarZero - 1; return (l > 0) && (b.Get(l).High == b.Get(l - 1).High); }
        /// <summary>
        /// Determines whether the most recent two barlists share the same low.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is L; otherwise, <c>false</c>.
        /// </returns>
        public static bool isL(BarList barlist) { int l = barlist.BarZero - 1; return (l > 0) && (barlist.Get(l).Low == barlist.Get(l - 1).Low); }
        /// <summary>
        /// Determines whether the specified barlist is a HigherHigh (HH).
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is HH; otherwise, <c>false</c>.
        /// </returns>
        public static bool isHH(BarList barlist) { int l = barlist.BarZero; return (l > 1) && (barlist.Get(l - 1).High > barlist.Get(l - 2).High); }
        /// <summary>
        /// Determines whether the specified barlist is HigherLow.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is HL; otherwise, <c>false</c>.
        /// </returns>
        public static bool isHL(BarList barlist) { int l = barlist.BarZero; return (l > 1) && (barlist.Get(l - 1).Low > barlist.Get(l - 2).Low); }
        /// <summary>
        /// Determines whether the specified barlist is LowerLow.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is LL; otherwise, <c>false</c>.
        /// </returns>
        public static bool isLL(BarList barlist) { int l = barlist.BarZero; return (l > 1) && (barlist.Get(l - 1).Low < barlist.Get(l - 2).Low); }
        /// <summary>
        /// Determines whether the specified barlist is LowerHigh.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is LH; otherwise, <c>false</c>.
        /// </returns>
        public static bool isLH(BarList barlist) { int l = barlist.BarZero; return (l > 1) && (barlist.Get(l - 1).High < barlist.Get(l - 2).High); }
        /// <summary>
        /// Determines whether the specified barlist is bullish, or HigherHigh+HigherLow.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is bull; otherwise, <c>false</c>.
        /// </returns>
        public static bool isBull(BarList barlist) { return isHH(barlist) && isHL(barlist); }
        /// <summary>
        /// Determines whether the specified barlist is bearish, bearish = LL + LH.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is bear; otherwise, <c>false</c>.
        /// </returns>
        public static bool isBear(BarList barlist) { return isLL(barlist) && isLH(barlist); }
        /// <summary>
        /// Determines whether the specified barlist is a reversal.  Reversals are HH + LL.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is rev; otherwise, <c>false</c>.
        /// </returns>
        public static bool isRev(BarList barlist) { return isLL(barlist) && isHH(barlist); }
        /// <summary>
        /// Determines whether the specified barlist is NoChange.  Highs and lows are the same.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is NC; otherwise, <c>false</c>.
        /// </returns>
        public static bool isNC(BarList barlist) { return isH(barlist) && isL(barlist); }
        /// <summary>
        /// Determines whether the specified barlist is HighChange, only the high changes.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is HC; otherwise, <c>false</c>.
        /// </returns>
        public static bool isHC(BarList barlist) { return isHH(barlist) && isL(barlist); }
        /// <summary>
        /// Determines whether the specified barlist is LowChange. Only the low change.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is LC; otherwise, <c>false</c>.
        /// </returns>
        public static bool isLC(BarList barlist) { return isLL(barlist) && isH(barlist); }
        /// <summary>
        /// Determines whether the specified barlist is NotAny, the high is lower and the low is higher.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is NA; otherwise, <c>false</c>.
        /// </returns>
        public static bool isNA(BarList barlist) { return isLH(barlist) && isHL(barlist); }
        /// <summary>
        /// Determines whether the specified barlist is NotHigher, more recent bar shares same high but lower low.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is NH; otherwise, <c>false</c>.
        /// </returns>
        public static bool isNH(BarList barlist) { return isH(barlist) && isHL(barlist); }
        /// <summary>
        /// Determines whether the specified barlist is NotLower, more recent bar shares same low but higher high.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        /// <returns>
        /// 	<c>true</c> if the specified barlist is NL; otherwise, <c>false</c>.
        /// </returns>
        public static bool isNL(BarList barlist) { return isL(barlist) && isLH(barlist); }
        public static string ToString(BarList b)
        {
            return (isH(b) ? "H," : "") + (isHH(b) ? "HH," : "") + (isHL(b) ? "HL," : "") + (isL(b) ? "L," : "") + (isLL(b) ? "LL," : "") + (isLH(b) ? "LH," : "") + " -> " + (isBull(b) ? "BU," : "") + (isBear(b) ? "BE," : "") + (isRev(b) ? "RV," : "") + (isHC(b) ? "HC" : "") + (isLC(b) ? "LC" : "") + (isNC(b) ? "NC" : "") + (isNA(b) ? "NA" : "") + (isNH(b) ? "NH" : "") + (isNL(b) ? "NL" : "");
        }
    }

}
