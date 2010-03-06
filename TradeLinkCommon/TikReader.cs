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
        /// <summary>
        /// estimate of ticks contained in file
        /// </summary>
        public int ApproxTicks = 0;
        /// <summary>
        /// real symbol for data represented in file
        /// </summary>
        public string RealSymbol { get { return _realsymbol; } }
        /// <summary>
        /// security-parsed symbol
        /// </summary>
        public string Symbol { get { return _sym; } }
        /// <summary>
        /// security represented by parsing realsymbol
        /// </summary>
        /// <returns></returns>
        public Security ToSecurity() { return _sec; }
        /// <summary>
        /// file is readable, has version and real symbol
        /// </summary>
        public bool isValid { get { return (_filever != 0) && (_realsymbol != string.Empty) && BaseStream.CanRead; } }
        /// <summary>
        /// count of ticks presently read
        /// </summary>
        public int Count = 0;
        public TikReader(string filepath) : base(new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read)) 
        {
            _path = filepath;
            FileInfo fi = new FileInfo(filepath);
            ApproxTicks = (int)((double)fi.Length / 39);
            ReadHeader();
        }

        bool _haveheader = false;
        int _filever = 0;

        void ReadHeader()
        {
            // get version id
            ReadByte();
            // get version
            _filever = ReadInt32();
            if (_filever != TikConst.FILECURRENTVERSION)
                throw new BadTikFile("version: " + _filever + " expected: " + TikConst.FILECURRENTVERSION);
            // get real symbol
            _realsymbol = ReadString();
            // get security from symbol
            _sec = TradeLink.Common.SecurityImpl.Parse(_realsymbol);
            _sec.Date = SecurityImpl.SecurityFromFileName(_path).Date;
            // get short symbol
            _sym = _sec.Symbol;
            // get end of header
            ReadByte();
            // make sure we read something
            if (_realsymbol.Length <= 0)
                throw new BadTikFile("no symbol defined in tickfile");
            // flag header as read
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
                TickImpl k = new TradeLink.Common.TickImpl(_realsymbol);
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
                            k.datetime = ((long)k.date * 1000000) + (long)k.time;
                            k._ask = ReadUInt64();
                            k.os = ReadInt32();
                            k.oe = ReadString();
                            k.depth = ReadInt32();
                            break;
                        }
                    case TikConst.TickBid:
                        {
                            k.date = ReadInt32();
                            k.time = ReadInt32();
                            k.datetime = ((long)k.date * 1000000) + (long)k.time;
                            k._bid = ReadUInt64();
                            k.bs = ReadInt32();
                            k.be = ReadString();
                            k.depth = ReadInt32();
                        }
                        break;
                    case TikConst.TickFull:
                        {
                            k.date = ReadInt32();
                            k.time = ReadInt32();
                            k.datetime = ((long)k.date * 1000000) + (long)k.time;
                            k._trade = ReadUInt64();
                            k.size = ReadInt32();
                            k.ex = ReadString();
                            k._bid = ReadUInt64();
                            k.bs = ReadInt32();
                            k.be = ReadString();
                            k._ask = ReadUInt64();
                            k.os = ReadInt32();
                            k.oe = ReadString();
                            k.depth = ReadInt32();
                        }
                        break;
                    case TikConst.TickQuote:
                        {
                            k.date = ReadInt32();
                            k.time = ReadInt32();
                            k.datetime = ((long)k.date * 1000000) + (long)k.time;
                            k._bid = ReadUInt64();
                            k.bs = ReadInt32();
                            k.be = ReadString();
                            k._ask = ReadUInt64();
                            k.os = ReadInt32();
                            k.oe = ReadString();
                            k.depth = ReadInt32();
                        }
                        break;
                    case TikConst.TickTrade:
                        {
                            k.date = ReadInt32();
                            k.time = ReadInt32();
                            k.datetime = ((long)k.date * 1000000) + (long)k.time;
                            k._trade = ReadUInt64();
                            k.size = ReadInt32();
                            k.ex = ReadString();
                        }
                        break;
                    default:
                        // weird data, try to keep reading 
                        ReadByte();
                        // but don't send this tick, just get next record
                        return true;
                }
                // send any tick we have
                if (gotTick != null)
                    gotTick(k);
                // read end of tick
                ReadByte();
                // count it
                Count++;
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

    public class BadTikFile : Exception
    {
        public BadTikFile() : base() { }
        public BadTikFile(string message) : base(message) { }
    }
}
