using System;


namespace TradeLib
{
    public static class BarLink
    {
        public static bool isH(BarList b) { int l = b.BarZero - 1; return (l > 0) && (b.Get(l).High == b.Get(l - 1).High); }
        public static bool isL(BarList b) { int l = b.BarZero - 1; return (l > 0) && (b.Get(l).Low == b.Get(l - 1).Low); }
        public static bool isHH(BarList b) { int l = b.BarZero; return (l > 1) && (b.Get(l - 1).High > b.Get(l - 2).High); }
        public static bool isHL(BarList b) { int l = b.BarZero; return (l > 1) && (b.Get(l - 1).Low > b.Get(l - 2).Low); }
        public static bool isLL(BarList b) { int l = b.BarZero; return (l > 1) && (b.Get(l - 1).Low < b.Get(l - 2).Low); }
        public static bool isLH(BarList b) { int l = b.BarZero; return (l > 1) && (b.Get(l - 1).High < b.Get(l - 2).High); }
        public static bool isBull(BarList b) { return isHH(b) && isHL(b); }
        public static bool isBear(BarList b) { return isLL(b) && isLH(b); }
        public static bool isRev(BarList b) { return isLL(b) && isHH(b); }
        public static bool isNC(BarList b) { return isH(b) && isL(b); }
        public static bool isHC(BarList b) { return isHH(b) && isL(b); }
        public static bool isLC(BarList b) { return isLL(b) && isH(b); }
        public static bool isNA(BarList b) { return isLH(b) && isHL(b); }
        public static bool isNH(BarList b) { return isH(b) && isHL(b); }
        public static bool isNL(BarList b) { return isL(b) && isLH(b); }
        public static string ToString(BarList b)
        {
            return (isH(b) ? "H," : "") + (isHH(b) ? "HH," : "") + (isHL(b) ? "HL," : "") + (isL(b) ? "L," : "") + (isLL(b) ? "LL," : "") + (isLH(b) ? "LH," : "") + " -> " + (isBull(b) ? "BU," : "") + (isBear(b) ? "BE," : "") + (isRev(b) ? "RV," : "") + (isHC(b) ? "HC" : "") + (isLC(b) ? "LC" : "") + (isNC(b) ? "NC" : "") + (isNA(b) ? "NA" : "") + (isNH(b) ? "NH" : "") + (isNL(b) ? "NL" : "");
        }
    }

}
