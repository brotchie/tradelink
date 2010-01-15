using System;
using System.Collections.Generic;
using System.Text;
using VBRediClasses;
using TradeLink.API;
using TradeLink.Common;

namespace ServerRedi
{
    public class ServerRedi : TLServer_WM
    {
        public const string PROGRAM = "RediServer";
        VBCacheClass _cc;
        VBOrderClass _oc;
        bool _conn = false;
        public bool isConnected { get { return _conn; } }
        public ServerRedi()
        {
            newProviderName = Providers.REDI;
        }
        public event DebugDelegate SendDebug;
        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);
        }
        public bool Start()
        {
            try
            {
                debug(Util.TLSIdentity());
                debug("Attempting to start " + PROGRAM);
                _cc = new VBCacheClass();
                _oc = new VBOrderClass();
                _cc.VBRediCache.CacheEvent += new RediLib.ECacheControl_CacheEventEventHandler(VBRediCache_CacheEvent);
            }
            catch (Exception ex)
            {
                debug("error starting "+PROGRAM);
                debug(ex.Message + ex.StackTrace);
                debug("Did you forget to login to Redi?");
                _conn = false;
                return false;
            }
            debug("Started successfully.");
            debug("User: " + _cc.VBRediCache.UserID);
            _conn = true;
            return true;
        }

        public override void Stop()
        {
            try
            {

            }
            catch { }
            base.Stop();
        }

        void VBRediCache_CacheEvent(int action, int row)
        {
            switch (action)
            {
                case 1:
                    break;
                default:
                    break;
            }

        }
    }
}
