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
            newRegisterStocks += new DebugDelegate(ServerRedi_newRegisterStocks);
        }

        void ServerRedi_newRegisterStocks(string msg)
        {
            debug("Subscribe request: " + msg);
            if (!isConnected)
            {
                debug("not connected.");
                return;
            }

            // get symbols
            Basket b = BasketImpl.FromString(msg);
            object err = null;
            foreach (Security s in b)
            {
                try
                {
                    object vret = _cc.VBRediCache.Submit("L1", true, ref err);
                    checkerror(ref err);
                    _cc.VBRediCache.AddWatch(0, s.Symbol, string.Empty, ref err);
                    checkerror(ref err);
                }
                catch (Exception ex)
                {
                    debug(s.Symbol + " error subscribing: " + ex.Message + ex.StackTrace);
                }
            }
            
        }

        void checkerror(ref object err)
        {
            if (err != null)
                debug(err.ToString());
            err = null;
        }

        // respond to redi events
        void VBRediCache_CacheEvent(int action, int row)
        {
            int err = 0;
            object cv = new object();
            switch (action)
            {
                case 1: //CN_SUBMIT
                    {


                        for (int i = 0; i < row; i++)
                        {
                            _cc.VBGetCell(i, "Text", ref cv, ref err);
                            debug(cv.ToString());
                        }
                    }
                    break;
                    /*
                case 4 : //CN_INSERT
                    break;
                case 5: // CN_UPDATE
                    break;
                case 7: // CN_REMOVING
                    break;
                case 8 : // CN_REMOVED
                    break;
                     */
                default:
                    {
                        _cc.VBGetCell(row, "Text", ref cv, ref err);
                        debug(cv.ToString());
                    }
                    break;
            }

        }

        public event DebugDelegate SendDebug;
        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);
        }
        public bool Start(string user,string pw)
        {
            try
            {
                debug(Util.TLSIdentity());
                debug("Attempting to start " + PROGRAM);
                _cc = new VBCacheClass();
                _oc = new VBOrderClass();
                _cc.VBRediCache.CacheEvent += new RediLib.ECacheControl_CacheEventEventHandler(VBRediCache_CacheEvent);
                _cc.VBRediCache.UserID = user;
                _cc.VBRediCache.Password = pw;
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


    }
}
