using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// utility class to support windows messaging transport for tradelink clients.
    /// </summary>
    public static class WMUtil
    {
        // used to unpack an lparam to a decimal
        public static decimal unpack(long i)
        {
            if (i == 0) return 0;
            int w = (int)(i >> 16);
            int f = (int)(i - (w << 16));
            decimal dec = (decimal)w;
            decimal frac = (decimal)f;
            frac /= 1000;
            dec += frac;
            return dec;
        }
        // used to pack a decimal to a long for use in returning LPARAMS via COPYDATA
        public static long pack(decimal d)
        {
            if (d == 0) return 0;
            int whole = (int)Math.Truncate(d);
            int frac = (int)(1000*(d - whole));
            long packed = (whole << 16) + frac;
            return packed;
        }

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        public const string CLIENTWINDOW = "TradeLinkClient";
        public const string SERVERWINDOW = "TradeLinkServer";
        public const string SIMWINDOW = "TL-BROKER-SIMU";
        public const string LIVEWINDOW = "TL-BROKER-LIVE";
        public const string REPLAYWINDOW = "TL-BROKER-HIST";
        public const string TESTWINDOW = "TL-BROKER-TEST";

        //SendMessage
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(System.IntPtr hwnd, int msg, int wparam, ref COPYDATASTRUCT lparam);
        const int WM_COPYDATA = 0x004A;

        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string ClassName, string WindowName);

                /// <summary>
        /// Gets a handle for a given window name.  Will return InPtr.Zero if no match is found.
        /// </summary>
        /// <param name="WindowName">Name of the window.</param>
        /// <returns></returns>
        public static IntPtr HisHandle(string WindowName)
        {
            IntPtr p = IntPtr.Zero;
            try
            {
                p = WMUtil.FindWindow(null, WindowName);
            }
            catch (NullReferenceException) { }
            return p;
        }

        private static IntPtr FindClient(string name) { return FindWindow(null, name); }

        public static bool Found(string name) { return (FindClient(name) != IntPtr.Zero); }

        public static string GetUniqueWindow(string RootWindowName)
        {
            string name = RootWindowName;
            if (Found(name))
            {
                int inst = -1;
                do
                {
                    inst++;
                } while (Found(name + "." + inst.ToString()));
                name += "." + inst.ToString();
            }
            return name;
        }

        public static TradeLinkMessage ToTradeLinkMessage(ref System.Windows.Forms.Message windowsmessageref)
        {
            if (windowsmessageref.Msg != WM_COPYDATA)
                return null;
            TradeLinkMessage m = new TradeLinkMessage();
            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            cds = (COPYDATASTRUCT)Marshal.PtrToStructure(windowsmessageref.LParam, typeof(COPYDATASTRUCT));
            if (cds.cbData > 0)
            {
                m.body = Marshal.PtrToStringAnsi(cds.lpData);
                m.type = (MessageTypes)(int)cds.dwData;
            }
            return m;
        }

        /// <summary>
        /// Sends the MSG.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="desthandle">The desthandle.</param>
        /// <param name="sourcehandle">The sourcehandle.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static long SendMsg(string str, System.IntPtr desthandle, System.IntPtr sourcehandle, int type)
        {
            if ((desthandle == IntPtr.Zero) || (sourcehandle == IntPtr.Zero)) return -1; // fail on invalid handles
            WMUtil.COPYDATASTRUCT cds = new WMUtil.COPYDATASTRUCT();
            cds.dwData = (IntPtr)type;
            str = str + '\0';
            cds.cbData = str.Length + 1;

            System.IntPtr pData = Marshal.AllocCoTaskMem(str.Length);
            pData = Marshal.StringToCoTaskMemAnsi(str);

            cds.lpData = pData;

            IntPtr res = WMUtil.SendMessage(desthandle, WMUtil.WM_COPYDATA, (int)sourcehandle, ref cds);
            Marshal.FreeCoTaskMem(pData);
            return res.ToInt64();
        }

        /// <summary>
        /// Sends the MSG from source window to destination window, using WM_COPYDATA.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messagetype">The messagetype.</param>
        /// <param name="destinationwindow">The destinationwindow.</param>
        /// <returns></returns>
        public static long SendMsg(string message, MessageTypes messagetype, IntPtr sourcehandle,string destinationwindow)
        {
            IntPtr him = WMUtil.HisHandle(destinationwindow);
            return WMUtil.SendMsg(message, him,sourcehandle, (int)messagetype);
        }
    }

    public class TradeLinkMessage
    {
        public MessageTypes type = MessageTypes.UNKNOWN_MESSAGE;
        public string body = "";
        public long response = (long)MessageTypes.OK;
    }
}
