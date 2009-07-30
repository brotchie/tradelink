using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using System.IO;

namespace TradeLink.Common
{
    /// <summary>
    /// read tradelink tick files
    /// </summary>
    public class TikReader : BinaryReader
    {
        string _realsymbol = string.Empty;
        string _sym = string.Empty;
        Security _sec = new TradeLink.Common.SecurityImpl();
        string _path = string.Empty;
        int ApproxTicks = 0;

        public TikReader(string filepath) : base(new FileStream(filepath, FileMode.Open)) 
        {
            _path = filepath;
            FileInfo fi = new FileInfo(filepath);
            ApproxTicks = (int)((double)fi.Length / 40);
        }

        bool _haveheader = false;
        int _filever = 0;

        void ReadHeader()
        {
            // get version id
            ReadByte();
            // get version
            _filever = (int)ReadByte();
            // get real symbol
            _realsymbol = ReadString();
            // get security from symbol
            _sec = TradeLink.Common.SecurityImpl.Parse(_realsymbol);
            // get short symbol
            _sym = _sec.Symbol;
            // get end of header
            ReadByte();
            // set flag
            _haveheader = true;
        }

        public event TickDelegate gotTick;

        /// <summary>
        /// returns true if more data to process, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool NextTick()
        {
            if (!_haveheader)
                ReadHeader();
            try
            {
                // get tick type
                byte type = ReadByte();
                // prepare a tick
                Tick k = new TradeLink.Common.TickImpl(_realsymbol);
                // get the tick
                switch (type)
                {
                    case TikConst.EndData: return false; 
                    case TikConst.EndTick: return true; 
                    case TikConst.StartData: return true; 
                    case TikConst.Version: return true; 
                    case TikConst.TickAsk:
                        {
                            k.date = ReadInt32();
                            k.time = ReadInt32();
                            k.iask = ReadInt32();
                            k.os = ReadInt32();
                            k.oe = ReadString();
                            k.depth = ReadInt32();
                            break;
                        }
                    case TikConst.TickBid:
                        {
                            k.date = ReadInt32();
                            k.time = ReadInt32();
                            k.ibid = ReadInt32();
                            k.bs = ReadInt32();
                            k.be = ReadString();
                            k.depth = ReadInt32();
                        }
                        break;
                    case TikConst.TickFull:
                        {
                            k.date = ReadInt32();
                            k.time = ReadInt32();
                            k.itrade = ReadInt32();
                            k.size = ReadInt32();
                            k.ex = ReadString();
                            k.ibid = ReadInt32();
                            k.bs = ReadInt32();
                            k.be = ReadString();
                            k.iask = ReadInt32();
                            k.os = ReadInt32();
                            k.oe = ReadString();
                            k.depth = ReadInt32();
                        }
                        break;
                    case TikConst.TickQuote:
                        {
                            k.date = ReadInt32();
                            k.time = ReadInt32();
                            k.ibid = ReadInt32();
                            k.bs = ReadInt32();
                            k.be = ReadString();
                            k.iask = ReadInt32();
                            k.os = ReadInt32();
                            k.oe = ReadString();
                            k.depth = ReadInt32();
                        }
                        break;
                    case TikConst.TickTrade:
                        {
                            k.date = ReadInt32();
                            k.time = ReadInt32();
                            k.itrade = ReadInt32();
                            k.size = ReadInt32();
                            k.ex = ReadString();
                        }
                        break;
                }
                // send any tick we have
                if (gotTick != null)
                    gotTick(k);
                // read end of tick
                ReadByte();
                // assume there is more
                return true;
            }
            catch (EndOfStreamException)
            {
                return false;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }

        }



    }
}
