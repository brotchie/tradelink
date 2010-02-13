using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;


namespace TradeLink.Common
{
    public class MessageTracker
    {
        TLClient _tl = null;
        /// <summary>
        /// create a message tracker
        /// </summary>
        public MessageTracker() : this(null) { }
        /// <summary>
        /// create a message tracker that communicates with a TL client
        /// </summary>
        /// <param name="tl"></param>
        public MessageTracker(TLClient tl)
        {
            _tl = tl;
        }
        /// <summary>
        /// thrown if open price is received
        /// </summary>
        public event StringDecimalDelegate GotOpenPrice;
        /// <summary>
        /// triggered if most recent close is received
        /// </summary>
        public event StringDecimalDelegate GotClosePrice;
        /// <summary>
        /// triggered if day high is received (any exchange, all day)
        /// </summary>
        public event StringDecimalDelegate GotHighPrice;
        /// <summary>
        /// triggered if day low is received (any exchange, all day)
        /// </summary>
        public event StringDecimalDelegate GotLowPrice;
        /// <summary>
        /// triggered if day high is received (nyse only, nyse hours)
        /// </summary>
        public event StringDecimalDelegate GotNyseHighPrice;
        /// <summary>
        /// triggered if day lowis received (nyse only, nyse hours)
        /// </summary>
        public event StringDecimalDelegate GotNyseLowPrice;
        /// <summary>
        /// triggered if day high is received (any exchange, nyse hours)
        /// </summary>
        public event StringDecimalDelegate GotIntraHighPrice;
        /// <summary>
        /// triggered if day lowis received (any exchange, nyse hours)
        /// </summary>
        public event StringDecimalDelegate GotIntraLowPrice;
        /// <summary>
        /// triggered if list of feature is received
        /// </summary>
        public event MessageArrayDel GotFeatures;
        /// <summary>
        /// triggered if debug is sent
        /// </summary>
        public event DebugDelegate SendDebug;
        /// <summary>
        /// triggered if send messageresponse occurs
        /// </summary>
        public event MessageDelegate SendMessageResponse;
        /// <summary>
        /// triggered if provider is received
        /// </summary>
        public event ProviderDelegate GotProvider;
        public virtual bool GotMessage(MessageTypes type, uint source, uint dest, uint msgid, string request, ref string response)
        {
            decimal v = 0;
            long lv = 0;
            switch (type)
            {
                case MessageTypes.CLOSEPRICE:
                    {
                        if ((GotClosePrice != null) && long.TryParse(response, out lv))
                        {
                            GotClosePrice(request, WMUtil.unpack(lv));
                        }
                        return true;
                    }
                case MessageTypes.OPENPRICE:
                    {
                        if ((GotOpenPrice != null) && long.TryParse(response, out lv))
                            GotOpenPrice(request, WMUtil.unpack(lv));
                        return true;
                    }
                case MessageTypes.DAYHIGH:
                    {
                        if ((GotHighPrice != null) && long.TryParse(response, out lv))
                            GotHighPrice(request, WMUtil.unpack(lv));
                        return true;
                    }
                case MessageTypes.DAYLOW:
                    {
                        if ((GotLowPrice != null) && long.TryParse(response, out lv))
                            GotLowPrice(request, WMUtil.unpack(lv));
                        return true;
                    }
                case MessageTypes.NYSEDAYHIGH:
                    {
                        if ((GotNyseHighPrice != null) && long.TryParse(response, out lv))
                            GotNyseHighPrice(request, WMUtil.unpack(lv));
                        return true;
                    }
                case MessageTypes.NYSEDAYLOW:
                    {
                        if ((GotNyseLowPrice != null) && long.TryParse(response, out lv))
                            GotNyseLowPrice(request, WMUtil.unpack(lv));
                        return true;
                    }
                case MessageTypes.INTRADAYHIGH:
                    {
                        if ((GotIntraHighPrice != null) && long.TryParse(response, out lv))
                            GotIntraHighPrice(request, WMUtil.unpack(lv));
                        return true;
                    }
                case MessageTypes.INTRADAYLOW:
                    {
                        if ((GotIntraLowPrice != null) && long.TryParse(response, out lv))
                            GotIntraLowPrice(request, WMUtil.unpack(lv));
                        return true;
                    }

                case MessageTypes.BROKERNAME:
                    {
                        if (GotProvider != null)
                        {
                            try
                            {
                                Providers p = (Providers)Enum.Parse(typeof(Providers), response);
                                GotProvider(p);
                            }
                            catch (Exception ex)
                            {
                                debug("Unknown provider: " + response);
                                debug(ex.Message + ex.StackTrace);
                                return false;
                            }
                        }
                        return true;
                    }
                case MessageTypes.FEATURERESPONSE:
                    {
                        if (GotFeatures != null)
                        {
                            string[] r = response.Split(',');
                            List<MessageTypes> f = new List<MessageTypes>();
                            foreach (string rs in r)
                            {
                                try
                                {
                                    MessageTypes mt = (MessageTypes)Enum.Parse(typeof(MessageTypes), rs);
                                    f.Add(mt);
                                }
                                catch { continue; }
                            }
                            if (f.Count > 0)
                                GotFeatures(f.ToArray());
                        }
                        return true;
                    }
            }
            return false;
        }

        public virtual bool SendMessage(MessageTypes type, uint source, uint dest, uint msgid, string request, string response)
        {
            if (_tl == null)return false;
            if (!_tl.RequestFeatureList.Contains(type))
            {
                SendDebug(type.ToString() + " not supported by " + _tl.Name);
                return false;
            }
            try
            {
                // prepare message
                switch (type)
                {
                    case MessageTypes.DOMREQUEST:
                        request.Replace(Book.EMPTYREQUESTOR, _tl.Name);
                        break;
                    case MessageTypes.FEATUREREQUEST:
                        request = _tl.Name;
                        break;
                }
                // send it
                long result = _tl.TLSend(type, request);
                string res = result.ToString();
                // pass along result
                if ((SendMessageResponse != null) && (result != 0))
                    SendMessageResponse(type, source, dest, msgid, request, ref res);
                return true;
            }
            catch (Exception ex)
            {
                debug("Error on: "+type.ToString()+ source + dest + msgid + request + response);
                debug(ex.Message + ex.StackTrace);
            }
            return false;
        }
        

        void debug(string m)
        {
            if (SendDebug != null)
                SendDebug(m);
        }

        public const char delim = '+';
        public const int PARAM1 = 0;
        public const int PARAM2 = 1;
        public const int PARAM3 = 1;
        public static string[] ParseRequest(string request)
        {
            return request.Split(delim);
        }
        public static string RequestParam(string request,int param) { return ParseRequest(request)[param]; }
        public static string BuildParams(string[] param)
        {
            return string.Join(delim.ToString(), param);
        }
    }
}
