/*
    The AmeritradeBrokerAPI Class Library

    Copyright (c) 2008 by Cedric L. Harris and ATrade Investment Technologies, LLC All rights reserved.
    Contact Info. CHarris@Gr8Alerts.COM
                  http://WWW.FindMyNextTrade.COM


    The AmeritradeBrokerAPI Class Library is a C# implementation of interface specification defined by
    TD Amertirade Securities, INC


    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions
    are met:
    1. Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
    2. Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution.

    THIS SOFTWARE IS PROVIDED BY THE AUTHOR AND CONTRIBUTORS ``AS IS'' AND
    ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
    ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
    FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
    OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
    HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
    LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
    OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    SUCH DAMAGE.
*/

#define SIGNED_TD_NDA

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Threading;
using System.IO.Compression;
using System.Text.RegularExpressions;

using System.Xml;
using System.IO;
using System.Collections;
using MSXML2;
using NZlib.Streams;



namespace AMTD_API
{

    public class AmeritradeBrokerAPI
    {
        public static string SOURCEID = "TRDLNK";

        #region Asyncronous Section

        public delegate void EventHandlerWithArgs(DateTime time, AmeritradeBrokerAPI.ATradeArgument args);


        /*/
         * Available Streaming State Objects.
         * 
        /*/

        public RequestState rs_ActivesStreaming = null;
        public RequestState rs_ChartSnapShot = null;
        public RequestState rs_ChartStreaming = null;

        public RequestState rs_LevelOneSnapshot = null;
        public RequestState rs_LevelOneStreaming = null;

        public RequestState rs_LevelTwoStreaming = null;

        public RequestState rs_NewsStreaming = null;
        public RequestState rs_OptionQuoteStreaming = null;

        public static object oSyncLocker = new object();




        public class RequestState
        {
            // Create Decoder for appropriate encoding type


            public HttpWebRequest Request;
            public Stream ResponseStream;
            public Level2DataSource Level2DataSourceType;
            public StringBuilder RequestData;

            public Decoder StreamDecode = Encoding.UTF8.GetDecoder();
            const int BUFFER_SIZE = 65535 * 10;
            public AsyncType FunctionType = AsyncType.None;
            public long Level2ByteArray2NDx = 0;

            public byte[] Level2ByteArray2 = new byte[BUFFER_SIZE];
            public byte[] ChartByteArray2 = new byte[BUFFER_SIZE];
            public byte[] BufferRead = new byte[BUFFER_SIZE];

            public long ChartByteArray2NDx = 0;
            public string stockSymbol = string.Empty;
            public string ServiceName = string.Empty;
            public string OriginalSerivceName = string.Empty;
            public object locker = new object();
            public AmeritradeBrokerAPI oParent = null;
            public Form oParentForm = null;
            public bool lNewStockSymbol = false;

            List<String> cSortedBid = new List<string>();
            List<String> cSortedAsk = new List<string>();
            private int nTotalBidOrders = 0;
            private int nTotalAskOrders = 0;


            public enum AsyncType
            {
                None = 0,
                ActivesStreaming = 1,
                ChartSnapshot = 2,
                ChartStreaming = 3,
                LevelOneSnapshot = 4,
                LevelOneStreaming = 5,
                LevelTwoStreaming = 6,
                ResendRequest = 7
            }


            public event EventHandlerWithArgs TickWithArgs;


            public RequestState()
            {
                BufferRead = new byte[BUFFER_SIZE];
                RequestData = new StringBuilder("");
                Request = null;
                ResponseStream = null;
                GC.Collect();

            }




            public void CloseStream(RequestState lrs)
            {
                try
                {
                    // Get a lock on the RequestState Object

                    lock (lrs)
                    {
                        Monitor.Enter(lrs);

                        lrs.lNewStockSymbol = true;
                        System.Threading.Thread.Sleep(300);
                        lrs.Request.Abort();
                        System.Threading.Thread.Sleep(100);
                        lrs.ResponseStream.Flush();
                        lrs.ResponseStream.Close();

                        Monitor.Exit(lrs);
                        Monitor.Pulse(lrs);
                    }
                }
                catch (Exception exc) { }

            }



            public void SendData(String cData, string _serviceName)
            {

                ATradeArgument oParameters = new ATradeArgument();
                oParameters.DisplayMssg = cData;
                oParameters.ServiceName = _serviceName;

                TickWithArgs.Invoke(DateTime.Now, oParameters);

            }



            public void SendLevelOneDataSnapshot(List<L1quotes> cData, string stockSymbol)
            {

                ATradeArgument oParameters = new ATradeArgument();
                oParameters.FunctionType = AsyncType.LevelOneSnapshot;
                oParameters.oLevelOneData = cData;
                oParameters.stocksymbol = stockSymbol;
                oParameters.DisplayMssg = string.Empty;

                TickWithArgs.Invoke(DateTime.Now, oParameters);

            }



            public void SendLevelOneDataStreaming(List<L1quotes> cData, string stockSymbol, string _serviceName)
            {

                ATradeArgument oParameters = new ATradeArgument();
                oParameters.FunctionType = AsyncType.LevelOneStreaming;
                oParameters.oLevelOneData = cData;
                oParameters.ServiceName = _serviceName;
                oParameters.stocksymbol = stockSymbol;
                oParameters.DisplayMssg = string.Empty;

                TickWithArgs.Invoke(DateTime.Now, oParameters);

            }



            public void SendLevel2Data(List<string> cBidData, List<string> cAskData, string stockSymbol, string _serviceName)
            {

                ATradeArgument oParameters = new ATradeArgument();
                oParameters.FunctionType = AsyncType.LevelTwoStreaming;
                oParameters.oLevel2BidData = cBidData;
                oParameters.oLevel2AskData = cAskData;

                oParameters.stocksymbol = stockSymbol;
                oParameters.ServiceName = _serviceName;
                oParameters.DisplayMssg = string.Empty;


                TickWithArgs.Invoke(DateTime.Now, oParameters);

            }




            public void SendHistoricalChartData(List<string> cData, string stockSymbol, string _serviceName)
            {

                List<string> oChartDay = new List<string>();
                List<List<string>> oChartDataByDay = new List<List<string>>();
                string cDataNDX = string.Empty;
                string cLastDataNDX = string.Empty;
                bool lDataAdded = false;

                foreach (string oString in cData)
                {

                    string[] cLineData = oString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    cDataNDX = cLineData[8];

                    if (cDataNDX.CompareTo(cLastDataNDX) == 0 || cLastDataNDX.Length == 0)
                    {
                        oChartDay.Add(oString);
                        lDataAdded = false;
                    }
                    else
                    {
                        oChartDataByDay.Add(oChartDay);
                        oChartDay = new List<string>();
                        lDataAdded = true;

                        oChartDay.Add(oString);

                    }
                    cLastDataNDX = cDataNDX;


                }

                if (lDataAdded == false)
                {
                    oChartDataByDay.Add(oChartDay);
                }


                ATradeArgument oParameters = new ATradeArgument();
                oParameters.FunctionType = AsyncType.ChartSnapshot;
                oParameters.oHistoricalData = oChartDataByDay;

                oParameters.stocksymbol = stockSymbol;
                oParameters.ServiceName = _serviceName;
                oParameters.DisplayMssg = string.Empty;


                TickWithArgs.Invoke(DateTime.Now, oParameters);

            }



            public void SendHistoricalChartDataStreaming(string Symbol, String cData, string _serviceName)
            {

                ATradeArgument oParameters = new ATradeArgument();
                oParameters.FunctionType = AsyncType.ChartStreaming;
                oParameters.DisplayMssg = cData;
                oParameters.stocksymbol = Symbol;
                oParameters.ServiceName = _serviceName;

                TickWithArgs.Invoke(DateTime.Now, oParameters);

            }



            public string TD_GetResponseValue(Int32 valueType, byte[] value, Int32 nStart, Int32 nLength)
            {
                string replyvalue = string.Empty;

                switch (valueType)
                {
                    // Strings or Characters
                    case 0:

                        if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("U") == 0)
                            replyvalue = "UP";
                        else
                            if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("D") == 0)
                                replyvalue = "UP";
                            else
                                if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("\0 ") == 0)
                                    replyvalue = "UNCH";
                                else
                                    if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("\0n") == 0)
                                        replyvalue = "NYSE";
                                    else
                                        if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("\0q") == 0)
                                            replyvalue = "NASDAQ";
                                        else
                                            if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("\0u") == 0)
                                                replyvalue = "OTCBB";
                                            else
                                                if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("\0p") == 0)
                                                    replyvalue = "PACX";
                                                else
                                                    replyvalue = System.Text.Encoding.ASCII.GetString(value, nStart, nLength);

                        break;

                    case 1:
                        bool lBoolValue = BitConverter.ToBoolean(value, 0);
                        replyvalue = lBoolValue.ToString();
                        break;


                    case 2:
                        Int16 nCharValue = BitConverter.ToInt16(value, 0);
                        replyvalue = nCharValue.ToString();
                        break;


                    // Float or Integer 32 // 4 bytes
                    case 4:

                        float nFloat_ShortValue = BitConverter.ToSingle(value, 0);
                        replyvalue = nFloat_ShortValue.ToString();
                        break;

                    // Long // 8 bytes
                    case 8:
                        long nLongValue = BitConverter.ToInt64(value, 0);
                        replyvalue = nLongValue.ToString();
                        break;

                    default:
                        Int32 nDefaultValue = BitConverter.ToInt32(value, 0);
                        replyvalue = nDefaultValue.ToString();
                        break;

                }
                return replyvalue;

            }



            public void process_AsyncActivesStreaming(IAsyncResult asyncResult)
            {

                // Get the RequestState object from the asyncresult
                RequestState rs = (RequestState)asyncResult.AsyncState;

                if (rs.Request.ServicePoint.CurrentConnections > 0)
                {


                    // Pull out the ResponseStream that was set in RespCallback
                    Stream responseStream = rs.ResponseStream;

                    // At this point rs.BufferRead should have some data in it.
                    // Read will tell us if there is any data there

                    int read = responseStream.EndRead(asyncResult);


                    if (read > 0)
                    {

                        rs.RequestData.Append(Encoding.ASCII.GetString(rs.BufferRead, 0, read));

                        // Make another call so that we continue retrieving any all incoming data                                    
                        IAsyncResult ar = responseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);

                    }
                    else
                    {

                        // Close the response stream since we do not have any more incoming data.
                        responseStream.Close();

                        // If we have not more bytes read, then the server has finished sending us data.
                        if (read == 0) { }
                    }
                }
            }



            public void process_AsyncLevelOneSnapshot(IAsyncResult asyncResult)
            {

                // Get the RequestState object from the asyncresult
                RequestState rs = (RequestState)asyncResult.AsyncState;


                if (rs.lNewStockSymbol == true)
                {
                    return;
                }


                try
                {
                    if (rs.Request.ServicePoint.CurrentConnections > 0)
                    {

                        // Pull out the ResponseStream that was set in RespCallback
                        Stream responseStream = rs.ResponseStream;

                        // At this point rs.BufferRead should have some data in it.
                        // Read will tell us if there is any data there

                        int read = responseStream.EndRead(asyncResult);


                        if (read > 0)
                        {
                            Array.Copy(rs.BufferRead, 0, ChartByteArray2, ChartByteArray2NDx, read);
                            ChartByteArray2NDx = ChartByteArray2NDx + read;

                            // Make another call so that we continue retrieving any all incoming data                                    
                            IAsyncResult ar = responseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);

                        }
                        else
                        {

                            // Close the response stream since we do not have any more incoming data.


                            ChartByteArray2NDx = 0;

                            // If we have not more bytes read, then the server has finished sending us data.
                            if (read == 0)
                            {

                                List<L1quotes> oL1Quotes = new List<L1quotes>();
                                L1quotes oL1Quote = new L1quotes();

                                string compressedText = Convert.ToBase64String(ChartByteArray2);
                                byte[] gzBuffer = Convert.FromBase64String(compressedText);



                                MemoryStream ms = new MemoryStream();
                                int nFieldNDX = 0;
                                int nStartPos = 13;
                                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                                ms.Write(gzBuffer, 64, gzBuffer.Length - 64);

                                byte[] nMsg = new byte[sizeof(Int32)];

                                ms.Close();

                                /*/
                                 * S = Streaming
                                 * N = Snapshot
                                /*/

                                nMsg[0] = gzBuffer[0];
                                string cRequestType = System.Text.Encoding.ASCII.GetString(nMsg, 0, 1);



                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[2];
                                nMsg[1] = gzBuffer[1];
                                int nNextFieldLength = BitConverter.ToInt32(nMsg, 0);



                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[5];
                                nMsg[1] = gzBuffer[4];
                                nMsg[2] = gzBuffer[3];

                                long nNextField = BitConverter.ToInt32(nMsg, 0);



                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[7];
                                nMsg[1] = gzBuffer[6];
                                int nMessageLength = BitConverter.ToInt32(nMsg, 0);



                                /*/
                                 * 1 = L1 Quote
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[9];
                                nMsg[1] = gzBuffer[8];
                                int nQuoteType = BitConverter.ToInt32(nMsg, 0);


                                /*/
                                 * Stock Symbol : Field 0
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[10];
                                int nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[12];
                                nMsg[1] = gzBuffer[11];
                                int nSymbolLength = BitConverter.ToInt32(nMsg, 0);
                                nMsg = new byte[nSymbolLength];
                                Array.Copy(gzBuffer, nStartPos, nMsg, 0, nSymbolLength);
                                oL1Quote.stock = rs.TD_GetResponseValue(0, nMsg, 0, nSymbolLength);



                                /*/
                                 * Bid Price : Field 1
                                /*/

                                nFieldNDX = nStartPos + nSymbolLength;


                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.bid = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                                /*/
                                 * Ask Price : Field 2
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.ask = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                                /*/
                                 * Last Price : Field 3
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.last = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                                /*/
                                 * Bid Size : Field 4
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.bid_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);


                                /*/
                                 * Ask Size : Field 5
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.ask_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                                /*/
                                 * Bid ID : Field 6
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(short)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.bid_id = TD_GetResponseValue(sizeof(short), nMsg, nStartPos, 0);


                                /*/
                                 * Ask ID : Field 7
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(short)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.ask_id = TD_GetResponseValue(sizeof(short), nMsg, nStartPos, 0);


                                /*/
                                 * Volume : Field 8
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(long)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                nMsg[4] = gzBuffer[nFieldNDX++];
                                nMsg[5] = gzBuffer[nFieldNDX++];
                                nMsg[6] = gzBuffer[nFieldNDX++];
                                nMsg[7] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.volume = TD_GetResponseValue(sizeof(long), nMsg, nStartPos, 0);




                                /*/
                                 * Last Size : Field 9
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.lastsize = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                                /*/
                                 * Trade Time : Field 10
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.tradetime = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                                /*/
                                 * Quote Time : Field 11
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.quotetime = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                                /*/
                                 * High : Field 12
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.high = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                                /*/
                                 * Low : Field 13
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.low = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                                /*/
                                 * Tick : Field 14
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(char)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);

                                nMsg = new byte[sizeof(char)];
                                Array.Copy(gzBuffer, nStartPos, nMsg, 0, sizeof(char));
                                oL1Quote.tick = TD_GetResponseValue(0, nMsg, 0, sizeof(char));



                                /*/
                                 * Close : Field 15
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.close = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);




                                /*/
                                  * Exchange : Field 16
                                 /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(char)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);

                                nMsg = new byte[sizeof(char)];
                                Array.Copy(gzBuffer, nStartPos, nMsg, 0, sizeof(char));
                                oL1Quote.exchange = TD_GetResponseValue(0, nMsg, 0, sizeof(char));



                                /*/
                                 * Marginable : Field 17
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(bool)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.marginable = TD_GetResponseValue(sizeof(bool), nMsg, 0, sizeof(char));


                                /*/
                                 * Shortable : Field 18
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(bool)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.shortable = TD_GetResponseValue(sizeof(bool), nMsg, 0, sizeof(char));


                                /*/
                                 * ISLAND BID : Field 19
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.islandbid = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                                /*/
                                 * ISLAND ASK : Field 20
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.islandask = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                                /*/
                                 * ISLAND ASK : Field 21
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(long)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                nMsg[4] = gzBuffer[nFieldNDX++];
                                nMsg[5] = gzBuffer[nFieldNDX++];
                                nMsg[6] = gzBuffer[nFieldNDX++];
                                nMsg[7] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.islandvolume = TD_GetResponseValue(sizeof(long), nMsg, nStartPos, 0);



                                /*/
                                 * QUOTE DATE : Field 22
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.quotedate = TD_GetResponseValue(100, nMsg, nStartPos, 0);




                                /*/
                                 * QUOTE DATE : Field 23
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.tradedate = TD_GetResponseValue(100, nMsg, nStartPos, 0);


                                /*/
                                 * Volatility : Field 24
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.volatility = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                                /*/
                                 * Description : Field 25
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(short)];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nStartPos = nFieldNDX;

                                int nStringlLength = BitConverter.ToInt16(nMsg, 0);
                                nFieldNDX = nFieldNDX + nStringlLength;

                                nMsg = new byte[nStringlLength];
                                Array.Copy(gzBuffer, nStartPos, nMsg, 0, nStringlLength);
                                oL1Quote.description = TD_GetResponseValue(0, nMsg, 0, nStringlLength);




                                /*/
                                 * Trade ID : Field 26
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(char)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);

                                nMsg = new byte[sizeof(char)];
                                Array.Copy(gzBuffer, nStartPos, nMsg, 0, sizeof(char));
                                oL1Quote.trade_id = TD_GetResponseValue(0, nMsg, 0, sizeof(char));



                                /*/
                                 * Digits : Field 27
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.digits = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                                /*/
                                 * Open : Field 28
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.open = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                                /*/
                                 * Change : Field 29
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.change = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                                /*/
                                 * 52-Week High : Field 30
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.week_high_52 = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                                /*/
                                 * 52-Week Low : Field 31
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.week_low_52 = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                                /*/
                                 * PE-Ratio : Field 32
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.pe_ratio = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                                /*/
                                 * Dividend Amount : Field 33
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.dividend_amt = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);




                                /*/
                                 * Dividend Yield : Field 34
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.dividend_yield = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                                /*/
                                 * ISLAND BID SIZE : Field 35
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.island_bid_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);




                                /*/
                                 * ISLAND ASK SIZE : Field 36
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.island_ask_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                                /*/
                                 * NAV : Field 37
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.nav = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);




                                /*/
                                 * FUND : Field 38
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                nStartPos = nFieldNDX;


                                nMsg = new byte[sizeof(float)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                oL1Quote.fund = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);




                                /*/
                                 * EXCHANGE NAME : Field 39
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(short)];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nStartPos = nFieldNDX;

                                nStringlLength = BitConverter.ToInt16(nMsg, 0);
                                nFieldNDX = nFieldNDX + nStringlLength;

                                nMsg = new byte[nStringlLength];
                                Array.Copy(gzBuffer, nStartPos, nMsg, 0, nStringlLength);
                                oL1Quote.exchange_name = TD_GetResponseValue(0, nMsg, 0, nStringlLength);



                                /*/
                                 * DIVIDEND DATE : Field 40
                                /*/

                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                nMsg = new byte[sizeof(short)];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nStartPos = nFieldNDX;

                                nStringlLength = BitConverter.ToInt16(nMsg, 0);
                                nFieldNDX = nFieldNDX + nStringlLength;

                                nMsg = new byte[nStringlLength];
                                Array.Copy(gzBuffer, nStartPos, nMsg, 0, nStringlLength);
                                oL1Quote.dividend_date = rs.TD_GetResponseValue(0, nMsg, 0, nStringlLength);


                                oL1Quotes.Add(oL1Quote);
                                rs.SendLevelOneDataSnapshot(oL1Quotes, rs.stockSymbol);


                            }
                        }

                    }
                }
                catch (Exception exc) { }
            }





            public void process_AsyncLevelOneStreaming(IAsyncResult asyncResult)
            {


                // Get the RequestState object from the asyncresult
                RequestState rs = (RequestState)asyncResult.AsyncState;


                if (rs.lNewStockSymbol == true)
                {
                    /*/ This tells use to return because the resources have been destroyed. /*/

                    rs.CloseStream(rs);
                    return;
                }

                try
                {

                    if (rs.Request.ServicePoint.CurrentConnections > 0)
                    {

                        // Pull out the ResponseStream that was set in RespCallback
                        Stream responseStream = rs.ResponseStream;

                        // At this point rs.BufferRead should have some data in it.
                        // Read will tell us if there is any data there


                        int read = responseStream.EndRead(asyncResult);


                        if (read == 0)
                        {
                            rs.CloseStream(rs);
                            responseStream.Close();
                            rs.oParent.TD_RequestAsyncLevel2Streaming(rs.stockSymbol, rs.Level2DataSourceType, rs.oParentForm);
                            rs.oParent = null;
                            return;
                        }


                        if (read > 0)
                        {

                            Array.Copy(rs.BufferRead, 0, ChartByteArray2, ChartByteArray2NDx, read);
                            ChartByteArray2NDx = ChartByteArray2NDx + read;


                            string compressedText1 = Convert.ToBase64String(ChartByteArray2);
                            byte[] gzBuffer1 = Convert.FromBase64String(compressedText1);
                            byte[] DataByteArray = new byte[read];
                            byte[] aTempStorage = new byte[read];

                            string mydata = string.Empty;
                            int nStartPos1 = 0;
                            bool lStartFound = false;

                            int nEndPos1 = 0;
                            bool lEndFound = false;
                            int ntotalMessageChunkLength = 0;

                            for (int nLoop = 0; nLoop < ChartByteArray2NDx; nLoop++)
                            {

                                /*/
                                 * Find the start of the streaming message.
                                /*/
                                if (gzBuffer1[nLoop] == 83 && lStartFound == false)
                                {
                                    nStartPos1 = nLoop;
                                    lStartFound = true;

                                }


                                /*/
                                 * Find the end of the streaming message. End of message delimeter = 255,10
                                /*/
                                if (gzBuffer1[nLoop] == 255 && gzBuffer1[nLoop + 1] == 10 && lEndFound == false && lStartFound == true)
                                {

                                    byte[] nTotMsg = new byte[sizeof(short)];
                                    nTotMsg[0] = gzBuffer1[nStartPos1 + 1];
                                    nTotMsg[1] = gzBuffer1[nStartPos1 + 2];
                                    Array.Reverse(nTotMsg);
                                    ntotalMessageChunkLength = BitConverter.ToInt16(nTotMsg, 0);


                                    nEndPos1 = nLoop;
                                    lEndFound = true;
                                    nStartPos1 = nStartPos1 + 5;
                                    //DataByteArray = new byte[(nEndPos1 - nStartPos1)];
                                    DataByteArray = new byte[ntotalMessageChunkLength - 2];
                                    Array.Copy(gzBuffer1, nStartPos1, DataByteArray, 0, ntotalMessageChunkLength - 4);


                                    long nRemaining = ChartByteArray2NDx - (ntotalMessageChunkLength + nStartPos1);

                                    if (nRemaining == 0)
                                    {
                                        /*/
                                         * Reset the primary collection array.
                                        /*/
                                        ChartByteArray2NDx = 0;
                                        ChartByteArray2 = new byte[BUFFER_SIZE];
                                    }
                                    else
                                    {
                                        /*/
                                         * Reposition the primary collection array by moving the remaining data toward the top of the collection array.
                                         * This will allow for unprocessed data received to get processed next in line and any new data will get place
                                         * at the bottom of the collection array where it will wait its place in line.
                                        /*/

                                        ChartByteArray2NDx = ChartByteArray2NDx - (ntotalMessageChunkLength + nStartPos1);
                                        aTempStorage = new byte[nRemaining];

                                        Array.Copy(gzBuffer1, (ntotalMessageChunkLength + nStartPos1), aTempStorage, 0, nRemaining);
                                        ChartByteArray2 = new byte[BUFFER_SIZE];
                                        Array.Copy(aTempStorage, 0, ChartByteArray2, 0, nRemaining);

                                    }

                                    break;
                                }
                            }




                            if (lEndFound == true)
                            {

                                bool lContinueParsing = true;


                                while (lContinueParsing == true)
                                {

                                    List<L1quotes> oL1Quotes = new List<L1quotes>();
                                    L1quotes oL1Quote = new L1quotes();

                                    string compressedText = Convert.ToBase64String(DataByteArray);
                                    byte[] gzBuffer = Convert.FromBase64String(compressedText);


                                    byte[] nMsg = null;
                                    int nFieldNumber = 0;
                                    int nFieldNDX = 0;
                                    int nStartPos = 0;
                                    short nStringlLength = 0;


                                    /*/
                                     * Stock Symbol : Field 0
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);


                                        if (nFieldNumber == 0)
                                        {
                                            nMsg = new byte[sizeof(short)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            nStringlLength = BitConverter.ToInt16(nMsg, 0);


                                            nMsg = new byte[nStringlLength];
                                            Array.Copy(gzBuffer, nFieldNDX, nMsg, 0, nStringlLength);
                                            oL1Quote.stock = rs.TD_GetResponseValue(0, nMsg, 0, nStringlLength);
                                        }
                                        else
                                            nFieldNDX--;
                                    }



                                    /*/
                                     * Bid Price : Field 1
                                    /*/

                                    nFieldNDX = nFieldNDX + nStringlLength;


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);


                                        if (nFieldNumber == 1)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.bid = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);
                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Ask Price : Field 2
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                        if (nFieldNumber == 2)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.ask = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Last Price : Field 3
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                        if (nFieldNumber == 3)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.last = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);
                                        }
                                        else
                                            nFieldNDX--;
                                    }



                                    /*/
                                     * Bid Size : Field 4
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                        if (nFieldNumber == 4)
                                        {
                                            nMsg = new byte[sizeof(Int32)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.bid_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);
                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Ask Size : Field 5
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                        if (nFieldNumber == 5)
                                        {
                                            nMsg = new byte[sizeof(Int32)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.ask_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);
                                        }
                                        else
                                            nFieldNDX--;

                                    }

                                    /*/
                                     * Bid ID : Field 6
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                        if (nFieldNumber == 6)
                                        {
                                            nMsg = new byte[sizeof(short)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.bid_id = TD_GetResponseValue(sizeof(short), nMsg, nStartPos, 0);
                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Ask ID : Field 7
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);


                                        if (nFieldNumber == 7)
                                        {
                                            nMsg = new byte[sizeof(short)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.ask_id = TD_GetResponseValue(sizeof(short), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Volume : Field 8
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                        if (nFieldNumber == 8)
                                        {
                                            nMsg = new byte[sizeof(long)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            nMsg[4] = gzBuffer[nFieldNDX++];
                                            nMsg[5] = gzBuffer[nFieldNDX++];
                                            nMsg[6] = gzBuffer[nFieldNDX++];
                                            nMsg[7] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.volume = TD_GetResponseValue(sizeof(long), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;
                                    }




                                    /*/
                                     * Last Size : Field 9
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);


                                        if (nFieldNumber == 9)
                                        {
                                            nMsg = new byte[sizeof(Int32)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.lastsize = TD_GetResponseValue(100, nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Trade Time : Field 10
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);


                                        if (nFieldNumber == 10)
                                        {
                                            nMsg = new byte[sizeof(Int32)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.tradetime = TD_GetResponseValue(100, nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Quote Time : Field 11
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                        if (nFieldNumber == 11)
                                        {
                                            nMsg = new byte[sizeof(Int32)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.quotetime = TD_GetResponseValue(100, nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;


                                    }


                                    /*/
                                     * High : Field 12
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);


                                        if (nFieldNumber == 12)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.high = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }



                                    /*/
                                     * Low : Field 13
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                        if (nFieldNumber == 13)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.low = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Tick : Field 14
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 14)
                                        {
                                            nMsg = new byte[sizeof(char)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);

                                            nMsg = new byte[sizeof(char)];
                                            Array.Copy(gzBuffer, nStartPos, nMsg, 0, sizeof(char));
                                            oL1Quote.tick = TD_GetResponseValue(0, nMsg, 0, sizeof(char));

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Close : Field 15
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 15)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.close = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Exchange : Field 16
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 16)
                                        {
                                            nMsg = new byte[sizeof(char)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);

                                            nMsg = new byte[sizeof(char)];
                                            Array.Copy(gzBuffer, nStartPos, nMsg, 0, sizeof(char));
                                            oL1Quote.exchange = TD_GetResponseValue(0, nMsg, 0, sizeof(char));

                                        }
                                        else
                                            nFieldNDX--;

                                    }



                                    /*/
                                     * Marginable : Field 17
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 17)
                                        {
                                            nMsg = new byte[sizeof(bool)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.marginable = TD_GetResponseValue(sizeof(bool), nMsg, 0, sizeof(char));

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Shortable : Field 18
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 18)
                                        {
                                            nMsg = new byte[sizeof(bool)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.shortable = TD_GetResponseValue(sizeof(bool), nMsg, 0, sizeof(char));

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * ISLAND BID : Field 19
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 19)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.islandbid = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);
                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * ISLAND ASK : Field 20
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 20)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.islandask = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * ISLAND ASK : Field 21
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 21)
                                        {
                                            nMsg = new byte[sizeof(long)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            nMsg[4] = gzBuffer[nFieldNDX++];
                                            nMsg[5] = gzBuffer[nFieldNDX++];
                                            nMsg[6] = gzBuffer[nFieldNDX++];
                                            nMsg[7] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.islandvolume = TD_GetResponseValue(sizeof(long), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;
                                    }



                                    /*/
                                     * QUOTE DATE : Field 22
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 22)
                                        {
                                            nMsg = new byte[sizeof(Int32)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.quotedate = TD_GetResponseValue(100, nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * QUOTE DATE : Field 23
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 23)
                                        {
                                            nMsg = new byte[sizeof(Int32)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.tradedate = TD_GetResponseValue(100, nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Volatility : Field 24
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 24)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.volatility = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Description : Field 25
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                                        if (nFieldNumber == 25)
                                        {
                                            nMsg = new byte[sizeof(short)];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nStartPos = nFieldNDX;

                                            nStringlLength = BitConverter.ToInt16(nMsg, 0);
                                            nFieldNDX = nFieldNDX + nStringlLength;

                                            nMsg = new byte[nStringlLength];
                                            Array.Copy(gzBuffer, nStartPos, nMsg, 0, nStringlLength);
                                            oL1Quote.description = TD_GetResponseValue(0, nMsg, 0, nStringlLength);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Trade ID : Field 26
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 26)
                                        {
                                            nMsg = new byte[sizeof(char)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);

                                            nMsg = new byte[sizeof(char)];
                                            Array.Copy(gzBuffer, nStartPos, nMsg, 0, sizeof(char));
                                            oL1Quote.trade_id = TD_GetResponseValue(0, nMsg, 0, sizeof(char));

                                        }
                                        else
                                            nFieldNDX--;

                                    }



                                    /*/
                                     * Digits : Field 27
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 27)
                                        {
                                            nMsg = new byte[sizeof(Int32)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.digits = TD_GetResponseValue(100, nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Open : Field 28
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 28)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.open = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Change : Field 29
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 29)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.change = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * 52-Week High : Field 30
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 30)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.week_high_52 = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * 52-Week Low : Field 31
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 31)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.week_low_52 = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;
                                    }



                                    /*/
                                     * PE-Ratio : Field 32
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 32)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.pe_ratio = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Dividend Amount : Field 33
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 33)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.dividend_amt = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * Dividend Yield : Field 34
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 34)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.dividend_yield = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * ISLAND BID SIZE : Field 35
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 35)
                                        {
                                            nMsg = new byte[sizeof(Int32)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.island_bid_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * ISLAND ASK SIZE : Field 36
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 36)
                                        {
                                            nMsg = new byte[sizeof(Int32)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.island_ask_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * NAV : Field 37
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 37)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.nav = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * FUND : Field 38
                                    /*/

                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                                        nStartPos = nFieldNDX;

                                        if (nFieldNumber == 38)
                                        {
                                            nMsg = new byte[sizeof(float)];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[2] = gzBuffer[nFieldNDX++];
                                            nMsg[3] = gzBuffer[nFieldNDX++];
                                            Array.Reverse(nMsg);
                                            oL1Quote.fund = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                                        }
                                        else
                                            nFieldNDX--;

                                    }


                                    /*/
                                     * EXCHANGE NAME : Field 39
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);


                                        if (nFieldNumber == 39)
                                        {
                                            nMsg = new byte[sizeof(short)];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nStartPos = nFieldNDX;

                                            nStringlLength = BitConverter.ToInt16(nMsg, 0);
                                            nFieldNDX = nFieldNDX + nStringlLength;

                                            nMsg = new byte[nStringlLength];
                                            Array.Copy(gzBuffer, nStartPos, nMsg, 0, nStringlLength);
                                            oL1Quote.exchange_name = TD_GetResponseValue(0, nMsg, 0, nStringlLength);

                                        }
                                        else
                                            nFieldNDX--;
                                    }



                                    /*/
                                     * DIVIDEND DATE : Field 40
                                    /*/


                                    if (nFieldNDX < gzBuffer.Length)
                                    {
                                        nMsg = new byte[sizeof(Int32)];
                                        nMsg[0] = gzBuffer[nFieldNDX++];
                                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);


                                        if (nFieldNumber == 40)
                                        {
                                            nMsg = new byte[sizeof(short)];
                                            nMsg[1] = gzBuffer[nFieldNDX++];
                                            nMsg[0] = gzBuffer[nFieldNDX++];
                                            nStartPos = nFieldNDX;

                                            nStringlLength = BitConverter.ToInt16(nMsg, 0);
                                            nFieldNDX = nFieldNDX + nStringlLength;

                                            nMsg = new byte[nStringlLength];
                                            Array.Copy(gzBuffer, nStartPos, nMsg, 0, nStringlLength);
                                            oL1Quote.dividend_date = rs.TD_GetResponseValue(0, nMsg, 0, nStringlLength);


                                        }
                                        else
                                            nFieldNDX--;
                                    }


                                    oL1Quotes.Add(oL1Quote);


                                    rs.SendLevelOneDataStreaming(oL1Quotes, rs.stockSymbol, rs.ServiceName);

                                    if (nFieldNDX >= gzBuffer.Length - 1)
                                    {
                                        lContinueParsing = false;
                                    }
                                }
                            }
                        }


                        //// Make another call so that we continue retrieving any all incoming data                                                           

                        if (rs.lNewStockSymbol == false)
                        {
                            try
                            {

                                IAsyncResult ar = responseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);

                            }
                            catch (Exception exc)
                            {
                                rs.CloseStream(rs);
                                responseStream.Close();

                                rs.oParent.TD_RequestAsyncLevel1QuoteStreaming(rs.stockSymbol, rs.ServiceName, rs.oParentForm);
                                rs.oParent = null;
                            }

                        }

                    }

                }
                catch (Exception exc)
                {

                    /*/ This is very important.  In case the remote server should drop our connection /*/
                    /*/ This will allow us to recreate the original request and begin reciving data.  /*/

                    if (rs.lNewStockSymbol == false)
                    {
                        rs.CloseStream(rs);

                        rs.oParent.TD_RequestAsyncLevel1QuoteStreaming(rs.stockSymbol, rs.ServiceName, rs.oParentForm);
                        rs.oParent = null;
                    }

                    return;
                }

            }





            public void process_AsyncLevelTwoStreaming(IAsyncResult asyncResult)
            {


                // Get the RequestState object from the asyncresult
                RequestState rs = (RequestState)asyncResult.AsyncState;


                if (rs.lNewStockSymbol == true)
                {
                    /*/ This tells use to return because the resources have been destroyed. /*/

                    rs.CloseStream(rs);
                    return;
                }

                try
                {

                    if (rs.Request.ServicePoint.CurrentConnections > 0)
                    {


                        // Pull out the ResponseStream that was set in RespCallback
                        Stream responseStream = rs.ResponseStream;

                        // At this point rs.BufferRead should have some data in it.
                        // Read will tell us if there is any data there


                        int read = responseStream.EndRead(asyncResult);

                        if (read == 0)
                        {
                            rs.CloseStream(rs);
                            responseStream.Close();
                            rs.oParent.TD_RequestAsyncLevel2Streaming(rs.stockSymbol, rs.Level2DataSourceType, rs.oParentForm);
                            rs.oParent = null;
                            return;
                        }


                        if (read > 36)
                        {

                            Level2ByteArray2NDx = 0;
                            Level2ByteArray2 = new byte[BUFFER_SIZE];


                            // Make sure we store all the incoming data until we reach the end.

                            Array.Copy(rs.BufferRead, 0, Level2ByteArray2, Level2ByteArray2NDx, read);
                            Level2ByteArray2NDx = Level2ByteArray2NDx + read;


                            string compressedText = Convert.ToBase64String(Level2ByteArray2);

                            byte[] gzBuffer = Convert.FromBase64String(compressedText);


                            MemoryStream ms = new MemoryStream();


                            // Find  the right position at which to start reading the actual streaming data //

                            int nStartPos = 0;
                            bool lStreamingDataFound = false;

                            if (gzBuffer[0] != 83)
                            {
                                for (int npos = 0; npos < gzBuffer.Length; npos++)
                                {

                                    if (gzBuffer[npos] == 83 && gzBuffer[npos - 1] == 10 && gzBuffer[npos - 2] == 255)
                                    {
                                        nStartPos = npos;
                                        lStreamingDataFound = true;
                                        break;
                                    }
                                }
                            }
                            else
                                lStreamingDataFound = true;



                            if (lStreamingDataFound == true)
                            {

                                int nFieldNDX = nStartPos;
                                int msgLength = BitConverter.ToInt32(gzBuffer, 0);

                                if (lStreamingDataFound == true)
                                {
                                    ms.Write(gzBuffer, 64, gzBuffer.Length - 64);
                                }
                                else
                                    ms.Write(gzBuffer, 0, gzBuffer.Length);



                                byte[] nMsg = new byte[sizeof(Int32)];


                                /*/
                                 * S = Streaming
                                 * N = Snapshot
                                /*/

                                nMsg[0] = gzBuffer[nFieldNDX++];
                                string cRequestType = System.Text.Encoding.ASCII.GetString(nMsg, 0, 1);



                                // Data Length //
                                nMsg = new byte[sizeof(short)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                Int16 nMessageLen = BitConverter.ToInt16(nMsg, 0);


                                /*/
                                 * 52 / 82 - NASDAQ Chart
                                 * 53 / 83 - NYSE Chart
                                 * 55 / 85 - Indices Chart
                                /*/

                                nMsg = new byte[sizeof(short)];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                int nSID = BitConverter.ToInt16(nMsg, 0);
                                string cStreamingRequestChart = string.Empty;
                                switch (nSID)
                                {

                                    case 8:
                                        cStreamingRequestChart = " NASDAQ Level 2 Book";
                                        break;

                                    case 60:
                                        cStreamingRequestChart = " INET Level 2 Book";
                                        break;

                                    case 81:
                                        cStreamingRequestChart = " NYSE Level 2 Book";
                                        break;

                                    case 84:
                                        cStreamingRequestChart = " Options Level 2 (Opra)";
                                        break;

                                }


                                // Col #00 - Symbol Length Data Field Column
                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                Int32 nCol00 = BitConverter.ToInt32(nMsg, 0);


                                // Get stock symbol length
                                nMsg = new byte[sizeof(short)];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                int nSymbolLength = BitConverter.ToInt16(nMsg, 0);


                                // Get stock symbol
                                nStartPos = nFieldNDX;
                                nMsg = new byte[nSymbolLength];
                                Array.Copy(gzBuffer, nStartPos, nMsg, 0, nSymbolLength);
                                string cSymbol = TD_GetResponseValue(0, nMsg, 0, nSymbolLength);


                                nFieldNDX = nFieldNDX + nSymbolLength;


                                // Col #01 - Time of Level 2 Field Column
                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                Int32 nCol01 = BitConverter.ToInt32(nMsg, 0);


                                // Get time of Level 2 Data Field Column
                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                Int32 nlevel2DataTime = BitConverter.ToInt32(nMsg, 0);



                                // Col #02 - Level 2 Data Field Column//
                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                Int32 nCol02 = BitConverter.ToInt32(nMsg, 0);



                                // Get length of compressed data
                                nMsg = new byte[sizeof(short)];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                int nTotalLenOfCompressedData = BitConverter.ToInt16(nMsg, 0);


                                ms.Close();


                                byte[] CompressedData = null;
                                string cTotalViewData = string.Empty;
                                StringBuilder cTempData = new StringBuilder();
                                int totalLength = 0;


                                //if (cSymbol.IndexOf(">L2") < 0)
                                //{
                                //    CompressedData = new byte[nTotalLenOfCompressedData];
                                //    Array.Copy(gzBuffer, nFieldNDX, CompressedData, 0, nTotalLenOfCompressedData);
                                //}
                                //else
                                //{


                                nStartPos = nFieldNDX;
                                nMsg = new byte[read - nFieldNDX];
                                Array.Copy(gzBuffer, nStartPos, nMsg, 0, (read - nFieldNDX));
                                cTotalViewData = TD_GetResponseValue(0, nMsg, 0, (read - nFieldNDX));
                                cTempData.Append(cTotalViewData);


                                //}


                                //byte[] writeData =null;
                                //Stream s2= null;

                                //if (cSymbol.IndexOf(">L2") < 0)
                                //{
                                //    writeData = new byte[BUFFER_SIZE];
                                //    s2 = new InflaterInputStream(new MemoryStream(CompressedData));
                                //}


                                try
                                {


                                    //    if (cSymbol.IndexOf(">L2") < 0)
                                    //    {
                                    //        while (true)
                                    //        {
                                    //            int size = s2.Read(writeData, 0, writeData.Length);
                                    //            if (size > 0)
                                    //            {
                                    //                totalLength += size;
                                    //                cTempData.Append(System.Text.Encoding.ASCII.GetString(writeData, 0, size));
                                    //            }
                                    //            else
                                    //            {
                                    //                break;
                                    //            }
                                    //        }

                                    //        s2.Close();
                                    //    }



                                    string cNumberString = "0123456789";
                                    string cCharString = ".;,abcdefghijklmnopqrstuvwxyz";

                                    int nBidRowsInBook = -1;
                                    int nBidTotal = -1;

                                    int nAskRowsInBook = -1;
                                    int nAskTotal = -1;

                                    int nElementCnt = 0;

                                    bool lProcessingBid = false;
                                    bool lProcessingAsk = false;
                                    bool lSymbolFound = false;


                                    string Level2RowString = string.Empty;
                                    int nstage = 0;

                                    string ctempval = "<bid>;";

                                    for (int len = 0; len < cTempData.Length; len++)
                                    {
                                        if (cNumberString.Contains(cTempData.ToString().Substring(len, 1)) == true || cCharString.Contains(cTempData.ToString().Substring(len, 1).ToLower()) == true)
                                        {
                                            ctempval = ctempval + cTempData.ToString().Substring(len, 1);
                                        }
                                        else
                                        {
                                            if (cTempData.ToString().Substring(len, 1).Contains("") == true)
                                            {
                                                ctempval = ctempval + (nstage == 0 ? "<ask>;" : "<bid>;");
                                                nstage = (nstage == 0 ? 1 : 0);
                                            }
                                        }

                                    }

                                    cTempData = new StringBuilder();
                                    cTempData.Append(ctempval + "\n");

                                    string[] cLines = cTempData.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (string cs in cLines)
                                    {


                                        if (cs.IndexOf("<bid>") >= 0 && lProcessingBid == false)
                                        {
                                            Level2RowString = string.Empty;
                                            nElementCnt = 0;
                                            lProcessingBid = true;
                                            lProcessingAsk = false;
                                            lSymbolFound = false;
                                        }
                                        else
                                        {
                                            if (cs.IndexOf("<ask>") >= 0 && lProcessingAsk == false)
                                            {
                                                Level2RowString = string.Empty;
                                                nElementCnt = 0;
                                                lProcessingBid = false;
                                                lProcessingAsk = true;
                                                lSymbolFound = false;
                                            }
                                        }


                                        if (lProcessingBid == true && cs.IndexOf("<bid>") < 0 && cs.IndexOf("<ask>") < 0)
                                        {
                                            if (nBidRowsInBook == -1 && lProcessingBid == true)
                                            {

                                                nBidRowsInBook = 0;

                                                if (nTotalBidOrders <= 0)
                                                {
                                                    nTotalBidOrders = Convert.ToInt32(cs);

                                                }

                                            }
                                            else
                                            {
                                                if (nBidTotal == -1 && lProcessingBid == true)
                                                {
                                                    nBidTotal = 0;
                                                    nTotalBidOrders = nTotalBidOrders - Convert.ToInt32(cs);
                                                }
                                                else
                                                {


                                                    if (lSymbolFound == true && cCharString.Contains(cs.Substring(0, 1).ToLower()) == true)
                                                    {
                                                        Level2RowString = string.Empty;
                                                        nElementCnt = 0;
                                                        lSymbolFound = false;
                                                    }
                                                    else
                                                    {
                                                        Level2RowString += cs.ToUpper() + ",";
                                                        nElementCnt++;

                                                        if (cCharString.Contains(cs.Substring(0, 1).ToLower()) == true)
                                                        {
                                                            lSymbolFound = true;
                                                        }
                                                    }


                                                    if (nElementCnt == 6)
                                                    {
                                                        int nSymPos = 0;
                                                        string[] cRow = Level2RowString.ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                                        Level2RowString = string.Empty;


                                                        for (nSymPos = 0; nSymPos < cRow.Length; nSymPos++)
                                                        {
                                                            if (cNumberString.Contains(cRow[nSymPos].Substring(0, 1)) == false)
                                                            {
                                                                break;
                                                            }
                                                        }

                                                        switch (nSymPos)
                                                        {
                                                            case 0:

                                                                Level2RowString = cRow[nSymPos].Trim().ToString() + "," + cRow[3].Trim().ToString() + "," + cRow[4].Trim().ToString() + ",";
                                                                cSortedBid.Add(Level2RowString);


                                                                break;

                                                            case 3:

                                                                Level2RowString = cRow[nSymPos].Trim().ToString() + "," + cRow[0].Trim().ToString() + "," + cRow[1].Trim().ToString() + ",";
                                                                cSortedBid.Add(Level2RowString);

                                                                break;

                                                            case 4:

                                                                Level2RowString = cRow[nSymPos].Trim().ToString() + "," + cRow[2].Trim().ToString() + "," + cRow[1].Trim().ToString() + ",";
                                                                cSortedBid.Add(Level2RowString);

                                                                break;
                                                        }

                                                        Level2RowString = string.Empty;
                                                        nElementCnt = 0;
                                                        lSymbolFound = false;
                                                    }


                                                }
                                            }
                                        }
                                        else
                                        {

                                            if (lProcessingAsk == true && cs.IndexOf("<bid>") < 0 && cs.IndexOf("<ask>") < 0)
                                            {

                                                if (nAskRowsInBook == -1 && lProcessingAsk == true)
                                                {
                                                    nAskRowsInBook = 0;

                                                    if (nTotalAskOrders <= 0)
                                                    {
                                                        nTotalAskOrders = Convert.ToInt32(cs);

                                                    }


                                                }
                                                else
                                                {
                                                    if (nAskTotal == -1 && lProcessingAsk == true)
                                                    {
                                                        nAskTotal = 0;
                                                        nTotalAskOrders = nTotalAskOrders - Convert.ToInt32(cs);

                                                    }
                                                    else
                                                    {


                                                        if (lSymbolFound == true && cCharString.Contains(cs.Substring(0, 1).ToLower()) == true)
                                                        {
                                                            Level2RowString = string.Empty;
                                                            nElementCnt = 0;
                                                            lSymbolFound = false;
                                                        }
                                                        else
                                                        {
                                                            Level2RowString += cs.ToUpper() + ",";
                                                            nElementCnt++;

                                                            if (cCharString.Contains(cs.Substring(0, 1).ToLower()) == true)
                                                            {
                                                                lSymbolFound = true;
                                                            }
                                                        }



                                                        if (nElementCnt == 6)
                                                        {

                                                            int nSymPos = 0;
                                                            string[] cRow = Level2RowString.ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                                            Level2RowString = string.Empty;

                                                            for (nSymPos = 0; nSymPos < cRow.Length; nSymPos++)
                                                            {
                                                                if (cNumberString.Contains(cRow[nSymPos].Substring(0, 1)) == false)
                                                                {
                                                                    break;
                                                                }
                                                            }


                                                            switch (nSymPos)
                                                            {
                                                                case 0:

                                                                    Level2RowString = cRow[nSymPos].Trim().ToString() + "," + cRow[3].Trim().ToString() + "," + cRow[4].Trim().ToString() + ",";
                                                                    cSortedAsk.Add(Level2RowString);

                                                                    break;

                                                                case 3:

                                                                    Level2RowString = cRow[nSymPos].Trim().ToString() + "," + cRow[0].Trim().ToString() + "," + cRow[1].Trim().ToString() + ",";
                                                                    cSortedAsk.Add(Level2RowString);

                                                                    break;

                                                                case 4:

                                                                    Level2RowString = cRow[nSymPos].Trim().ToString() + "," + cRow[2].Trim().ToString() + "," + cRow[1].Trim().ToString() + ",";

                                                                    cSortedAsk.Add(Level2RowString);
                                                                    break;
                                                            }


                                                            Level2RowString = string.Empty;
                                                            nElementCnt = 0;
                                                            lSymbolFound = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }


                                    //if (cSymbol.IndexOf(">L2") < 0)
                                    //{
                                    //    s2.Close();
                                    //    s2 = null;
                                    //}



                                    if (cSortedBid.Count >= nTotalBidOrders && cSortedAsk.Count >= nTotalAskOrders)
                                    {
                                        rs.SendLevel2Data(cSortedBid, cSortedAsk, rs.stockSymbol, rs.ServiceName);
                                    }


                                    cSortedBid = new List<string>();
                                    cSortedAsk = new List<string>();


                                    // Make another call so that we continue retrieving any all incoming data                                                                

                                    if (rs.lNewStockSymbol == false)
                                    {

                                        IAsyncResult ar = responseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);
                                    }


                                }
                                catch (Exception e)
                                {

                                    rs.CloseStream(rs);
                                    responseStream.Close();

                                    rs.oParent.TD_RequestAsyncLevel2Streaming(rs.stockSymbol, rs.Level2DataSourceType, rs.oParentForm);
                                    rs.oParent = null;


                                    //if (s2 != null)
                                    //{
                                    //    s2.Close();
                                    //}
                                }
                            }
                            else
                            {
                                if (rs.lNewStockSymbol == false)
                                {
                                    try
                                    {

                                        IAsyncResult ar = responseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);

                                    }
                                    catch (Exception exc)
                                    {
                                        rs.CloseStream(rs);
                                        responseStream.Close();

                                        rs.oParent.TD_RequestAsyncLevel2Streaming(rs.stockSymbol, rs.Level2DataSourceType, rs.oParentForm);
                                        rs.oParent = null;
                                    }
                                }

                            }

                        }
                        else
                        {
                            if (rs.lNewStockSymbol == false)
                            {
                                try
                                {

                                    IAsyncResult ar = responseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);


                                }
                                catch (Exception exc)
                                {
                                    rs.CloseStream(rs);
                                    responseStream.Close();

                                    rs.oParent.TD_RequestAsyncLevel2Streaming(rs.stockSymbol, rs.Level2DataSourceType, rs.oParentForm);
                                    rs.oParent = null;
                                }
                            }

                        }

                    }
                }
                catch (Exception exc)
                {

                    /*/ This is very important.  In case the remote server should drop our connection /*/
                    /*/ This will allow us to recreate the original request and begin reciving data.  /*/

                    if (rs.lNewStockSymbol == false)
                    {
                        rs.CloseStream(rs);

                        rs.oParent.TD_RequestAsyncLevel2Streaming(rs.stockSymbol, rs.Level2DataSourceType, rs.oParentForm);
                        rs.oParent = null;
                    }
                    return;
                }

            }



            public void process_AsyncNewsStreaming(IAsyncResult asyncResult)
            {

                // Get the RequestState object from the asyncresult
                RequestState rs = (RequestState)asyncResult.AsyncState;

                if (rs.Request.ServicePoint.CurrentConnections > 0)
                {


                    // Pull out the ResponseStream that was set in RespCallback
                    Stream responseStream = rs.ResponseStream;

                    // At this point rs.BufferRead should have some data in it.
                    // Read will tell us if there is any data there

                    int read = responseStream.EndRead(asyncResult);


                    if (read > 0)
                    {

                        rs.RequestData.Append(Encoding.ASCII.GetString(rs.BufferRead, 0, read));

                        // Make another call so that we continue retrieving any all incoming data                                    
                        IAsyncResult ar = responseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);

                    }
                    else
                    {

                        // Close the response stream since we do not have any more incoming data.

                        if (read == 0) { }
                    }
                }
            }




            public void process_AsyncOptionQuoteStreaming(IAsyncResult asyncResult)
            {

                // Get the RequestState object from the asyncresult
                RequestState rs = (RequestState)asyncResult.AsyncState;

                if (rs.Request.ServicePoint.CurrentConnections > 0)
                {


                    // Pull out the ResponseStream that was set in RespCallback
                    Stream responseStream = rs.ResponseStream;

                    // At this point rs.BufferRead should have some data in it.
                    // Read will tell us if there is any data there

                    int read = responseStream.EndRead(asyncResult);


                    if (read > 0)
                    {

                        rs.RequestData.Append(Encoding.ASCII.GetString(rs.BufferRead, 0, read));

                        // Make another call so that we continue retrieving any all incoming data                                    
                        IAsyncResult ar = responseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);

                    }
                    else
                    {

                        // Close the response stream since we do not have any more incoming data.                                                
                        if (read == 0) { }
                    }
                }
            }


            public void process_AsyncChartSnapshot(IAsyncResult asyncResult)
            {

                try
                {


                    // Get the RequestState object from the asyncresult
                    RequestState rs = (RequestState)asyncResult.AsyncState;

                    if (rs.lNewStockSymbol == true)
                    {
                        rs.CloseStream(rs);

                        return;

                    }


                    if (rs.Request.ServicePoint.CurrentConnections > 0)
                    {

                        List<String> cSortedLines = new List<string>();


                        // Pull out the ResponseStream that was set in RespCallback
                        Stream responseStream = rs.ResponseStream;

                        // At this point rs.BufferRead should have some data in it.
                        // Read will tell us if there is any data there

                        int read = responseStream.EndRead(asyncResult);


                        if (read > 0)
                        {
                            // Make sure we store all the incoming data until we reach the end.

                            Array.Copy(rs.BufferRead, 0, ChartByteArray2, ChartByteArray2NDx, read);
                            ChartByteArray2NDx = ChartByteArray2NDx + read;

                            // Make another call so that we continue retrieving any all incoming data                                    
                            IAsyncResult ar = responseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);

                        }
                        else
                        {

                            ChartByteArray2NDx = 0;

                            // If we have not more bytes read, then the server has finished sending us data.
                            if (read == 0)
                            {

                                string compressedText = Convert.ToBase64String(ChartByteArray2);
                                byte[] gzBuffer = Convert.FromBase64String(compressedText);


                                MemoryStream ms = new MemoryStream();


                                int nFieldNDX = 0;
                                int nStartPos = 21;
                                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                                ms.Write(gzBuffer, 64, gzBuffer.Length - 64);


                                byte[] nMsg = new byte[sizeof(Int32)];


                                /*/
                                 * S = Streaming
                                 * N = Snapshot
                                /*/

                                nMsg[0] = gzBuffer[nFieldNDX++];
                                string cRequestType = System.Text.Encoding.ASCII.GetString(nMsg, 0, 1);

                                // Skip these next 4 bytes
                                nFieldNDX = nFieldNDX + 4;


                                // Get message length
                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                string nTotalMessageLength = TD_GetResponseValue(100, nMsg, nStartPos, 0);


                                /*/
                                 * 52 / 82 - NASDAQ Chart
                                 * 53 / 83 - NYSE Chart
                                 * 55 / 85 - Indices Chart
                                /*/

                                nMsg = new byte[sizeof(short)];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                int nSID = BitConverter.ToInt16(nMsg, 0);
                                string cStreamingRequestChart = string.Empty;
                                switch (nSID)
                                {

                                    case 82:
                                        cStreamingRequestChart = " NASDAQ Chart";

                                        break;
                                    case 83:
                                        cStreamingRequestChart = " NYSE Chart";

                                        break;
                                    case 85:
                                        cStreamingRequestChart = " Indices Chart";

                                        break;
                                }


                                // Get stock symbol length
                                nMsg = new byte[sizeof(short)];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                int nSymbolLength = BitConverter.ToInt16(nMsg, 0);


                                // Get stock symbol
                                nStartPos = nFieldNDX;
                                nMsg = new byte[nSymbolLength];
                                Array.Copy(gzBuffer, nStartPos, nMsg, 0, nSymbolLength);
                                string cSymbol = TD_GetResponseValue(0, nMsg, 0, nSymbolLength);

                                nFieldNDX = nFieldNDX + nSymbolLength;


                                // Get status
                                nMsg = new byte[sizeof(short)];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                int nStatus = BitConverter.ToInt16(nMsg, 0);


                                // Get length of compressed data
                                nMsg = new byte[sizeof(Int32)];
                                nMsg[0] = gzBuffer[nFieldNDX++];
                                nMsg[1] = gzBuffer[nFieldNDX++];
                                nMsg[2] = gzBuffer[nFieldNDX++];
                                nMsg[3] = gzBuffer[nFieldNDX++];
                                Array.Reverse(nMsg);
                                string nTotalLenOfCompressedData = TD_GetResponseValue(100, nMsg, nStartPos, 0);


                                ms.Close();

                                byte[] CompressedData = new byte[Convert.ToInt32(nTotalLenOfCompressedData)];
                                Array.Copy(gzBuffer, nFieldNDX, CompressedData, 0, Convert.ToInt32(nTotalLenOfCompressedData));


                                StringBuilder cTempData = new StringBuilder();
                                int totalLength = 0;
                                byte[] writeData = new byte[BUFFER_SIZE];
                                Stream s2 = new InflaterInputStream(new MemoryStream(CompressedData));

                                try
                                {
                                    while (true)
                                    {
                                        int size = s2.Read(writeData, 0, writeData.Length);
                                        if (size > 0)
                                        {
                                            totalLength += size;
                                            cTempData.Append(System.Text.Encoding.ASCII.GetString(writeData, 0, size));
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    s2.Close();

                                    string[] cLines = cTempData.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (string cs in cLines)
                                    {
                                        cSortedLines.Add(cs);
                                    }

                                    rs.SendHistoricalChartData(cSortedLines, rs.stockSymbol, rs.ServiceName);

                                }
                                catch (Exception e)
                                {
                                    s2.Close();
                                }

                            }
                        }
                    }
                }
                catch (Exception exc) { }
            }


            public void process_AsyncChartStreaming(IAsyncResult asyncResult)
            {


                // Get the RequestState object from the asyncresult
                RequestState rs = (RequestState)asyncResult.AsyncState;



                if (rs.lNewStockSymbol == true)
                {
                    /*/ This tells use to return because the resources have been destroyed. /*/

                    rs.CloseStream(rs);

                    return;
                }

                try
                {

                    if (rs.Request.ServicePoint.CurrentConnections > 0)
                    {

                        // Pull out the ResponseStream that was set in RespCallback
                        Stream responseStream = rs.ResponseStream;

                        // At this point rs.BufferRead should have some data in it.
                        // Read will tell us if there is any data there

                        int read = responseStream.EndRead(asyncResult);

                        if (read == 0)
                        {
                            rs.CloseStream(rs);
                            responseStream.Close();
                            rs.oParent.TD_RequestAsyncLevel2Streaming(rs.stockSymbol, rs.Level2DataSourceType, rs.oParentForm);
                            rs.oParent = null;
                            return;
                        }

                        Int64 nElapsedTime = 0;


                        if (read > 0)
                        {

                            byte[] ChartByteArray = new byte[read];

                            // Convert byte stream to Char array and then String
                            // len shows how many characters are converted to Unicode

                            Array.Copy(rs.BufferRead, 0, ChartByteArray, 0, read);

                            string compressedText = Convert.ToBase64String(ChartByteArray);
                            byte[] gzBuffer = Convert.FromBase64String(compressedText);

                            int nStartOfData2Parse = 2;
                            int nFieldNDX = nStartOfData2Parse;

                            string incomingReply = System.Text.Encoding.ASCII.GetString(gzBuffer, 0, nStartOfData2Parse);


                            /*/
                             * Find the start of the Streaming message/data.
                            /*/

                            if (incomingReply.CompareTo("S\0") != 0)
                            {
                                for (int nPos = 0; nPos < gzBuffer.Length; nPos++)
                                {
                                    if ((nPos + 2) < gzBuffer.Length - 1)
                                    {
                                        string TestString = System.Text.Encoding.ASCII.GetString(gzBuffer, nPos, nStartOfData2Parse);
                                        if (TestString.CompareTo("S\0") == 0)
                                        {
                                            int nBytes2Copy = gzBuffer.Length - nPos;
                                            byte[] gzBufferTemp = new byte[nBytes2Copy];
                                            Array.Copy(gzBuffer, nPos, gzBufferTemp, 0, nBytes2Copy);

                                            gzBuffer = new byte[nBytes2Copy];
                                            Array.Copy(gzBufferTemp, 0, gzBuffer, 0, nBytes2Copy);

                                            incomingReply = System.Text.Encoding.ASCII.GetString(gzBuffer, 0, nStartOfData2Parse);

                                            break;
                                        }
                                    }
                                }

                            }

                            // Check to see if this is a streamer message and not a heartbeat time update message, with data

                            if (incomingReply.CompareTo("S") == 0)
                            {

                                nFieldNDX = 1;

                                while (incomingReply.CompareTo("?\n") != 0)
                                {

                                    int nStartPos = 0;

                                    // Data Length //
                                    byte[] nMsg = new byte[sizeof(short)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    nMsg[1] = gzBuffer[nFieldNDX++];
                                    Array.Reverse(nMsg);
                                    Int16 nMessageLen = BitConverter.ToInt16(nMsg, 0);


                                    // SID //
                                    nMsg = new byte[sizeof(short)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    nMsg[1] = gzBuffer[nFieldNDX++];
                                    Array.Reverse(nMsg);
                                    short nSID = BitConverter.ToInt16(nMsg, 0);



                                    // Col #00 - Symbol Length Column//
                                    nMsg = new byte[sizeof(Int32)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    Int32 nCol00 = BitConverter.ToInt32(nMsg, 0);


                                    // Symbol Length //
                                    nMsg = new byte[sizeof(short)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    nMsg[1] = gzBuffer[nFieldNDX++];
                                    Array.Reverse(nMsg);
                                    short SymbolLength = BitConverter.ToInt16(nMsg, 0);


                                    // Symbol String //
                                    string cSymbol = System.Text.Encoding.ASCII.GetString(gzBuffer, nFieldNDX, SymbolLength);
                                    nFieldNDX = nFieldNDX + SymbolLength;

                                    rs.RequestData.Append(cSymbol + ",");


                                    // Col #01 - Sequence Field ID Column//
                                    nMsg = new byte[sizeof(Int32)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    Int32 nCol01 = BitConverter.ToInt32(nMsg, 0);


                                    // Sequence Field //
                                    nMsg = new byte[sizeof(Int32)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    nMsg[1] = gzBuffer[nFieldNDX++];
                                    nMsg[2] = gzBuffer[nFieldNDX++];
                                    nMsg[3] = gzBuffer[nFieldNDX++];
                                    Array.Reverse(nMsg);
                                    Int32 nSequence = BitConverter.ToInt32(nMsg, 0);

                                    rs.RequestData.Append(nSequence.ToString() + ",");


                                    // Col #02 - Open Price Field ID Column//
                                    nMsg = new byte[sizeof(Int32)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    Int32 nCol02 = BitConverter.ToInt32(nMsg, 0);

                                    // Open Price
                                    nMsg = new byte[sizeof(float)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    nMsg[1] = gzBuffer[nFieldNDX++];
                                    nMsg[2] = gzBuffer[nFieldNDX++];
                                    nMsg[3] = gzBuffer[nFieldNDX++];
                                    Array.Reverse(nMsg);
                                    string nOpenPrice = rs.TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                    rs.RequestData.Append(nOpenPrice + ",");


                                    // Col #03 - High Price Field ID Column//
                                    nMsg = new byte[sizeof(Int32)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    Int32 nCol03 = BitConverter.ToInt32(nMsg, 0);

                                    // High Price
                                    nMsg = new byte[sizeof(float)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    nMsg[1] = gzBuffer[nFieldNDX++];
                                    nMsg[2] = gzBuffer[nFieldNDX++];
                                    nMsg[3] = gzBuffer[nFieldNDX++];
                                    Array.Reverse(nMsg);
                                    string nHighPrice = rs.TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                    rs.RequestData.Append(nHighPrice + ",");


                                    // Col #04 - Low Price Field ID Column//
                                    nMsg = new byte[sizeof(Int32)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    Int32 nCol04 = BitConverter.ToInt32(nMsg, 0);

                                    // Low Price
                                    nMsg = new byte[sizeof(float)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    nMsg[1] = gzBuffer[nFieldNDX++];
                                    nMsg[2] = gzBuffer[nFieldNDX++];
                                    nMsg[3] = gzBuffer[nFieldNDX++];
                                    Array.Reverse(nMsg);
                                    string nLowPrice = rs.TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                    rs.RequestData.Append(nLowPrice + ",");

                                    // Col #05 - Close Price Field ID Column//
                                    nMsg = new byte[sizeof(Int32)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    Int32 nCol05 = BitConverter.ToInt32(nMsg, 0);

                                    // Close Price
                                    nMsg = new byte[sizeof(float)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    nMsg[1] = gzBuffer[nFieldNDX++];
                                    nMsg[2] = gzBuffer[nFieldNDX++];
                                    nMsg[3] = gzBuffer[nFieldNDX++];
                                    Array.Reverse(nMsg);
                                    string nClosePrice = rs.TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);

                                    rs.RequestData.Append(nClosePrice + ",");


                                    // Col #06 - Volume Price Field ID Column//
                                    nMsg = new byte[sizeof(Int32)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    Int32 nCol06 = BitConverter.ToInt32(nMsg, 0);

                                    // Volume 
                                    nMsg = new byte[sizeof(long)];
                                    nMsg[0] = 0;
                                    nMsg[1] = 0;
                                    nMsg[2] = 0;
                                    nMsg[3] = 0;

                                    nMsg[4] = gzBuffer[nFieldNDX++];
                                    nMsg[5] = gzBuffer[nFieldNDX++];
                                    nMsg[6] = gzBuffer[nFieldNDX++];
                                    nMsg[7] = gzBuffer[nFieldNDX++];
                                    Array.Reverse(nMsg);
                                    string nVolume = rs.TD_GetResponseValue(sizeof(long), nMsg, nStartPos, 0);

                                    rs.RequestData.Append(nVolume + ",");


                                    // Col #07 - Time Field ID Column//
                                    nMsg = new byte[sizeof(Int32)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    Int32 nCol07 = BitConverter.ToInt32(nMsg, 0);

                                    // Time Price
                                    nMsg = new byte[sizeof(long)];
                                    nMsg[0] = 0;
                                    nMsg[1] = 0;
                                    nMsg[2] = 0;
                                    nMsg[3] = 0;

                                    nMsg[4] = gzBuffer[nFieldNDX++];
                                    nMsg[5] = gzBuffer[nFieldNDX++];
                                    nMsg[6] = gzBuffer[nFieldNDX++];
                                    nMsg[7] = gzBuffer[nFieldNDX++];
                                    Array.Reverse(nMsg);
                                    string nTime = rs.TD_GetResponseValue(sizeof(long), nMsg, nStartPos, 0);

                                    rs.RequestData.Append(nTime + ",");


                                    // Col #08 - Date Field ID Column//
                                    nMsg = new byte[sizeof(Int32)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    Int32 nCol08 = BitConverter.ToInt32(nMsg, 0);

                                    // Time Price
                                    nMsg = new byte[sizeof(long)];
                                    nMsg[0] = 0;
                                    nMsg[1] = 0;
                                    nMsg[2] = 0;
                                    nMsg[3] = 0;

                                    nMsg[4] = gzBuffer[nFieldNDX++];
                                    nMsg[5] = gzBuffer[nFieldNDX++];
                                    nMsg[6] = gzBuffer[nFieldNDX++];
                                    nMsg[7] = gzBuffer[nFieldNDX++];
                                    Array.Reverse(nMsg);
                                    string nDate = rs.TD_GetResponseValue(sizeof(long), nMsg, nStartPos, 0);

                                    rs.RequestData.Append(nDate + ",");

                                    // Look for the End Delimeter 
                                    incomingReply = System.Text.Encoding.ASCII.GetString(gzBuffer, nFieldNDX, nStartOfData2Parse);

                                }

                                if (rs.RequestData.Length > 0)
                                {
                                    // If we have actual streaming data, then send to the user interface.

                                    rs.SendHistoricalChartDataStreaming(rs.stockSymbol, rs.RequestData.ToString(), rs.ServiceName);
                                    rs.RequestData = new StringBuilder();

                                }

                            }


                            // Check to see if this is a heartbeat and time update message
                            if (incomingReply.CompareTo("HT") == 0)
                            {
                                if (read >= 10)
                                {
                                    // Get the timestamp of the heartbeat time update message
                                    byte[] nMsg = new byte[sizeof(long)];
                                    nMsg[0] = gzBuffer[nFieldNDX++];
                                    nMsg[1] = gzBuffer[nFieldNDX++];
                                    nMsg[2] = gzBuffer[nFieldNDX++];
                                    nMsg[3] = gzBuffer[nFieldNDX++];
                                    nMsg[4] = gzBuffer[nFieldNDX++];
                                    nMsg[5] = gzBuffer[nFieldNDX++];
                                    nMsg[6] = gzBuffer[nFieldNDX++];
                                    nMsg[7] = gzBuffer[nFieldNDX++];
                                    Array.Reverse(nMsg);
                                    nElapsedTime = BitConverter.ToInt64(nMsg, 0);

                                }

                            }
                            else
                            {

                                /*/
                                 * Check to see if the service is NOT available.
                                /*/
                                if (incomingReply.CompareTo("N\0") == 0)
                                {
                                    /*/
                                     * Old/alternative method of decoding byte data.
                                    /*/
                                    //char[] tmpChartArray = new char [read];                                
                                    //int len = rs.StreamDecode.GetChars(rs.BufferRead, 0, read, tmpChartArray, 0);

                                    //rs.SendData("Streaming " + Encoding.ASCII.GetString(rs.BufferRead, 21, read - 23).ToLower(), string.Empty);

                                    //rs.SendData("Streaming service temporarily not available.", string.Empty);

                                }

                            }
                        }

                    }


                    if (rs.Request.ServicePoint.CurrentConnections > 0 && rs.lNewStockSymbol == false)
                    {

                        Stream responseStream2 = rs.ResponseStream;
                        IAsyncResult ar = responseStream2.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);

                    }
                }
                catch (Exception exc)
                {

                    /*/ This is very important.  In case the remote server should drop our connection /*/
                    /*/ This will allow us to recreate the original request and begin reciving data.  /*/

                    if (rs.lNewStockSymbol == false)
                    {
                        rs.CloseStream(rs);
                        rs.oParent.TD_RequestAsyncChart_Streaming(rs.stockSymbol, rs.ServiceName, rs.oParentForm);
                        rs.oParent = null;
                    }
                    return;
                }

            }

        }




        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 65535 * 10;

        #endregion


        #region Sorting Section

        // Implements the manual sorting of items by columns.
        public class ListItemSorter : IComparer
        {
            private int col;
            private string SortDirection = "DESC";


            public ListItemSorter()
            {
                RequestState dd = new RequestState();

                col = 0;
            }

            public ListItemSorter(int column)
            {
                col = column;
            }

            public void setSortcol(int column)
            {
                if (column > -1)
                {
                    if (column == col && SortDirection.CompareTo("ASC") == 0)
                    {
                        SortDirection = "DESC";
                    }
                    else
                        SortDirection = "ASC";

                    col = column;
                }

            }

            public void setSortDirection(string cSort)
            {
                if (cSort.CompareTo("ASC") == 0)
                {
                    SortDirection = "ASC";
                }
                else
                {
                    if (cSort.CompareTo("DESC") == 0)
                    {
                        SortDirection = "DESC";
                    }
                }
            }

            public int getSortCol()
            {
                return col;
            }


            public int getSortstatus()
            {

                return col;
            }

            public int Compare(object x, object y)
            {
                int nRetValue = 1;

                if (col > -1)
                {
                    if (SortDirection.CompareTo("DESC") == 0)
                    {
                        nRetValue = String.Compare(((ListViewItem)y).SubItems[col].Text, ((ListViewItem)x).SubItems[col].Text);
                    }
                    else
                        nRetValue = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);

                }

                return nRetValue;
            }
        }

        #endregion





        public class ATradeArgument
        {


            public enum ATradeMessageType
            {
                NONE = 1,
                ATRADE_LOGIN_STATUS = 2,
                NEW_ALERT_ARRIVED = 3,
                STOCK_QUOTE_UPDATE = 4,
                STOCK_CHART_DATA_UPDATE = 5,
                STOCK_SYMBOL_INVALID = 6,
                LEVEL2_DATA_UPDATE = 7,
                ATTEMPT_DATA_LOGIN = 8,
                ATTEMPT_DATA_LOGIN_SUCCEEDED = 9,
                ATTEMPT_DATA_LOGIN_FAILED = 10,
                LEVEL1_DATA_UPDATE = 11,
                BROKER_LOGIN_STATUS = 12,
                BROKER_ENTER_TRADE = 13,
                BROKER_CANCEL_TRADE = 14,
                BROKER_MODIFY_TRADE = 15,
                BROKER_CANCEL_ALL_OPEN_ORDERS = 16,
                BROKER_EXIT_ALL_POSITIONS = 17,
                BROKER_STOCK_TRADE_STATUS_UPDATE = 18,
                BROKER_ACCOUNT_AND_POSITION_UPDATE = 19,
                BROKER_STOCK_TRADE_REPLY_MESSAGE = 20,
                BROKER_TRANSACTION_HISTORY_REQUEST = 21,
                BROKER_TRANSACTION_HISTORY_REPLY = 22,
                BROKER_TRANSACTION_HISTORY_ERROR = 23,
                REFRESH_LINKED_CONTROLS = 24,
                REQUEST_ASYNC_DATA = 25,
                ASYNC_RESULT_STOCK_FOUND = 26,
                ASYNC_RESULT_STOCK_NOT_FOUND = 27,
                LEVEL3_STOCK_QUOTE_REQUEST = 28,
                LEVEL3_STOCK_QUOTE_RESULTS = 29

            };


            public enum ATradeLoginStatusType
            {
                LOGIN_NOT_ATTEMPTED = 0,
                NOT_LOGGED_IN = 1,
                LOGIN_FAILED = 2,
                LOGIN_SUCCEEDED = 3
            }


            public enum ATradeTradeReportType
            {
                NONE = 0,
                DAY_TRADE = 1,
                TRADE_BASIS = 2,
                PROFIT_DISBURSEMENT = 3
            }

            public AmeritradeBrokerAPI.ATradeArgument.ATradeMessageType MessageType = ATradeMessageType.NONE;



            // ATrade Stock Alert Messages //
            public String nalertnumber = string.Empty;
            public String calerttype = string.Empty;
            public String centrytime = string.Empty;
            public String stocksymbol = string.Empty;
            public String description = string.Empty;
            public String alertprice = string.Empty;
            public String shares = string.Empty;
            public String elapsedhrs = string.Empty;
            public String elapsedmins = string.Empty;
            public String elapsedsecs = string.Empty;
            public bool is_Shortable = true;
            public DateTime cDateTime = DateTime.Today;
            public bool lOK2GetLevel3Data = false;
            public bool lRefreshLinkedControls = false;
            public transactionHistoryType transHistoryType;
            public string transactionFromDate = string.Empty;
            public string transactionToDate = string.Empty;
            public ATradeTradeReportType ReportType = ATradeTradeReportType.NONE;
            public bool lIncludeProfitDisburesement = false;
            public bool lCheckForSymbol = false;


            // Atrade Display Message //
            public String ResultsCode = string.Empty;
            public String DisplayMssg = string.Empty;
            public String FormMssg = string.Empty;
            public double nLastExecutedPrice = 0;
            public string BrokerUserName = string.Empty;
            public string BrokerPassword = string.Empty;
            public string BrokerAcctID = string.Empty;
            public AmeritradeBrokerAPI oBroker = new AmeritradeBrokerAPI();

            // Stock Quote Array //
            public AmeritradeBrokerAPI.quotes[] Stockquotevalues;
            public List<List<List<string>>> oHistoricalChartDataForLevel3Quotes = new List<List<List<string>>>();



            // Stock Trade Reply From Brokers //
            public struct ATradeChartData
            {
                public string cTime;
                public string cOpen;
                public string cHigh;
                public string cLow;
                public string cclose;
                public string cVolume;
                public string TA_01;

            };


            public struct ATradeLevel1Data
            {
                public string symbol;
                public string highToday;
                public string lowToday;
                public string lastExecutedPrice;
                public string totalVolume;
                public string percentChange;
                public string priceChange;
                public string exchange;
                public string PE;
                public string AvgVolume;
                public string High52Week;
                public string Low52Week;
                public string CalendarHigh;
                public string CalendarLow;
                public string EPS;
                public string CompanyName;
                public string Beta;
                public string Volatility;
                public string Bid;
                public string Ask;
                public string BidAskSize;
                public string PrevClosePrice;
                public string OpenPrice;
                public string isShortable;
            }

            public enum orderType
            {
                M_Market = 0,
                L_Limit = 1,
                S_Stop = 2,
                X_Stop_Limit = 3,
                D_Debit = 4,
                C_Credit = 5,
                T_Trailing_Stop = 6,
                EX_Exercise_Option = 7
            }

            public enum tradeType
            {
                Normal_Market_1 = 0,
                External_Hour_Market_2 = 1,
                German_Market_4 = 2,
                AM_Session_8 = 3,
                Seamless_Session_16 = 4

            }


            public enum orderActionType
            {
                B_Buy = 0,
                S_Sell = 1,
                SS_Short_Sell = 2,
                BC_Buy_to_Cover = 3,
                E_Exchange = 4,
                EX_Exercise_Option = 5
            }



            public enum orderStatus
            {
                OPEN = 0,
                FILLED = 1,
                EXPIRED = 2,
                PENDING = 3,
                PENDING_CANCEL = 4,
                CANCELED = 5,
                PENDING_REPLACE = 6,
                REPLACED = 7,
                REVIEW_RELEASE = 8
            }

            public struct tradeDetails
            {
                public string OrderID;
                public string OrderTimeInForce;
                public string cStockSymbol;
                public string OrderShares;
                public string OrderRouting;
                public string OrderPrice;
                public string OrderType;
                public string TradeType;

            }



            public struct tradeReplyDetails
            {
                public string OrderNumber;                  // Returned by the broker   
                public bool lIsCancelable;
                public bool lIsEditable;
                public bool lIsEnhancedOrder;
                public string EnhancedOrderType;
                public string DisplayStatus;                // This is the actual/current status of the order
                public string OrderRoutingStatus;
                public string OrderReceivedTime;
                public string OrderReportedTime;
                public string OrderSharesRemianing;         // This is the number of outstanding shares in this order;
                public string cStockSymbol;
                public string AssetType;                    // E-Equity/Stock, F-Mutual Fund, O-Option, B-Bond
                public string OrderShares;
                public string OrderID;
                public string Action;                       // This is the actual/current action
                public string TradeType;                    // This is the type of trade entered
                public string OrderType;                    // This is the type of order entered
                public string OrderPrice;
                public string OrderPriceFilled;             // This is the average price filled for this order
                public string OrderQuantityFilled;          // This is the total number of shares filled.
            }


            public class transactionHistoryReplyDetails
            {
                public string accountId;
                public string searchDateFrom;
                public string searchDateTo;
                public string orderDate;
                public string historyReqType;
                public string subType;
                public string buySellCode;
                public string assetType;
                public string symbol;
                public string description;
                public string cusip;
                public string price;
                public string quantity;
                public string transactionID;
                public string value;
                public string commission;
                public string orderNumber;
                public string fees;
                public string additionalFees;
                public string cashBalanceEffect;
                public string openClose;
                public string optionType;
                public string optionStrike;
                public string accruedInterest;
                public string parentchildIndicator;
                public string sharesBefore;
                public string sharesAfter;
                public string otherCharges;
                public string redemptionFee;
                public string cdscFee;
                public string bondInterestRate;
                public bool lStockMatched;

            }




            public class getExchangeTypes
            {

                public string getExchange(int nExchangeCode)
                {
                    string cretval = string.Empty;


                    switch (nExchangeCode)
                    {
                        case 1:
                            cretval = "NASDAQ National market";
                            break;
                        case 2:
                            cretval = "NASDAQ Small Cap";
                            break;
                        case 3:
                            cretval = "NASDAQ other OTC (PINKS)";
                            break;
                        case 4:
                            cretval = "NASDAQ OTCBB ";
                            break;
                        case 5:
                            cretval = "NASDAQ ";
                            break;
                        case 6:
                            cretval = "American Stock Exchange ";
                            break;
                        case 7:
                            cretval = "New York Stock Exchange ";
                            break;
                        case 8:
                            cretval = "Chicago Stock Exchange ";
                            break;
                        case 9:
                            cretval = "Philadelphia Stock Exchange ";
                            break;
                        case 10:
                            cretval = "Cincinnati Stock Exchange ";
                            break;
                        case 11:
                            cretval = "Pacific Stock Exchange ";
                            break;
                        case 12:
                            cretval = "Boston Stock Exchange ";
                            break;
                        case 13:
                            cretval = "Chicago Board Options Exchange ";
                            break;
                        case 14:
                            cretval = "OPRA System ";
                            break;
                        case 15:
                            cretval = "NASDAQ Alternate Display facility ";
                            break;
                        case 16:
                            cretval = "International Stock Exchange ";
                            break;
                        case 17:
                            cretval = "Boston Options Exchange ";
                            break;
                        case 20:
                            cretval = "Philadelphia Board of Trade ";
                            break;
                        case 27:
                            cretval = "DTN(Calculated/Index/Statistic) ";
                            break;
                        case 30:
                            cretval = "Chicago Board Of Trade ";
                            break;
                        case 31:
                            cretval = "Dow Jones (CBOT) ";
                            break;
                        case 32:
                            cretval = "CBOE Futures Exchange ";
                            break;
                        case 33:
                            cretval = "Kansas City Board Of Trade ";
                            break;
                        case 34:
                            cretval = "Chicago Mercantile Exchange ";
                            break;
                        case 35:
                            cretval = "Minneapolis Grain Exchange ";
                            break;
                        case 36:
                            cretval = "New York Mercantile Exchange ";
                            break;
                        case 37:
                            cretval = "Commodities Exchange Center ";
                            break;
                        case 38:
                            cretval = "New York Board Of Trade ";
                            break;
                        case 39:
                            cretval = "Cantor Financial Futures Exchange (deprecated) ";
                            break;
                        case 40:
                            cretval = "One Chicago ";
                            break;
                        case 41:
                            cretval = "NQLX (deprecated) ";
                            break;
                        case 42:
                            cretval = "Chicago Board Of Trade Mini Sized Contracts ";
                            break;
                        case 43:
                            cretval = "Chicago Mercantile Exchange Mini Sized Contracts ";
                            break;
                        case 44:
                            cretval = "EUREXUS ";
                            break;
                        case 45:
                            cretval = "New York Mercantile Exchange Mini Sized Contracts ";
                            break;
                        case 50:
                            cretval = "Toronto Stock Exchange ";
                            break;
                        case 51:
                            cretval = "Montreal Stock Exchange ";
                            break;
                        case 52:
                            cretval = "Canadian Venture Exchange ";
                            break;
                        case 53:
                            cretval = "Winnipeg Stock Exchange ";
                            break;
                        case 54:
                            cretval = "Winnipeg Commodities Exchange (WPG) ";
                            break;
                        case 60:
                            cretval = "Alliance / CBOT / EUREX ";
                            break;
                        case 61:
                            cretval = "London International Financial Futures Exchange ";
                            break;
                        case 62:
                            cretval = "London Metals Exchange ";
                            break;
                        case 63:
                            cretval = "International Petroleum Exchange ";
                            break;
                        case 64:
                            cretval = "Baltic ";
                            break;
                        case 65:
                            cretval = "Deutsche Terminbourse ";
                            break;
                        case 66:
                            cretval = "Paris:Marche a Terme International de France ";
                            break;
                        case 67:
                            cretval = "Singapore International Monetary Exchange ";
                            break;
                        case 68:
                            cretval = "European Exchange ";
                            break;
                        case 69:
                            cretval = "EURONEXT Index Derivatives ";
                            break;
                        case 70:
                            cretval = "EURONEXT Interest Rates ";
                            break;
                        case 71:
                            cretval = "EURONEXT Commodities ";
                            break;
                        case 72:
                            cretval = "Tullett Liberty ";
                            break;
                        case 73:
                            cretval = "Barclays Bank ";
                            break;
                        case 74:
                            cretval = "Hot Spot ";
                            break;
                        case 75:
                            cretval = "Warenterminborse Hannover ";
                            break;
                        case 76:
                            cretval = "EUREX Indexes ";
                            break;
                        default:
                            cretval = "";
                            break;

                    }

                    return cretval;
                }
            }


            public class ATradeLevel2Data
            {


                private string cndx;
                public string cNDX
                {
                    get { return cndx; }
                    set { cNDX = value; }
                }


                private double nbid;
                public double nBid
                {
                    get { return nbid; }
                    set { nBid = value; }
                }

                private double nask;
                public double nAsk
                {
                    get { return nask; }
                    set { nAsk = value; }
                }

                private string cbid;
                public string cBid
                {
                    get { return cbid; }
                    set { cBid = value; }
                }

                private string cask;
                public string cAsk
                {
                    get { return cask; }
                    set { cAsk = value; }
                }

                private string cbidSize;
                public string cBidSize
                {
                    get { return cbidSize; }
                    set { cBidSize = value; }
                }

                private string caskSize;
                public string cAskSize
                {
                    get { return caskSize; }
                    set { cAskSize = value; }
                }

                private string mmID;
                public string MMID
                {
                    get { return mmID; }
                    set { MMID = value; }
                }

                private string ctime;
                public string cTime
                {
                    get { return ctime; }
                    set { cTime = value; }
                }

                private string csymbol;
                public string cSymbol
                {
                    get { return csymbol; }
                    set { cSymbol = value; }
                }


                public ATradeLevel2Data(string lcBid, string lcAsk, string lcBidSize, string lcAskSize, string lMMID, string lctime, string lcsymbol, string lcNDX)
                {
                    this.cbid = lcBid;
                    this.cask = lcAsk;
                    this.cbidSize = lcBidSize;
                    this.caskSize = lcAskSize;
                    this.mmID = lMMID;
                    this.ctime = lctime;
                    this.csymbol = lcsymbol;

                    this.nbid = double.Parse(lcBid);
                    this.nask = double.Parse(lcAsk);
                    this.cndx = lcNDX;

                }
            }


            public string DTN_IP = string.Empty;
            public int DTN_Port = 0;
            public int DTN_Port2 = 0;


            public string ServiceName = string.Empty;
            public List<ATradeChartData> ChartDataArray = null;
            public List<ATradeLevel2Data> Level2DataArray = null;
            public List<ATradeLevel1Data> Level1DataArray = null;
            public List<CashBalances> oCashBalances = new List<CashBalances>();
            public List<Positions> oPositions = new List<Positions>();
            public List<tradeReplyDetails> oTradeHistory = new List<tradeReplyDetails>();
            public List<transactionHistoryReplyDetails> oTransactionHistory = new List<transactionHistoryReplyDetails>();
            public tradeDetails oStockTradeDetails = new tradeDetails();
            public List<List<string>> oHistoricalData = new List<List<string>>();
            public List<L1quotes> oLevelOneData = new List<L1quotes>();
            public List<string> oLevel2BidData = new List<string>();
            public List<string> oLevel2AskData = new List<string>();
            public RequestState.AsyncType FunctionType = RequestState.AsyncType.None;

        }



        [StructLayout(LayoutKind.Sequential)]
        public struct quotes
        {
            public string stock;
            public string exchange;
            public string bid;
            public string ask;
            public string last;
            public string change;
            public string change_pct;
            public string bid_ask_size;
            public string high;
            public string low;
            public string volume;
            public string description;
            public string stockQuoteNDX;
            public string isShortable;
            public string AlertType;
            public string AlertPrice;
            public string AlertShares;
            public string CurrentShares;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct L1quotes
        {
            public string stock;
            public string bid;
            public string ask;
            public string last;
            public string bid_size;
            public string ask_size;
            public string bid_id;
            public string ask_id;
            public string volume;
            public string lastsize;
            public string tradetime;
            public string quotetime;
            public string high;
            public string low;
            public string tick;
            public string close;
            public string exchange;
            public string marginable;
            public string shortable;
            public string islandbid;
            public string islandask;
            public string islandvolume;
            public string quotedate;
            public string tradedate;
            public string volatility;
            public string description;
            public string trade_id;
            public string digits;
            public string open;
            public string change;
            public string week_high_52;
            public string week_low_52;
            public string pe_ratio;
            public string dividend_amt;
            public string dividend_yield;
            public string island_bid_size;
            public string island_ask_size;
            public string nav;
            public string fund;
            public string exchange_name;
            public string dividend_date;

            public string stockQuoteNDX;
        }


        public enum orderHistoryType
        {
            ALL_ORDERS = 0,
            OPEN_ORDERS = 1,
            PENDING_ORDERS = 2,
            CANCELLED_ORDERS = 3,
            FILLED = 4,

        }


        public enum transactionHistoryType
        {
            ALL_TRANS = 0,
            TRADES = 1,
            BUY = 2,
            SELL = 3,
            DEPOSITS_AND_WITHDRAWALS = 4,
            CHECKING = 5,
            DIVIDENDS = 6,
            INTEREST = 7,
            OTHER = 8
        }


        public enum Level2DataSource
        {
            NYSE = 0,
            NASDAQ = 1,
            AMEX = 2,
            INET = 3

        }


        public bool loginStatus = false;
        public bool TD_loginStatus = false;
        private List<string> ShortableStockList = new List<string>();



        /*/
         * 
         * STOCK QUOTE TAG FIELDS
         * 
         * 
        /*/
        public string _symbol_tag = "symbol";
        public string _exchange_tag = "exchange";
        public string _bid_tag = "bid";
        public string _ask_tag = "ask";
        public string _last_tag = "last";
        public string _change_tag = "change";
        public string _change_pct_tag = "change-percent";
        public string _bid_ask_size_tag = "bid-ask-size";
        public string _high_tag = "high";
        public string _low_tag = "low";
        public string _volume_tag = "volume";
        public string _error_tag = "error";
        public string _quote_tag = "quote";
        public string _orderid_tag = "order-id";

        public string _symbol = string.Empty;
        public string _exchange = string.Empty;
        public string _bid = string.Empty;
        public string _ask = string.Empty;
        public string _last = string.Empty;
        public string _change = string.Empty;
        public string _change_pct = string.Empty;
        public string _bid_ask_size = string.Empty;
        public string _high = string.Empty;
        public string _low = string.Empty;
        public string _volume = string.Empty;
        public string _error = string.Empty;
        public string _quote = string.Empty;
        public string _orderid = string.Empty;


        /*/
         * 
         * 
         *  Positions and Balances
         * 
        /*/


        public struct CashBalances
        {
            public string InitialCashBalance;
            public string CurrentCashBalance;
            public string ChangeInCashBalance;
            public string DayTradingRoundTrips;
            public string StockBuyingPower;
            public string DayTradingBuyingPower;
            public string AvailableFundsForTrading;


        }

        public struct Positions
        {
            public string StockSymbol;
            public string Quantity;
            public string description;
            public string AssetType;
            public string AccountType;
            public string ClosePrice;
            public string PositionType;
            public string AveragePric;
            public string CurrentValue;

        }


        /*/
         * 
         * LOGIN TAG FIELDS
         * 
         * 
        /*/
        public string _result_tag = "RESULT";
        public string _sessionid_tag = "SESSION-ID";
        public string _userid_tag = "USER-ID";
        public string _cdi_tag = "CDI";
        public string _timeout_tag = "TIMEOUT";
        public string _associated_acct_tag = "ASSOCIATED-ACCOUNT-ID";
        public string _accountid_tag = "ACCOUNT-ID";
        public string _description_tag = "DESCRIPTION";
        public string _company_tag = "COMPANY";
        public string _segment_tag = "SEGMENT";
        public string _margintrading_tag = "MARGIN-TRADING";

        public string _result = string.Empty;
        public string _sessionid = string.Empty;
        public string _userid = string.Empty;
        public string _cdi = string.Empty;
        public string _timeout = string.Empty;
        public string _associated_acct = string.Empty;
        public string _accountid = string.Empty;
        public string _description = string.Empty;
        public string _company = string.Empty;
        public string _segment = string.Empty;
        public string _margintrading = string.Empty;




        /*/
         * 
         * GET STREAMER INFO TAG FIELDS
         * 
         * 
        /*/
        public string _token_tag = "TOKEN";
        public string _timestamp_tag = "TIMESTAMP";
        public string _cdDomain_tag = "CD-DOMAIN-ID";
        public string _usergroup_tag = "USERGROUP";
        public string _accesslevel_tag = "ACCESS-LEVEL";
        public string _acl_tag = "ACL";
        public string _appid_tag = "APP-ID";
        public string _authorized_tag = "AUTHORIZED";
        public string _errormsg_tag = "ERROR-MSG";
        public string _streamerurl_tag = "STREAMER-URL";



        public string _token = string.Empty;
        public string _timestamp = string.Empty;
        public string _cdDomain = string.Empty;
        public string _usergroup = string.Empty;
        public string _accesslevel = string.Empty;
        public string _acl = string.Empty;
        public string _appid = string.Empty;
        public string _authorized = string.Empty;
        public string _errormsg = string.Empty;
        public string _streamerurl = string.Empty;

        public List<string> MarketCol = new List<string>();


        public enum ChartUpdateType
        {
            INITIALIZE = 1,
            CHARTSTREAM_UPDATE = 2,
            LEVELONESTREAM_UPDATE = 3
        }



        public AmeritradeBrokerAPI()
        {
            MarketCol.Insert(0, "NYSE");
            MarketCol.Insert(1, "NASDAQ");
            MarketCol.Insert(2, "AMEX");
            MarketCol.Insert(3, "INET");

        }



        public string getLevel2SourceName(Level2DataSource Level2Source)
        {
            string cReplySource = string.Empty;

            switch (Level2Source)
            {
                case Level2DataSource.AMEX:
                    cReplySource = "NYSE_BOOK";
                    break;

                case Level2DataSource.NYSE:
                    cReplySource = "NYSE_BOOK";
                    break;

                case Level2DataSource.NASDAQ:
                    cReplySource = "LEVELII";
                    break;

                case Level2DataSource.INET:
                    cReplySource = "ADAP_INET";
                    break;

                default:
                    cReplySource = "NYSE_BOOK";
                    break;

            }

            return cReplySource;
        }



        public string getHTML(string cWebAddress)
        {
            string resultString = "";
            System.Net.WebClient webClient = new System.Net.WebClient();

            if (cWebAddress.Length > 0)
            {
                Stream webStream = webClient.OpenRead(cWebAddress);
                StreamReader webStreamReader = new StreamReader(webStream);
                resultString = webStreamReader.ReadToEnd();
                webStream.Close();

            }

            return resultString;

        }






        #region  TDAmeritrade API Code

        /*/
         * *****************************************************************************
         * TDAmeritrade API Code 
         *   
         *   Cedric Harris
         *   September 25, 2007
         * *****************************************************************************  
        /*/


        public bool TD_brokerLogin(string _userid, string _password, string _source, string _version)
        {
            bool retValue = false;
            XMLHTTP xmlHttp_ = new XMLHTTP();
            StringBuilder cpostdata = new StringBuilder();
            string lcPostUrl = string.Empty;

            cpostdata.Append("userid=" + Encode_URL(_userid));
            cpostdata.Append("&password=" + Encode_URL(_password));
            cpostdata.Append("&source=" + Encode_URL(_source));
            cpostdata.Append("&version=" + Encode_URL(_version));


            lcPostUrl = "https://apis.tdameritrade.com/apps/100/LogIn?source=" + Encode_URL(_source) + "&version=" + Encode_URL(_version);

            xmlHttp_.open("POST", lcPostUrl, false, _userid, _password);
            xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");

            xmlHttp_.send(cpostdata.ToString());


            string xmlData = xmlHttp_.responseText.ToString();
            string cResponseHeaders = xmlHttp_.getAllResponseHeaders();



            /*/
             * Assign Login values from the response string
             * 
            /*/


            StringReader reader = new StringReader(xmlData);
            XmlTextReader xml = new XmlTextReader(reader);


            // Check for errors.
            if (null == xmlData || "" == xmlData)
            {
                // Throw an exception.
                throw new Exception("Failed to connect, check username and password?");
            }

            // Read the Xml.
            while (xml.Read())
            {
                // Got an element?
                if (xml.NodeType == XmlNodeType.Element)
                {
                    #region Parse the Xml Element returned from TD Ameritrade
                    // Get this node.
                    string name = xml.Name;

                    // Get Result/Status
                    if (name.ToLower().ToString().CompareTo(_result_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        _result = xml.Value;
                    }

                    // Get Session ID
                    if (name.ToLower().ToString().CompareTo(_sessionid_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        _sessionid = xml.Value;
                    }

                    // Get USER ID
                    if (name.ToLower().ToString().CompareTo(_userid_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        _userid = xml.Value;
                    }

                    // Get CDI
                    if (name.ToLower().ToString().CompareTo(_cdi_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        _cdi = xml.Value;
                    }

                    // Get TimeOut
                    if (name.ToLower().ToString().CompareTo(_timeout_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        _timeout = xml.Value;
                    }

                    // Get Associated User Account
                    if (name.ToLower().ToString().CompareTo(_associated_acct_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        _associated_acct = xml.Value;
                    }

                    // Get Account ID
                    if (name.ToLower().ToString().CompareTo(_accountid_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        _accountid = xml.Value;
                    }

                    // Get Description
                    if (name.ToLower().ToString().CompareTo(_description_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        _description = xml.Value;
                    }

                    // Get Company
                    if (name.ToLower().ToString().CompareTo(_company_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        _company = xml.Value;
                    }

                    // Get Segment
                    if (name.ToLower().ToString().CompareTo(_segment_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        _segment = xml.Value;
                    }

                    // Get Margined Account
                    if (name.ToLower().ToString().CompareTo(_margintrading_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        _margintrading = xml.Value;
                    }
                    #endregion
                }

            }

            retValue = (_result.ToUpper().CompareTo("OK") >= 0 ? true : false);
            TD_loginStatus = retValue;

            return retValue;
        }



        public bool TD_GetStreamerInfo(string _userid, string _password, string _source, string _version)
        {

            bool retValue = false;

            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;

                cpostdata.Append("source=" + Encode_URL(_source));


                lcPostUrl = "https://apis.tdameritrade.com/apps/100/StreamerInfo?" + cpostdata.ToString();

                xmlHttp_.open("POST", lcPostUrl, false, _userid, _password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                string xmlData = xmlHttp_.responseText.ToString();
                string cResponseHeaders = xmlHttp_.getAllResponseHeaders();



                /*/
                 * Assign Login values from the response string
                 * 
                /*/


                StringReader reader = new StringReader(xmlData);
                XmlTextReader xml = new XmlTextReader(reader);


                // Check for errors.
                if (null == xmlData || "" == xmlData)
                {
                    // Throw an exception.
                    throw new Exception("Failed to connect, check username and password?");
                }

                // Read the Xml.
                while (xml.Read())
                {
                    // Got an element?
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        #region Parse the Xml Element returned from TD Ameritrade
                        // Get this node.
                        string name = xml.Name;

                        // Get Streamer URL
                        if (name.ToLower().ToString().CompareTo(_streamerurl_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _streamerurl = xml.Value;
                        }

                        // Get Token
                        if (name.ToLower().ToString().CompareTo(_token_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _token = xml.Value;
                        }

                        // Get TimeStamp
                        if (name.ToLower().ToString().CompareTo(_timestamp_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _timestamp = xml.Value;
                        }

                        // Get cdDomain
                        if (name.ToLower().ToString().CompareTo(_cdDomain_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _cdDomain = xml.Value;
                        }

                        // Get User Group
                        if (name.ToLower().ToString().CompareTo(_usergroup_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _usergroup = xml.Value;
                        }

                        // Get Access Level
                        if (name.ToLower().ToString().CompareTo(_accesslevel_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _accesslevel = xml.Value;
                        }

                        // Get ACL
                        if (name.ToLower().ToString().CompareTo(_acl_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _acl = xml.Value;
                        }

                        // Get Application ID
                        if (name.ToLower().ToString().CompareTo(_appid_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _appid = xml.Value;
                        }

                        // Get Authorized
                        if (name.ToLower().ToString().CompareTo(_authorized_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _authorized = xml.Value;
                        }

                        // Get Error Message
                        if (name.ToLower().ToString().CompareTo(_errormsg_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _errormsg = xml.Value;
                        }
                        #endregion
                    }

                }

                retValue = (_errormsg.Length == 0 ? true : false);
            }
            else
                retValue = false;

            return retValue;
        }

        
        public bool TD_KeepAlive(string _userid, string _password, string _source, string _version)
        {

            bool retValue = false;

            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;

                cpostdata.Append("source=" + Encode_URL(_source));

                lcPostUrl = "https://apis.tdameritrade.com/apps/KeepAlive?" + cpostdata.ToString();

                xmlHttp_.open("POST", lcPostUrl, false, _userid, _password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                retValue = true;

            }
            else
                retValue = false;

            return retValue;

        }


        public bool TD_Logout(string _userid, string _password, string _source, string _version)
        {
            bool retValue = false;

            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;

                cpostdata.Append("source=" + Encode_URL(_source));


                lcPostUrl = "https://apis.tdameritrade.com/apps/100/LogOut?" + cpostdata.ToString();

                xmlHttp_.open("POST", lcPostUrl, false, _userid, _password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                retValue = true;
            }
            else
                retValue = false;

            return retValue;

        }



        public string TD_sendOrder(string _userid, string _password, string _source, string _version, string _orderstring, ref string cResultValue, ref string cReplyOrderID)
        {

            _result = string.Empty;

            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;



                cpostdata.Append("source=" + Encode_URL(_source));
                cpostdata.Append("&orderstring=" + Encode_URL(_orderstring));

                lcPostUrl = "https://apis.tdameritrade.com/apps/100/EquityTrade?" + cpostdata.ToString();

                xmlHttp_.open("POST", lcPostUrl, false, _userid, _password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                string xmlData = xmlHttp_.responseText.ToString();
                string cResponseHeaders = xmlHttp_.getAllResponseHeaders();


                /*/
                 * Test Code : Use the following line to test the parsing of balances and positions
                 * 
                /*/

                StringReader reader = new StringReader(xmlData);
                XmlTextReader xml = new XmlTextReader(reader);


                // Check for errors.
                if (null == xmlData || "" == xmlData)
                {
                    // Throw an exception.
                    throw new Exception("Failed to connect, check username and password?");
                }

                string cParseStage = string.Empty;
                string name = string.Empty;


                // Read the Xml.
                while (xml.Read())
                {
                    // Got an element?
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        // Get this node.
                        name = xml.Name;

                        // Get Result/Status
                        if (name.ToLower().ToString().CompareTo(_result_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _result = xml.Value;

                        }


                        // Get Order ID
                        if (name.ToLower().ToString().CompareTo(_orderid_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _orderid = xml.Value;
                            cReplyOrderID = _orderid;

                        }


                        // Get Error Message
                        if (name.ToLower().ToString().CompareTo(_error_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _error = xml.Value;
                            cResultValue = _error;
                        }

                    }
                }
            }


            return _result;

        }



        public string TD_ModifyOrder(string _userid, string _password, string _source, string _version, string _orderstring, ref string cResultValue, ref string cReplyOrderID)
        {

            _result = string.Empty;

            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;


                cpostdata.Append("source=" + Encode_URL(_source));
                cpostdata.Append("&orderstring=" + Encode_URL(_orderstring));

                lcPostUrl = "https://apis.tdameritrade.com/apps/100/EditOrder?" + cpostdata.ToString();

                xmlHttp_.open("POST", lcPostUrl, false, _userid, _password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                string xmlData = xmlHttp_.responseText.ToString();
                string cResponseHeaders = xmlHttp_.getAllResponseHeaders();


                /*/
                 * Test Code : Use the following line to test the parsing of balances and positions
                 * 
                /*/

                StringReader reader = new StringReader(xmlData);
                XmlTextReader xml = new XmlTextReader(reader);


                // Check for errors.
                if (null == xmlData || "" == xmlData)
                {
                    // Throw an exception.
                    throw new Exception("Failed to connect, check username and password?");
                }

                string cParseStage = string.Empty;
                string name = string.Empty;


                // Read the Xml.
                while (xml.Read())
                {
                    // Got an element?
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        // Get this node.
                        name = xml.Name;

                        // Get Result/Status
                        if (name.ToLower().ToString().CompareTo(_result_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _result = xml.Value;

                        }


                        // Get Order ID
                        if (name.ToLower().ToString().CompareTo(_orderid_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _orderid = xml.Value;
                            cReplyOrderID = _orderid;

                        }


                        // Get Error Message
                        if (name.ToLower().ToString().CompareTo(_error_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _error = xml.Value;
                            cResultValue = _error;
                        }

                    }
                }
            }

            return _result;
        }



        public string TD_CancelOrder(string AccountID, string OrderID, string _userid, string _password, string _source, string _version, ref string cResultValue)
        {

            _result = string.Empty;

            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;


                cpostdata.Append("source=" + Encode_URL(_source));
                cpostdata.Append("&accountid=" + AccountID);
                cpostdata.Append("&orderid=" + OrderID);

                lcPostUrl = "https://apis.tdameritrade.com/apps/100/OrderCancel?" + cpostdata.ToString();

                xmlHttp_.open("POST", lcPostUrl, false, _userid, _password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                string xmlData = xmlHttp_.responseText.ToString();
                string cResponseHeaders = xmlHttp_.getAllResponseHeaders();


                /*/
                 * Test Code : Use the following line to test the parsing of balances and positions
                 * 
                /*/
                StringReader reader = new StringReader(xmlData);
                XmlTextReader xml = new XmlTextReader(reader);


                // Check for errors.
                if (null == xmlData || "" == xmlData)
                {
                    // Throw an exception.
                    throw new Exception("Failed to connect, check username and password?");
                }

                string cParseStage = string.Empty;
                string name = string.Empty;


                // Read the Xml.
                while (xml.Read())
                {
                    // Got an element?
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        // Get this node.
                        name = xml.Name;

                        // Get Result/Status
                        if (name.ToLower().ToString().CompareTo(_result_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _result = xml.Value;

                        }


                        // Get Error Message
                        if (name.ToLower().ToString().CompareTo(_error_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _error = xml.Value;
                            cResultValue = _error;
                        }

                    }
                }
            }

            return _result;
        }


        public bool TD_getTransactionHistory(string _userid, string _password, string _source, string _version, string _startDate, string _endDate, transactionHistoryType transHistoryType, ref List<AmeritradeBrokerAPI.ATradeArgument.transactionHistoryReplyDetails> oReplyDetails)
        {

            bool retValue = false;
            string lcPostUrl = string.Empty;

            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();

                cpostdata.Append("source=" + Encode_URL(_source));
                cpostdata.Append("&type=" + Encode_URL(Convert.ToString((int)transHistoryType)));

                if (_startDate.Length > 0 && _endDate.Length > 0)
                {
                    cpostdata.Append("&startdate=" + Encode_URL(_startDate));
                    cpostdata.Append("&enddate=" + Encode_URL(_endDate));
                }

                lcPostUrl = "https://apis.tdameritrade.com/apps/100/History?" + cpostdata.ToString();


                xmlHttp_.open("POST", lcPostUrl, false, _userid, _password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                string xmlData = xmlHttp_.responseText.ToString();
                string cResponseHeaders = xmlHttp_.getAllResponseHeaders();



                StringReader reader = new StringReader(xmlData);
                XmlTextReader xml = new XmlTextReader(reader);


                // Check for errors.
                if (null == xmlData || "" == xmlData)
                {
                    // Throw an exception.
                    throw new Exception("Failed to connect, check username and password?");
                }

                AmeritradeBrokerAPI.ATradeArgument.transactionHistoryReplyDetails _otransHistory = new ATradeArgument.transactionHistoryReplyDetails();

                string name = string.Empty;
                string _tAccountID = string.Empty;
                string _tStartDate = string.Empty;
                string _tEndDate = string.Empty;


                // Read the Xml.
                while (xml.Read())
                {
                    // Got an element?
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        // Get this node.
                        name = xml.Name;

                        if (name.ToLower().ToString().CompareTo("transaction-list") == 0)
                        {
                            _otransHistory = new ATradeArgument.transactionHistoryReplyDetails();

                        }


                        // Get account ID
                        if (name.ToLower().ToString().CompareTo("account-id") == 0)
                        {
                            xml.Read();
                            _tAccountID = xml.Value;

                        }

                        // Get start Date
                        if (name.ToLower().ToString().CompareTo("startdate") == 0)
                        {
                            xml.Read();
                            _tStartDate = xml.Value;

                        }


                        // Get end Date
                        if (name.ToLower().ToString().CompareTo("enddate") == 0)
                        {
                            xml.Read();
                            _tEndDate = xml.Value;

                        }


                        // Get orderDateTime
                        if (name.ToLower().ToString().CompareTo("orderdatetime") == 0 || name.ToLower().ToString().CompareTo("executeddate") == 0)
                        {
                            xml.Read();
                            _otransHistory.orderDate = xml.Value;

                        }

                        // Get type
                        if (name.ToLower().ToString().CompareTo("type") == 0)
                        {
                            xml.Read();
                            _otransHistory.historyReqType = xml.Value;

                        }

                        // Get subType
                        if (name.ToLower().ToString().CompareTo("subtype") == 0)
                        {
                            xml.Read();
                            _otransHistory.subType = xml.Value;

                        }


                        // Get buySellCode
                        if (name.ToLower().ToString().CompareTo("buysellcode") == 0)
                        {
                            xml.Read();
                            _otransHistory.buySellCode = xml.Value;

                        }


                        // Get assetType
                        if (name.ToLower().ToString().CompareTo("assettype") == 0)
                        {
                            xml.Read();
                            _otransHistory.assetType = xml.Value;

                        }


                        // Get symbol
                        if (name.ToLower().ToString().CompareTo("symbol") == 0)
                        {
                            xml.Read();
                            _otransHistory.symbol = xml.Value;

                        }


                        // Get description
                        if (name.ToLower().ToString().CompareTo("description") == 0)
                        {
                            xml.Read();
                            _otransHistory.description = xml.Value;

                        }


                        // Get cusip
                        if (name.ToLower().ToString().CompareTo("cusip") == 0)
                        {
                            xml.Read();
                            _otransHistory.cusip = xml.Value;

                        }


                        // Get price
                        if (name.ToLower().ToString().CompareTo("price") == 0)
                        {
                            xml.Read();
                            _otransHistory.price = xml.Value;

                        }


                        // Get quantity
                        if (name.ToLower().ToString().CompareTo("quantity") == 0)
                        {
                            xml.Read();
                            _otransHistory.quantity = xml.Value;
                            _otransHistory.quantity.Replace(",", "");

                        }


                        // Get transactionId
                        if (name.ToLower().ToString().CompareTo("transactionid") == 0)
                        {
                            xml.Read();
                            _otransHistory.transactionID = xml.Value;

                        }


                        // Get value
                        if (name.ToLower().ToString().CompareTo("value") == 0)
                        {
                            xml.Read();
                            _otransHistory.value = xml.Value;
                            _otransHistory.value.Replace(",", "");

                        }



                        // Get commission
                        if (name.ToLower().ToString().CompareTo("commission") == 0)
                        {
                            xml.Read();
                            _otransHistory.commission = xml.Value;
                            _otransHistory.commission.Replace(",", "");

                        }



                        // Get orderNumber
                        if (name.ToLower().ToString().CompareTo("ordernumber") == 0)
                        {
                            xml.Read();
                            _otransHistory.orderNumber = xml.Value;

                        }



                        // Get fees
                        if (name.ToLower().ToString().CompareTo("fees") == 0)
                        {
                            xml.Read();
                            _otransHistory.fees = (xml.Value.Length > 0 ? xml.Value : "0.00");
                            _otransHistory.fees.Replace(",", "");

                        }



                        // Get additionalFees
                        if (name.ToLower().ToString().CompareTo("additionalfees") == 0)
                        {
                            xml.Read();
                            _otransHistory.additionalFees = (xml.Value.Length > 0 ? xml.Value : "0.00");
                            _otransHistory.additionalFees.Replace(",", "");

                        }


                        // Get cashBalanceEffect
                        if (name.ToLower().ToString().CompareTo("cashbalanceeffect") == 0)
                        {
                            xml.Read();
                            _otransHistory.cashBalanceEffect = xml.Value;

                        }



                        // Get openClose
                        if (name.ToLower().ToString().CompareTo("openclose") == 0)
                        {
                            xml.Read();
                            _otransHistory.openClose = xml.Value;

                        }



                        // Get optionType
                        if (name.ToLower().ToString().CompareTo("optiontype") == 0)
                        {
                            xml.Read();
                            _otransHistory.optionType = xml.Value;

                        }



                        // Get optionStrike
                        if (name.ToLower().ToString().CompareTo("optionstrike") == 0)
                        {
                            xml.Read();
                            _otransHistory.optionStrike = xml.Value;

                        }


                        // Get accruedInterest
                        if (name.ToLower().ToString().CompareTo("accruedinterest") == 0)
                        {
                            xml.Read();
                            _otransHistory.accruedInterest = xml.Value;

                        }



                        // Get parentChildIndicator
                        if (name.ToLower().ToString().CompareTo("parentchildindicator") == 0)
                        {
                            xml.Read();
                            _otransHistory.parentchildIndicator = xml.Value;

                        }



                        // Get sharesBefore
                        if (name.ToLower().ToString().CompareTo("sharesbefore") == 0)
                        {
                            xml.Read();
                            _otransHistory.sharesBefore = xml.Value;

                        }



                        // Get sharesAfter
                        if (name.ToLower().ToString().CompareTo("sharesafter") == 0)
                        {
                            xml.Read();
                            _otransHistory.sharesAfter = xml.Value;

                        }


                        // Get otherCharges
                        if (name.ToLower().ToString().CompareTo("othercharges") == 0)
                        {
                            xml.Read();
                            _otransHistory.otherCharges = xml.Value;
                            _otransHistory.otherCharges.Replace(",", "");

                        }



                        // Get redemptionFee
                        if (name.ToLower().ToString().CompareTo("redemptionfee") == 0)
                        {
                            xml.Read();
                            _otransHistory.redemptionFee = xml.Value;

                        }


                        // Get cdscFee
                        if (name.ToLower().ToString().CompareTo("cdscfee") == 0)
                        {
                            xml.Read();
                            _otransHistory.cdscFee = xml.Value;

                        }


                        // Get bondInterestRate
                        if (name.ToLower().ToString().CompareTo("bondInterestrate") == 0)
                        {
                            xml.Read();
                            _otransHistory.bondInterestRate = xml.Value;

                        }



                        // Get Error Message
                        if (name.ToLower().ToString().CompareTo(_errormsg_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _errormsg = xml.Value;
                        }
                    }
                    else
                    {
                        if (xml.NodeType == XmlNodeType.EndElement)
                        {

                            if (xml.Name.ToString().ToLower().CompareTo("transaction-list") == 0)
                            {

                                _otransHistory.accountId = _tAccountID;
                                _otransHistory.searchDateFrom = _tStartDate;
                                _otransHistory.searchDateTo = _tEndDate;

                                oReplyDetails.Add(_otransHistory);

                            }

                        }
                    }
                }

                retValue = (_errormsg.Length == 0 ? true : false);
            }
            else
                retValue = false;

            return retValue;

        }





        /*/
        * 
        * This method is used to make the same call as TD_getOrderStatus()
        * but with the filter type for either : 
        *                                       ALL_ORDER          , 
        *                                       CANCELLED_ORDERS   , 
        *                                       OPEN_ORDERS        , 
        *                                       PENDING_ORDERS 
        * 
       /*/


        public void TD_getOrderStatusAndHistory(string _userid, string _password, string _source, string _version, string orderList, orderHistoryType OrderHistoryType, ref List<AmeritradeBrokerAPI.ATradeArgument.tradeReplyDetails> oReplyDetails, string _startDate, string _endDate)
        {

            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;

                cpostdata.Append("source=" + Encode_URL(_source));


                switch (OrderHistoryType)
                {
                    case orderHistoryType.ALL_ORDERS:

                        cpostdata.Append("&type=" + Encode_URL("all"));
                        cpostdata.Append("&days=" + Encode_URL("0"));

                        lcPostUrl = "https://apis.tdameritrade.com/apps/100/OrderStatus?" + cpostdata.ToString();

                        break;


                    case orderHistoryType.OPEN_ORDERS:

                        cpostdata.Append("&orderid=" + Encode_URL(orderList));
                        cpostdata.Append("&type=" + Encode_URL("open"));
                        cpostdata.Append("&days=" + Encode_URL("0"));

                        lcPostUrl = "https://apis.tdameritrade.com/apps/100/OrderStatus?source?" + cpostdata.ToString();

                        break;


                    case orderHistoryType.PENDING_ORDERS:

                        cpostdata.Append("&orderid=" + Encode_URL(orderList));
                        cpostdata.Append("&type=" + Encode_URL("pending"));
                        cpostdata.Append("&days=" + Encode_URL("0"));

                        lcPostUrl = "https://apis.tdameritrade.com/apps/100/OrderStatus?source?" + cpostdata.ToString();

                        break;


                    case orderHistoryType.FILLED:

                        cpostdata.Append("&orderid=" + Encode_URL(orderList));
                        cpostdata.Append("&type=" + Encode_URL("filled"));

                        if (_startDate.Length > 0 && _endDate.Length > 0)
                        {
                            cpostdata.Append("&startdate=" + Encode_URL(_startDate));
                            cpostdata.Append("&enddate=" + Encode_URL(_endDate));
                        }
                        else
                            cpostdata.Append("&days=" + Encode_URL("0"));


                        lcPostUrl = "https://apis.tdameritrade.com/apps/100/OrderStatus?source?" + cpostdata.ToString();

                        break;


                    case orderHistoryType.CANCELLED_ORDERS:

                        cpostdata.Append("&orderid=" + Encode_URL(orderList));
                        cpostdata.Append("&type=" + Encode_URL("canceled"));
                        cpostdata.Append("&days=" + Encode_URL("0"));

                        lcPostUrl = "https://apis.tdameritrade.com/apps/100/OrderStatus?source?" + cpostdata.ToString();

                        break;


                }

                xmlHttp_.open("POST", lcPostUrl, false, _userid, _password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                string xmlData = xmlHttp_.responseText.ToString();
                string cResponseHeaders = xmlHttp_.getAllResponseHeaders();


                /*/
                 * Test Code : Use the following line to test the parsing of balances and positions
                 * 
                /*/

                StringReader reader = new StringReader(xmlData);
                XmlTextReader xml = new XmlTextReader(reader);


                // Check for errors.
                if (null == xmlData || "" == xmlData)
                {
                    // Throw an exception.
                    throw new Exception("Failed to connect, check username and password?");
                }


                string[,] oFillDetails = new string[1, 2];
                List<string[,]> oFillDetailsList = new List<string[,]>();
                ATradeArgument.tradeReplyDetails oTradeHistoryDetails = new ATradeArgument.tradeReplyDetails();
                string name = string.Empty;


                while (xml.Read())
                {
                    // Got an element?
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        // Get this node.
                        name = xml.Name;

                        // Initialize the start of the collection 
                        if (name.ToLower().ToString().CompareTo("orderstatus") == 0)
                        {
                            oTradeHistoryDetails = new ATradeArgument.tradeReplyDetails();
                            oFillDetailsList = new List<string[,]>();

                        }


                        // Get order-number
                        if (name.ToLower().ToString().CompareTo("order-number") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderNumber = xml.Value;
                        }


                        // Get cancelable
                        if (name.ToLower().ToString().CompareTo("cancelable") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.lIsCancelable = Convert.ToBoolean(xml.Value);
                        }

                        // Get editable
                        if (name.ToLower().ToString().CompareTo("editable") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.lIsEditable = Convert.ToBoolean(xml.Value);
                        }

                        // Get enhanced-order
                        if (name.ToLower().ToString().CompareTo("enhanced-order") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.lIsEnhancedOrder = Convert.ToBoolean(xml.Value);
                        }

                        // Get EnhancedOrderType
                        if (name.ToLower().ToString().CompareTo("enhanced-type") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.EnhancedOrderType = xml.Value;
                        }

                        // Get display-status
                        if (name.ToLower().ToString().CompareTo("display-status") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.DisplayStatus = xml.Value;
                        }

                        // Get order-routing-status
                        if (name.ToLower().ToString().CompareTo("order-routing-status") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderRoutingStatus = xml.Value;
                        }

                        // Get order-received-date-time
                        if (name.ToLower().ToString().CompareTo("order-received-date-time") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderReceivedTime = xml.Value;
                        }

                        // Get reported-time
                        if (name.ToLower().ToString().CompareTo("reported-time") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderReportedTime = xml.Value;
                        }


                        // Get remaining-quantity
                        if (name.ToLower().ToString().CompareTo("remaining-quantity") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderSharesRemianing = xml.Value;
                        }

                        // Get symbol
                        if (name.ToLower().ToString().CompareTo("symbol") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.cStockSymbol = xml.Value;
                        }

                        // Get asset-type
                        if (name.ToLower().ToString().CompareTo("asset-type") == 0)
                        {
                            xml.Read();
                            if (xml.Value.ToUpper().CompareTo("E") == 0)
                                oTradeHistoryDetails.AssetType = "Equity";

                            if (xml.Value.ToUpper().CompareTo("F") == 0)
                                oTradeHistoryDetails.AssetType = "Mutual Fund";

                            if (xml.Value.ToUpper().CompareTo("O") == 0)
                                oTradeHistoryDetails.AssetType = "Option";

                            if (xml.Value.ToUpper().CompareTo("B") == 0)
                                oTradeHistoryDetails.AssetType = "Bond";


                        }

                        // Get quantity
                        if (name.ToLower().ToString().CompareTo("quantity") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderShares = xml.Value;
                        }

                        // Get order-id
                        if (name.ToLower().ToString().CompareTo("order-id") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderID = xml.Value;
                        }

                        // Get action
                        if (name.ToLower().ToString().CompareTo("action") == 0)
                        {
                            xml.Read();
                            if (xml.Value.ToUpper().CompareTo("B") == 0)
                                oTradeHistoryDetails.Action = "Buy Long";

                            if (xml.Value.ToUpper().CompareTo("S") == 0)
                                oTradeHistoryDetails.Action = "Sell";

                            if (xml.Value.ToUpper().CompareTo("SS") == 0)
                                oTradeHistoryDetails.Action = "Sell Short";

                            if (xml.Value.ToUpper().CompareTo("BC") == 0)
                                oTradeHistoryDetails.Action = "Buy to Cover";


                        }

                        // Get trade-type
                        if (name.ToLower().ToString().CompareTo("trade-type") == 0)
                        {
                            xml.Read();
                            if (xml.Value.CompareTo("1") == 0)
                                oTradeHistoryDetails.TradeType = "Normal Market";

                            if (xml.Value.CompareTo("2") == 0)
                                oTradeHistoryDetails.TradeType = "External Hour Market";

                            if (xml.Value.CompareTo("4") == 0)
                                oTradeHistoryDetails.TradeType = "German Market";

                            if (xml.Value.CompareTo("8") == 0)
                                oTradeHistoryDetails.TradeType = "AM Session";

                            if (xml.Value.CompareTo("16") == 0)
                                oTradeHistoryDetails.TradeType = "Seamless Session";

                        }

                        // Get order-type
                        if (name.ToLower().ToString().CompareTo("order-type") == 0)
                        {
                            xml.Read();
                            if (xml.Value.ToUpper().CompareTo("M") == 0)
                                oTradeHistoryDetails.OrderType = "Market";

                            if (xml.Value.ToUpper().CompareTo("L") == 0)
                                oTradeHistoryDetails.OrderType = "Limit";

                            if (xml.Value.ToUpper().CompareTo("S") == 0)
                                oTradeHistoryDetails.OrderType = "Stop";

                            if (xml.Value.ToUpper().CompareTo("X") == 0)
                                oTradeHistoryDetails.OrderType = "Stop Limit";

                            if (xml.Value.ToUpper().CompareTo("D") == 0)
                                oTradeHistoryDetails.OrderType = "Debit";

                            if (xml.Value.ToUpper().CompareTo("C") == 0)
                                oTradeHistoryDetails.OrderType = "Credit";

                            if (xml.Value.ToUpper().CompareTo("T") == 0)
                                oTradeHistoryDetails.OrderType = "Trailing Stop";

                            if (xml.Value.ToUpper().CompareTo("EX") == 0)
                                oTradeHistoryDetails.OrderType = "Exercise Option";


                        }


                        // Get Fill(s) - Quantity
                        if (name.ToLower().ToString().CompareTo("fill-quantity") == 0)
                        {
                            xml.Read();
                            oFillDetails = new string[1, 2];
                            oFillDetails[0, 0] = xml.Value;

                        }


                        // Get Fill(s) - Price
                        if (name.ToLower().ToString().CompareTo("fill-price") == 0)
                        {
                            xml.Read();
                            oFillDetails[0, 1] = xml.Value;
                            oFillDetailsList.Add(oFillDetails);

                        }



                        // Get limit-price
                        if (name.ToLower().ToString().CompareTo("limit-price") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderPrice = xml.Value;
                        }

                    }
                    else
                    {
                        if (xml.NodeType == XmlNodeType.EndElement)
                        {

                            if (xml.Name.ToString().ToLower().CompareTo("orderstatus") == 0)
                            {
                                Decimal nTotalDollarsExecuted = 0.00M;
                                Int32 nTotalSharesExecuted = 0;

                                if (oFillDetailsList.Count > 0)
                                {
                                    foreach (string[,] oDetail in oFillDetailsList)
                                    {
                                        nTotalSharesExecuted = nTotalSharesExecuted + Convert.ToInt32(Decimal.Round(Convert.ToDecimal(oDetail[0, 0]), 0));
                                        nTotalDollarsExecuted = nTotalDollarsExecuted + Convert.ToDecimal(Decimal.Round(Convert.ToDecimal(oDetail[0, 0]), 0) * Convert.ToDecimal(oDetail[0, 1]));

                                    }
                                    oTradeHistoryDetails.OrderPriceFilled = Convert.ToString(Decimal.Round(nTotalDollarsExecuted / nTotalSharesExecuted, 2));
                                    oTradeHistoryDetails.OrderQuantityFilled = Convert.ToString(nTotalSharesExecuted);
                                }
                                else
                                {
                                    oTradeHistoryDetails.OrderPriceFilled = oTradeHistoryDetails.OrderPrice;
                                    oTradeHistoryDetails.OrderQuantityFilled = oTradeHistoryDetails.OrderShares;

                                }

                                oReplyDetails.Add(oTradeHistoryDetails);
                            }
                        }
                    }
                }

            }

        }





        public void TD_getAcctBalancesAndPositions(string _userid, string _password, string _source, string _version, ref List<CashBalances> oCashBalances, ref List<Positions> oPositions)
        {

            bool retValue = false;
            string lcPostUrl = string.Empty;

            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();


                cpostdata.Append("source=" + Encode_URL(_source));

                lcPostUrl = "https://apis.tdameritrade.com/apps/100/BalancesAndPositions?" + cpostdata.ToString();


                xmlHttp_.open("POST", lcPostUrl, false, _userid, _password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                string xmlData = xmlHttp_.responseText.ToString();
                string cResponseHeaders = xmlHttp_.getAllResponseHeaders();


                /*/
                 * Test Code : Use the following line to test the parsing of balances and positions
                 * 
                /*/
                // StringReader reader = new StringReader( getTestBalancesAndPositions() );

                StringReader reader = new StringReader(xmlData);
                XmlTextReader xml = new XmlTextReader(reader);


                // Check for errors.
                if (null == xmlData || "" == xmlData)
                {
                    // Throw an exception.
                    throw new Exception("Failed to connect, check username and password?");
                }

                CashBalances _oCashBalances = new CashBalances();
                Positions _oPositions = new Positions();
                string cParseStage = string.Empty;
                string name = string.Empty;
                bool lReadingPositionsElement = false;



                // Read the Xml.
                while (xml.Read())
                {
                    // Got an element?
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        // Get this node.
                        name = xml.Name;

                        if (name.ToLower().ToString().CompareTo("balance") == 0)
                        {
                            cParseStage = "balances";
                            _oCashBalances = new CashBalances();

                        }


                        if (name.ToLower().ToString().CompareTo("account-value") == 0)
                        {
                            cParseStage = "account-value";
                        }


                        if (name.ToLower().ToString().CompareTo("position") == 0 && lReadingPositionsElement == false)
                        {
                            cParseStage = "positions";
                            lReadingPositionsElement = true;
                            _oPositions = new Positions();
                        }



                        // Get Round Trips
                        if (name.ToLower().ToString().CompareTo("round-trips") == 0) // && cParseStage.CompareTo("balances") == 0)
                        {
                            xml.Read();
                            _oCashBalances.DayTradingRoundTrips = xml.Value;

                        }

                        // Get Stock Buying Power
                        if (name.ToLower().ToString().CompareTo("stock-buying-power") == 0)// && cParseStage.CompareTo("balances") == 0)
                        {
                            xml.Read();
                            _oCashBalances.StockBuyingPower = xml.Value;

                        }

                        // Get DayTrading Buying Power
                        if (name.ToLower().ToString().CompareTo("day-trading-buying-power") == 0) //&& cParseStage.CompareTo("balances") == 0)
                        {
                            xml.Read();
                            _oCashBalances.DayTradingBuyingPower = xml.Value;

                        }

                        // Get Available Funds For Trading
                        if (name.ToLower().ToString().CompareTo("available-funds-for-trading") == 0) // && cParseStage.CompareTo("balances") == 0)
                        {
                            xml.Read();
                            _oCashBalances.AvailableFundsForTrading = xml.Value;

                        }



                        // Get Initial Cash Balance
                        if (name.ToLower().ToString().CompareTo("initial") == 0 && cParseStage.CompareTo("account-value") == 0)
                        {
                            xml.Read();
                            _oCashBalances.InitialCashBalance = xml.Value;
                        }

                        // Get Current Cash Balance
                        if (name.ToLower().ToString().CompareTo("current") == 0 && cParseStage.CompareTo("account-value") == 0)
                        {
                            xml.Read();
                            _oCashBalances.CurrentCashBalance = xml.Value;
                        }

                        // Get Change In Cash Balance
                        if (name.ToLower().ToString().CompareTo("change") == 0 && cParseStage.CompareTo("account-value") == 0)
                        {
                            xml.Read();
                            _oCashBalances.ChangeInCashBalance = xml.Value;
                        }


                        // Get Positions - Symbol
                        if (name.ToLower().ToString().CompareTo("symbol") == 0 && lReadingPositionsElement == true)
                        {
                            xml.Read();
                            _oPositions.StockSymbol = xml.Value;
                        }


                        // Get Positions - Quantity
                        if (name.ToLower().ToString().CompareTo("quantity") == 0 && lReadingPositionsElement == true)
                        {
                            xml.Read();
                            _oPositions.Quantity = xml.Value;
                        }


                        // Get Positions - Description
                        if (name.ToLower().ToString().CompareTo("description") == 0 && lReadingPositionsElement == true)
                        {
                            xml.Read();
                            _oPositions.description = xml.Value;
                        }


                        // Get Positions - Account Type
                        if (name.ToLower().ToString().CompareTo("account-type") == 0 && lReadingPositionsElement == true)
                        {
                            xml.Read();
                            _oPositions.AccountType = xml.Value;
                        }


                        // Get Positions - Asset Type
                        if (name.ToLower().ToString().CompareTo("asset-type") == 0 && lReadingPositionsElement == true)
                        {
                            xml.Read();
                            _oPositions.AssetType = xml.Value;
                        }

                        // Get Positions - Closing Price
                        if (name.ToLower().ToString().CompareTo("close-price") == 0 && lReadingPositionsElement == true)
                        {
                            xml.Read();
                            _oPositions.ClosePrice = xml.Value;
                        }

                        // Get Positions - Position Type
                        if (name.ToLower().ToString().CompareTo("position-type") == 0 && lReadingPositionsElement == true)
                        {
                            xml.Read();
                            _oPositions.PositionType = xml.Value;
                        }

                        // Get Positions - Average Price
                        if (name.ToLower().ToString().CompareTo("average-price") == 0 && lReadingPositionsElement == true)
                        {
                            xml.Read();
                            _oPositions.AveragePric = xml.Value;
                        }


                        // Get Positions - Current Value
                        if (name.ToLower().ToString().CompareTo("current-value") == 0 && lReadingPositionsElement == true)
                        {
                            xml.Read();
                            _oPositions.CurrentValue = xml.Value;

                        }



                        // Get Error Message
                        if (name.ToLower().ToString().CompareTo(_errormsg_tag.ToLower()) == 0)
                        {
                            xml.Read();
                            _errormsg = xml.Value;
                        }
                    }
                    else
                    {
                        if (xml.NodeType == XmlNodeType.EndElement)
                        {

                            if (xml.Name.ToString().ToLower().CompareTo("balance") == 0)
                            {
                                oCashBalances.Add(_oCashBalances);
                            }


                            if (xml.Name.ToString().ToLower().CompareTo("position") == 0)
                            {
                                lReadingPositionsElement = false;
                                oPositions.Add(_oPositions);
                            }


                            if (cParseStage.CompareTo(xml.Name.ToString().ToLower()) == 0)
                            {
                                cParseStage = string.Empty;
                            }

                        }
                    }
                }

                retValue = (_errormsg.Length == 0 ? true : false);
            }
            else
                retValue = false;
        }




        public bool TD_IsShortable(string symbol)
        {

            L1quotes oL1Quote = TD_GetLevel1Quote(symbol.ToUpper(), 0);

            return Convert.ToBoolean(oL1Quote.shortable);

        }


        public bool TD_IsShortableCustom(string symbol)
        {

            L1quotes oL1Quote = new L1quotes();

            while (true)
            {
                oL1Quote = TD_GetLevel1Quote2(symbol.ToUpper(), 0);
                if (oL1Quote.shortable != null)
                {
                    break;
                }
            }
            return Convert.ToBoolean(oL1Quote.shortable);

        }



        public Int32 GetNumberType(string NumberNameType)
        {
            Int32 oType = 1;

            switch (NumberNameType)
            {
                case "LONG":

                    oType = sizeof(Int32);

                    break;

            }

            return oType;
        }




        public string TD_GetResponseValue(Int32 valueType, byte[] value, Int32 nStart, Int32 nLength)
        {
            string replyvalue = string.Empty;

            switch (valueType)
            {
                // Strings or Characters
                case 0:

                    if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("U") == 0)
                        replyvalue = "UP";
                    else
                        if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("D") == 0)
                            replyvalue = "UP";
                        else
                            if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("\0 ") == 0)
                                replyvalue = "UNCH";
                            else
                                if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("\0n") == 0)
                                    replyvalue = "NYSE";
                                else
                                    if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("\0q") == 0)
                                        replyvalue = "NASDAQ";
                                    else
                                        if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("\0u") == 0)
                                            replyvalue = "OTCBB";
                                        else
                                            if (System.Text.Encoding.ASCII.GetString(value, nStart, nLength).CompareTo("\0p") == 0)
                                                replyvalue = "PACX";
                                            else
                                                replyvalue = System.Text.Encoding.ASCII.GetString(value, nStart, nLength);


                    break;

                case 1:
                    bool lBoolValue = BitConverter.ToBoolean(value, 0);
                    replyvalue = lBoolValue.ToString();
                    break;


                case 2:
                    Int16 nCharValue = BitConverter.ToInt16(value, 0);
                    replyvalue = nCharValue.ToString();
                    break;


                // Float or Integer 32 // 4 bytes
                case 4:

                    float nFloat_ShortValue = BitConverter.ToSingle(value, 0);
                    replyvalue = nFloat_ShortValue.ToString();
                    break;

                // Long // 8 bytes
                case 8:
                    long nLongValue = BitConverter.ToInt64(value, 0);
                    replyvalue = nLongValue.ToString();
                    break;

                default:
                    Int32 nDefaultValue = BitConverter.ToInt32(value, 0);
                    replyvalue = nDefaultValue.ToString();
                    break;

            }
            return replyvalue;

        }


        public string GetChartServiceString(string xExchange)
        {
            string cReplyString = xExchange;

            if (xExchange.Substring(0, 1).CompareTo("$") != 0)
            {
                switch (xExchange)
                {

                    case "NYSE":
                        cReplyString = "NYSE";
                        break;

                    case "NASDAQ":
                        cReplyString = "NASDAQ";
                        break;

                    case "INET":
                        cReplyString = "ADAP_INET";
                        break;

                    case "AMEX":
                        cReplyString = "NYSE";
                        break;

                    case "OTCBB":
                        cReplyString = "NASDAQ";
                        break;

                    case "\09":
                        cReplyString = "NASDAQ";
                        break;

                    default:
                        cReplyString = "NYSE";
                        break;

                }
            }

            return cReplyString;

        }




        #region Encoding Strings

        public string Encode_URL(string cUrlString)
        {

            StringBuilder encodedString = new StringBuilder();
            char[] encBytes = cUrlString.ToCharArray();

            foreach (char cb in encBytes)
            {
                switch ((byte)cb)
                {
                    case 58:
                        encodedString.Append("%3A");
                        break;

                    case 32:
                        encodedString.Append("%20");
                        break;

                    case 40:
                        encodedString.Append("%28");
                        break;

                    case 41:
                        encodedString.Append("%29");
                        break;

                    case 43:

                        encodedString.Append("%2B");
                        break;

                    case 45:
                        encodedString.Append("%2D");
                        break;

                    case 61:
                        encodedString.Append("%3D");
                        break;

                    case 124:
                        encodedString.Append("%7C");
                        break;

                    case 38:
                        encodedString.Append("%26");
                        break;

                    case 44:
                        encodedString.Append("%2C");
                        break;

                    case 126:
                        encodedString.Append("%7E");
                        break;

                    default:
                        encodedString.Append(cb);
                        break;
                }
            }

            return encodedString.ToString();
        }

        #endregion


        #endregion







        #region Latest Asynchronous Code - 05/28/2008


        public void TD_RequestAsyncChart_Snapshot(string _streamStockSymbol, string chartSource, int nDays, Form oForm)
        {

            List<String> cSortedLines = new List<string>();

            try
            {
                if (chartSource.IndexOf("NYSE_CHART") == 0 || chartSource.IndexOf("NASDAQ_CHART") == 0 || chartSource.IndexOf("IINDEX_CHART") == 0)
                {

                    string _streamerCommand = "S=" + chartSource.ToUpper() + "&C=GET&P=" + _streamStockSymbol.ToUpper() + ",0,610," + nDays.ToString() + "d,1m";

                    if (this.TD_loginStatus == true)
                    {

                        XMLHTTP xmlHttp_ = new XMLHTTP();
                        StringBuilder cpostdata = new StringBuilder();
                        string lcPostUrl = string.Empty;

                        cpostdata.Append("!U=" + _accountid);
                        cpostdata.Append("&W=" + _token);
                        cpostdata.Append("&A=userid=" + _accountid);
                        cpostdata.Append("&token=" + _token);
                        cpostdata.Append("&company=" + _company);
                        cpostdata.Append("&segment=" + _segment);
                        cpostdata.Append("&cddomain=" + _cdDomain);
                        cpostdata.Append("&usergroup=" + _usergroup);
                        cpostdata.Append("&accesslevel=" + _accesslevel);
                        cpostdata.Append("&authorized=" + _authorized);
                        cpostdata.Append("&acl=" + _acl);
                        cpostdata.Append("&timestamp=" + _timestamp);
                        cpostdata.Append("&appid=" + _appid);
                        cpostdata.Append("|" + _streamerCommand);

                        string encodedString = Encode_URL(cpostdata.ToString());
                        cpostdata = new StringBuilder();
                        cpostdata.Append(encodedString);

                        lcPostUrl = "http://" + this._streamerurl + "/" + cpostdata;


                        /*/
                         *   Read the response and decompress the chart data
                         * 
                        /*/


                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(lcPostUrl);
                        req.ContentType = "application/x-www-form-urlencoded";
                        req.Accept = "Accept image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*";
                        req.Method = "GET";
                        req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        req.Timeout = 60000;
                        req.ServicePoint.ConnectionLimit = 50;

                        //// Add the request into the state so it can be passed around

                        this.rs_ChartSnapShot = new RequestState();

                        //this.rs_ChartSnapShot.TickWithArgs += new AmeritradeBrokerAPI.EventHandlerWithArgs(((Form1)oForm).processEvent);
                        this.rs_ChartSnapShot.oParentForm = oForm;
                        this.rs_ChartSnapShot.lNewStockSymbol = false;


                        this.rs_ChartSnapShot.ServiceName = chartSource;
                        this.rs_ChartSnapShot.stockSymbol = _streamStockSymbol;
                        this.rs_ChartSnapShot.Request = req;
                        this.rs_ChartSnapShot.FunctionType = RequestState.AsyncType.ChartSnapshot;

                        // Issue the async request                
                        IAsyncResult r = (IAsyncResult)req.BeginGetResponse(new AsyncCallback(RespCallback), this.rs_ChartSnapShot);

                    }
                }
            }
            catch (Exception exce) { }
        }






        public List<String> TD_Request_NonAsyncChart_Snapshot(string _streamStockSymbol, string chartSource, int nDays)
        {

            List<String> cSortedLines = new List<string>();

            try
            {
                string _streamerCommand = "S=" + chartSource.ToUpper() + "&C=GET&P=" + _streamStockSymbol.ToUpper() + ",0,610," + nDays.ToString() + "d,1m";

                if (this.TD_loginStatus == true)
                {

                    XMLHTTP xmlHttp_ = new XMLHTTP();
                    StringBuilder cpostdata = new StringBuilder();
                    string lcPostUrl = string.Empty;

                    cpostdata.Append("!U=" + _accountid);
                    cpostdata.Append("&W=" + _token);
                    cpostdata.Append("&A=userid=" + _accountid);
                    cpostdata.Append("&token=" + _token);
                    cpostdata.Append("&company=" + _company);
                    cpostdata.Append("&segment=" + _segment);
                    cpostdata.Append("&cddomain=" + _cdDomain);
                    cpostdata.Append("&usergroup=" + _usergroup);
                    cpostdata.Append("&accesslevel=" + _accesslevel);
                    cpostdata.Append("&authorized=" + _authorized);
                    cpostdata.Append("&acl=" + _acl);
                    cpostdata.Append("&timestamp=" + _timestamp);
                    cpostdata.Append("&appid=" + _appid);
                    cpostdata.Append("|" + _streamerCommand);

                    string encodedString = Encode_URL(cpostdata.ToString());
                    cpostdata = new StringBuilder();
                    cpostdata.Append(encodedString);

                    lcPostUrl = "http://" + this._streamerurl + "/" + cpostdata;


                    /*/
                     *   Read the response and decompress the chart data
                     * 
                    /*/


                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(lcPostUrl);
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.Accept = "Accept image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*";
                    req.Method = "GET";
                    req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    req.Timeout = 60000;
                    req.ServicePoint.ConnectionLimit = 50;



                    //req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                    Stream respStream = resp.GetResponseStream();


                    byte[] chunk = new byte[65535 * 10];
                    byte[] ChartByteArray = new byte[65535 * 10];

                    int bytesRead = respStream.Read(ChartByteArray, 0, ChartByteArray.Length);
                    string compressedText = Convert.ToBase64String(ChartByteArray);
                    byte[] gzBuffer = Convert.FromBase64String(compressedText);

                    respStream.Flush();
                    resp.Close();
                    respStream.Close();


                    MemoryStream ms = new MemoryStream();


                    int nFieldNDX = 0;
                    int nStartPos = 21;
                    int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                    ms.Write(gzBuffer, 64, gzBuffer.Length - 64);


                    byte[] nMsg = new byte[sizeof(Int32)];


                    /*/
                     * S = Streaming
                     * N = Snapshot
                    /*/

                    nMsg[0] = gzBuffer[nFieldNDX++];
                    string cRequestType = System.Text.Encoding.ASCII.GetString(nMsg, 0, 1);

                    // Skip these next 4 bytes
                    nFieldNDX = nFieldNDX + 4;


                    // Get message length
                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    string nTotalMessageLength = TD_GetResponseValue(100, nMsg, nStartPos, 0);


                    /*/
                     * 52 / 82 - NASDAQ Chart
                     * 53 / 83 - NYSE Chart
                     * 55 / 85 - Indices Chart
                    /*/
                    nMsg = new byte[sizeof(short)];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    int nSID = BitConverter.ToInt16(nMsg, 0);
                    string cStreamingRequestChart = string.Empty;
                    switch (nSID)
                    {

                        case 82:
                            cStreamingRequestChart = " NASDAQ Chart";
                            break;
                        case 83:
                            cStreamingRequestChart = " NYSE Chart";
                            break;
                        case 85:
                            cStreamingRequestChart = " Indices Chart";
                            break;
                    }


                    // Get stock symbol length
                    nMsg = new byte[sizeof(short)];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    int nSymbolLength = BitConverter.ToInt16(nMsg, 0);


                    // Get stock symbol
                    nStartPos = nFieldNDX;
                    nMsg = new byte[nSymbolLength];
                    Array.Copy(gzBuffer, nStartPos, nMsg, 0, nSymbolLength);
                    string cSymbol = TD_GetResponseValue(0, nMsg, 0, nSymbolLength);

                    nFieldNDX = nFieldNDX + nSymbolLength;


                    // Get status
                    nMsg = new byte[sizeof(short)];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    int nStatus = BitConverter.ToInt16(nMsg, 0);


                    // Get length of compressed data
                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    string nTotalLenOfCompressedData = TD_GetResponseValue(100, nMsg, nStartPos, 0);

                    ms.Close();

                    byte[] CompressedData = new byte[Convert.ToInt32(nTotalLenOfCompressedData)];
                    Array.Copy(gzBuffer, nFieldNDX, CompressedData, 0, Convert.ToInt32(nTotalLenOfCompressedData));


                    StringBuilder cTempData = new StringBuilder();
                    int totalLength = 0;
                    byte[] writeData = new byte[65535 * 10];
                    Stream s2 = new InflaterInputStream(new MemoryStream(CompressedData));

                    try
                    {
                        while (true)
                        {
                            int size = s2.Read(writeData, 0, writeData.Length);
                            if (size > 0)
                            {
                                totalLength += size;
                                cTempData.Append(System.Text.Encoding.ASCII.GetString(writeData, 0, size));
                            }
                            else
                            {
                                break;
                            }
                        }
                        s2.Close();

                        string[] cLines = cTempData.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string cs in cLines)
                        {
                            cSortedLines.Add(cs);
                        }


                    }
                    catch (Exception e) { s2.Close(); return cSortedLines; }

                }


                GC.Collect();
            }
            catch (Exception exc) { }


            return cSortedLines;
        }




        #region Asynchronous TD Ameritrade methods

        private static void RespCallback(IAsyncResult ar)
        {
            // Get the RequestState object from the async result
            RequestState rs = (RequestState)ar.AsyncState;

            try
            {

                // Get the HttpWebRequest from RequestState
                HttpWebRequest req = rs.Request;

                // Calling EndGetResponse produces the HttpWebResponse object
                // which came from the request issued above
                HttpWebResponse resp = (HttpWebResponse)req.EndGetResponse(ar);


                // Now that we have the response, it is time to start reading
                // data from the response stream
                Stream ResponseStream = resp.GetResponseStream();


                // The read is also done using async so we'll want
                // to store the stream in RequestState
                rs.ResponseStream = ResponseStream;


                // Note that rs.BufferRead is passed in to BeginRead.  This is
                // where the data will be read into.            

                if (rs.Request.ServicePoint.CurrentConnections > 0 && rs.lNewStockSymbol == false)
                {
                    IAsyncResult iarRead = ResponseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);
                }
            }
            catch (Exception exc2)
            {

            }

        }


        private static void ReadCallBack(IAsyncResult asyncResult)
        {

            // Get the RequestState object from the asyncresult
            RequestState rs = (RequestState)asyncResult.AsyncState;

            try
            {

                if (rs.lNewStockSymbol == false)
                {

                    switch (rs.FunctionType)
                    {

                        case RequestState.AsyncType.ActivesStreaming:

                            break;

                        case RequestState.AsyncType.ChartSnapshot:

                            if (rs.lNewStockSymbol == false)
                            {
                                rs.process_AsyncChartSnapshot(asyncResult);
                            }
                            break;

                        case RequestState.AsyncType.ChartStreaming:

                            if (rs.lNewStockSymbol == false)
                            {
                                rs.process_AsyncChartStreaming(asyncResult);
                            }
                            break;

                        case RequestState.AsyncType.LevelOneSnapshot:

                            if (rs.lNewStockSymbol == false)
                            {
                                rs.process_AsyncLevelOneSnapshot(asyncResult);
                            }
                            break;

                        case RequestState.AsyncType.LevelOneStreaming:
                            if (rs.lNewStockSymbol == false)
                            {
                                rs.process_AsyncLevelOneStreaming(asyncResult);
                            }
                            break;

                        case RequestState.AsyncType.LevelTwoStreaming:

                            if (rs.lNewStockSymbol == false)
                            {
                                rs.process_AsyncLevelTwoStreaming(asyncResult);
                            }
                            break;
                    }
                }
            }
            catch (Exception exc)
            { }

            return;

        }



        public void TD_RequestAsyncChart_Streaming(string _streamStockSymbol, string chartSource, Form oForm)
        {

            string _streamerCommand = "S=" + chartSource + "&C=SUBS&P=" + _streamStockSymbol.ToUpper() + "&T=0+1+2+3+4+5+6+7";


            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;

                cpostdata.Append("!U=" + _accountid);
                cpostdata.Append("&W=" + _token);
                cpostdata.Append("&A=userid=" + _accountid);
                cpostdata.Append("&token=" + _token);
                cpostdata.Append("&company=" + _company);
                cpostdata.Append("&segment=" + _segment);
                cpostdata.Append("&cddomain=" + _cdDomain);
                cpostdata.Append("&usergroup=" + _usergroup);
                cpostdata.Append("&accesslevel=" + _accesslevel);
                cpostdata.Append("&authorized=" + _authorized);
                cpostdata.Append("&acl=" + _acl);
                cpostdata.Append("&timestamp=" + _timestamp);
                cpostdata.Append("&appid=" + _appid);
                cpostdata.Append("|" + _streamerCommand);

                string encodedString = Encode_URL(cpostdata.ToString());
                cpostdata = new StringBuilder();
                cpostdata.Append(encodedString);

                lcPostUrl = "http://" + this._streamerurl + "/" + cpostdata;


                /*/
                 *   Create the initial connection to the server.
                 * 
                /*/

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(lcPostUrl);

                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = lcPostUrl.Length;
                req.Accept = "Accept image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*";
                req.Method = "GET";

                req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                req.Timeout = 60000;
                req.ServicePoint.ConnectionLimit = 50;



                // Add the request into the state so it can be passed around

                this.rs_ChartStreaming = new RequestState();
                //this.rs_ChartStreaming.TickWithArgs += new AmeritradeBrokerAPI.EventHandlerWithArgs(((Form1)oForm).processEvent);
                this.rs_ChartStreaming.oParentForm = oForm;
                this.rs_ChartStreaming.lNewStockSymbol = false;

                this.rs_ChartStreaming.stockSymbol = _streamStockSymbol;
                this.rs_ChartStreaming.ServiceName = chartSource;
                this.rs_ChartStreaming.Request = req;
                this.rs_ChartStreaming.FunctionType = RequestState.AsyncType.ChartStreaming;

                // Issue the async request                
                IAsyncResult r = (IAsyncResult)req.BeginGetResponse(new AsyncCallback(RespCallback), this.rs_ChartStreaming);

            }
        }



        public void TD_RequestAsyncLevel1QuoteSnapshot(string symbol, Form oForm)
        {

            string _streamerCommand = "S=QUOTE" + "&C=GET&P=" + symbol + "&T=0+1+2+3+4+5+6+7+8+9+10+11+12+13+14+15+16+17+18+19+20+21+22+23+24+25+26+27+28+29+30+31+32+33+34+35+36+37+38+39+40";

            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;

                cpostdata.Append("!U=" + _accountid);
                cpostdata.Append("&W=" + _token);
                cpostdata.Append("&A=userid=" + _accountid);
                cpostdata.Append("&token=" + _token);
                cpostdata.Append("&company=" + _company);
                cpostdata.Append("&segment=" + _segment);
                cpostdata.Append("&cddomain=" + _cdDomain);
                cpostdata.Append("&usergroup=" + _usergroup);
                cpostdata.Append("&accesslevel=" + _accesslevel);
                cpostdata.Append("&authorized=" + _authorized);
                cpostdata.Append("&acl=" + _acl);
                cpostdata.Append("&timestamp=" + _timestamp);
                cpostdata.Append("&appid=" + _appid);
                cpostdata.Append("|" + _streamerCommand);

                string encodedString = Encode_URL(cpostdata.ToString());
                cpostdata = new StringBuilder();
                cpostdata.Append(encodedString);

                lcPostUrl = "http://" + this._streamerurl + "/" + cpostdata;

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(lcPostUrl);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Accept = "Accept image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*";
                req.Method = "GET";
                req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                req.Timeout = 60000;
                req.ServicePoint.ConnectionLimit = 50;



                //// Add the request into the state so it can be passed around

                this.rs_LevelOneSnapshot = new RequestState();
                //this.rs_LevelOneSnapshot.TickWithArgs += new AmeritradeBrokerAPI.EventHandlerWithArgs(((Form1)oForm).processEvent);
                this.rs_LevelOneSnapshot.oParentForm = oForm;
                this.rs_LevelOneSnapshot.lNewStockSymbol = false;


                this.rs_LevelOneSnapshot.stockSymbol = symbol;
                this.rs_LevelOneSnapshot.Request = req;
                this.rs_LevelOneSnapshot.FunctionType = RequestState.AsyncType.LevelOneSnapshot;

                // Issue the async request                
                IAsyncResult r = (IAsyncResult)req.BeginGetResponse(new AsyncCallback(RespCallback), this.rs_LevelOneSnapshot);
            }

        }



        public void TD_RequestAsyncLevel1QuoteStreaming(string symbol, string ServiceName, Form oForm)
        {

            string _streamerCommand = "S=QUOTE" + "&C=SUBS&P=" + symbol + "&T=0+1+2+3+4+5+6+7+8+9+10+11+12+13+14+15+16+17+18+19+20+21+22+23+24+25+26+27+28+29+30+31+32+33+34+35+36+37+38+39+40";

            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;

                cpostdata.Append("!U=" + _accountid);
                cpostdata.Append("&W=" + _token);
                cpostdata.Append("&A=userid=" + _accountid);
                cpostdata.Append("&token=" + _token);
                cpostdata.Append("&company=" + _company);
                cpostdata.Append("&segment=" + _segment);
                cpostdata.Append("&cddomain=" + _cdDomain);
                cpostdata.Append("&usergroup=" + _usergroup);
                cpostdata.Append("&accesslevel=" + _accesslevel);
                cpostdata.Append("&authorized=" + _authorized);
                cpostdata.Append("&acl=" + _acl);
                cpostdata.Append("&timestamp=" + _timestamp);
                cpostdata.Append("&appid=" + _appid);
                cpostdata.Append("|" + _streamerCommand);

                string encodedString = Encode_URL(cpostdata.ToString());
                cpostdata = new StringBuilder();
                cpostdata.Append(encodedString);

                lcPostUrl = "http://" + this._streamerurl + "/" + cpostdata;

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(lcPostUrl);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Accept = "Accept image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*";
                req.Method = "GET";
                req.MaximumResponseHeadersLength = 650000;
                req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                req.Timeout = 60000;
                req.ServicePoint.ConnectionLimit = 50;



                //// Add the request into the state so it can be passed around

                //this.rs_LevelOneStreaming = new RequestState();
                //this.rs_LevelOneStreaming.TickWithArgs += new AmeritradeBrokerAPI.EventHandlerWithArgs(((TDServerMain)oForm).processEvent);
                this.rs_LevelOneStreaming.oParentForm = oForm;
                this.rs_LevelOneStreaming.lNewStockSymbol = false;

                this.rs_LevelOneStreaming.oParent = this;
                this.rs_LevelOneStreaming.stockSymbol = symbol;
                this.rs_LevelOneStreaming.ServiceName = ServiceName;
                this.rs_LevelOneStreaming.Request = req;
                this.rs_LevelOneStreaming.FunctionType = RequestState.AsyncType.LevelOneStreaming;

                // Issue the async request                
                IAsyncResult r = (IAsyncResult)req.BeginGetResponse(new AsyncCallback(RespCallback), this.rs_LevelOneStreaming);
            }

        }




        public void TD_RequestAsyncLevel2Streaming(string symbol, Level2DataSource Level2DataSourceType, Form oForm)
        {
            /*/
             * Old code used for requesting NON-TotalView based on compressed data.
            /*/
            //string _streamerCommand = "S=" + this.getLevel2SourceName(Level2DataSourceType) + "&C=SUBS&P=" + symbol + "&T=0+1";

            /*/
             * New code used for requesting TotalView based on uncompressed data.
            /*/


            string _streamerCommand = "S=TOTAL_VIEW&C=SUBS&P=" + symbol + ">L2&T=0+1+2+3";



            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;

                cpostdata.Append("!U=" + _accountid);
                cpostdata.Append("&W=" + _token);
                cpostdata.Append("&A=userid=" + _accountid);
                cpostdata.Append("&token=" + _token);
                cpostdata.Append("&company=" + _company);
                cpostdata.Append("&segment=" + _segment);
                cpostdata.Append("&cddomain=" + _cdDomain);
                cpostdata.Append("&usergroup=" + _usergroup);
                cpostdata.Append("&accesslevel=" + _accesslevel);
                cpostdata.Append("&authorized=" + _authorized);
                cpostdata.Append("&acl=" + _acl);
                cpostdata.Append("&timestamp=" + _timestamp);
                cpostdata.Append("&appid=" + _appid);
                cpostdata.Append("|" + _streamerCommand);

                string encodedString = Encode_URL(cpostdata.ToString());
                cpostdata = new StringBuilder();
                cpostdata.Append(encodedString);

                lcPostUrl = "http://" + this._streamerurl + "/" + cpostdata;

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(lcPostUrl);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Accept = "Accept image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*";
                req.Method = "GET";
                req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                req.Timeout = 60000;
                req.ServicePoint.ConnectionLimit = 50;



                //// Add the request into the state so it can be passed around

                this.rs_LevelTwoStreaming = new RequestState();
                //this.rs_LevelTwoStreaming.TickWithArgs += new AmeritradeBrokerAPI.EventHandlerWithArgs(((Form1)oForm).processEvent);
                this.rs_LevelTwoStreaming.oParentForm = oForm;
                this.rs_LevelTwoStreaming.lNewStockSymbol = false;


                this.rs_LevelTwoStreaming.oParent = this;
                this.rs_LevelTwoStreaming.Level2DataSourceType = Level2DataSourceType;
                this.rs_LevelTwoStreaming.stockSymbol = symbol;
                this.rs_LevelTwoStreaming.ServiceName = Level2DataSourceType.ToString();
                this.rs_LevelTwoStreaming.Request = req;
                this.rs_LevelTwoStreaming.FunctionType = RequestState.AsyncType.LevelTwoStreaming;

                // Issue the async request                
                IAsyncResult r = (IAsyncResult)req.BeginGetResponse(new AsyncCallback(RespCallback), this.rs_LevelTwoStreaming);
            }

        }


        #endregion



        public bool TD_IsStockSymbolValid(string symbol)
        {
            bool lStockSymbolFound = true;
            AmeritradeBrokerAPI.L1quotes oL1Quote = new L1quotes();

            /*/ Make sure that the stock symbolis valid. /*/

            try
            {
                oL1Quote = this.TD_GetLevel1Quote(symbol, 1000);
            }
            catch (Exception Exc)
            {
                lStockSymbolFound = false;
            }

            lStockSymbolFound = (oL1Quote.stock == null || lStockSymbolFound == false ? false : true);

            return lStockSymbolFound;

        }



        //public bool TD_IsStockSymbolValid(string symbol)
        //{
        //    bool lStockSymbolFound = true;

        //    /*/ Make sure that the stock symbolis valid. /*/

        //    try
        //    {
        //         AmeritradeBrokerAPI.L1quotes oL1Quote = this.TD_GetLevel1Quote(symbol, 1000);
        //    }
        //    catch (Exception Exc)
        //    {
        //        lStockSymbolFound = false;
        //    }


        //    return lStockSymbolFound;
        //}


        public L1quotes TD_GetLevel1Quote(string symbol, int nTimeOut)
        {
            byte[] bReplyValue = null;
            L1quotes oL1Quotes = new L1quotes();
            Object oLockTD_GetLevel1Quote = new object();

            lock (oLockTD_GetLevel1Quote)
            {
                Monitor.Enter(oLockTD_GetLevel1Quote);

                string _streamerCommand = "S=QUOTE" + "&C=GET&P=" + symbol + "&T=0+1+2+3+4+5+6+7+8+9+10+11+12+13+14+15+16+17+18+19+20+21+22+23+24+25+26+27+28+29+30+31+32+33+34+35+36+37+38+39+40";

                try
                {

                    if (this.TD_loginStatus == true)
                    {

                        bReplyValue = new byte[65535];
                        XMLHTTP xmlHttp_ = new XMLHTTP();
                        StringBuilder cpostdata = new StringBuilder();
                        string lcPostUrl = string.Empty;

                        cpostdata.Append("!U=" + _accountid);
                        cpostdata.Append("&W=" + _token);
                        cpostdata.Append("&A=userid=" + _accountid);
                        cpostdata.Append("&token=" + _token);
                        cpostdata.Append("&company=" + _company);
                        cpostdata.Append("&segment=" + _segment);
                        cpostdata.Append("&cddomain=" + _cdDomain);
                        cpostdata.Append("&usergroup=" + _usergroup);
                        cpostdata.Append("&accesslevel=" + _accesslevel);
                        cpostdata.Append("&authorized=" + _authorized);
                        cpostdata.Append("&acl=" + _acl);
                        cpostdata.Append("&timestamp=" + _timestamp);
                        cpostdata.Append("&appid=" + _appid);
                        cpostdata.Append("|" + _streamerCommand);

                        string encodedString = Encode_URL(cpostdata.ToString());
                        cpostdata = new StringBuilder();
                        cpostdata.Append(encodedString);

                        lcPostUrl = "http://" + this._streamerurl + "/" + cpostdata;

                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(lcPostUrl);
                        req.ContentType = "application/x-www-form-urlencoded";
                        req.Accept = "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*";
                        req.Headers.Add("Accept-Encoding: deflate, gzip");
                        req.Method = "GET";
                        req.Timeout = (nTimeOut == 0 ? 60000 : nTimeOut);
                        req.ReadWriteTimeout = (nTimeOut == 0 ? 8000 : nTimeOut);
                        req.ServicePoint.ConnectionLimit = 50;

                        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();


                        // Retrieve response stream
                        Stream respStream = resp.GetResponseStream();

                        byte[] chunk = new byte[65535];
                        byte[] ChartByteArray = new byte[65535];

                        int bytesRead = respStream.Read(ChartByteArray, 0, ChartByteArray.Length);
                        string compressedText = Convert.ToBase64String(ChartByteArray);
                        byte[] gzBuffer = Convert.FromBase64String(compressedText);


                        respStream.Flush();
                        resp.Close();
                        respStream.Close();



                        MemoryStream ms = new MemoryStream();

                        int nFieldNDX = 0;
                        int nStartPos = 13;
                        int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                        ms.Write(gzBuffer, 64, gzBuffer.Length - 64);


                        byte[] nMsg = new byte[sizeof(Int32)];

                        ms.Close();

                        /*/
                         * S = Streaming
                         * N = Snapshot
                        /*/

                        nMsg[0] = gzBuffer[0];
                        string cRequestType = System.Text.Encoding.ASCII.GetString(nMsg, 0, 1);



                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[2];
                        nMsg[1] = gzBuffer[1];
                        int nNextFieldLength = BitConverter.ToInt32(nMsg, 0);



                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[5];
                        nMsg[1] = gzBuffer[4];
                        nMsg[2] = gzBuffer[3];

                        long nNextField = BitConverter.ToInt32(nMsg, 0);



                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[7];
                        nMsg[1] = gzBuffer[6];
                        int nMessageLength = BitConverter.ToInt32(nMsg, 0);



                        /*/
                         * 1 = L1 Quote
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[9];
                        nMsg[1] = gzBuffer[8];
                        int nQuoteType = BitConverter.ToInt32(nMsg, 0);


                        /*/
                         * Stock Symbol : Field 0
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[10];
                        int nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[12];
                        nMsg[1] = gzBuffer[11];
                        int nSymbolLength = BitConverter.ToInt32(nMsg, 0);
                        nMsg = new byte[nSymbolLength];
                        Array.Copy(gzBuffer, nStartPos, nMsg, 0, nSymbolLength);
                        oL1Quotes.stock = TD_GetResponseValue(0, nMsg, 0, nSymbolLength);



                        /*/
                         * Bid Price : Field 1
                        /*/

                        nFieldNDX = nStartPos + nSymbolLength;


                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.bid = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                        /*/
                         * Ask Price : Field 2
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.ask = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                        /*/
                         * Last Price : Field 3
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.last = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                        /*/
                         * Bid Size : Field 4
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.bid_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);


                        /*/
                         * Ask Size : Field 5
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.ask_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                        /*/
                         * Bid ID : Field 6
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(short)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.bid_id = TD_GetResponseValue(sizeof(short), nMsg, nStartPos, 0);


                        /*/
                         * Ask ID : Field 7
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(short)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.ask_id = TD_GetResponseValue(sizeof(short), nMsg, nStartPos, 0);


                        /*/
                         * Volume : Field 8
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(long)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        nMsg[4] = gzBuffer[nFieldNDX++];
                        nMsg[5] = gzBuffer[nFieldNDX++];
                        nMsg[6] = gzBuffer[nFieldNDX++];
                        nMsg[7] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.volume = TD_GetResponseValue(sizeof(long), nMsg, nStartPos, 0);




                        /*/
                         * Last Size : Field 9
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.lastsize = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                        /*/
                         * Trade Time : Field 10
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.tradetime = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                        /*/
                         * Quote Time : Field 11
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.quotetime = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                        /*/
                         * High : Field 12
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.high = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                        /*/
                         * Low : Field 13
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.low = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                        /*/
                         * Tick : Field 14
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(char)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);

                        nMsg = new byte[sizeof(char)];
                        Array.Copy(gzBuffer, nStartPos, nMsg, 0, sizeof(char));
                        oL1Quotes.tick = TD_GetResponseValue(0, nMsg, 0, sizeof(char));



                        /*/
                         * Close : Field 15
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.close = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);




                        /*/
                          * Exchange : Field 16
                         /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(char)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);

                        nMsg = new byte[sizeof(char)];
                        Array.Copy(gzBuffer, nStartPos, nMsg, 0, sizeof(char));
                        oL1Quotes.exchange = TD_GetResponseValue(0, nMsg, 0, sizeof(char));



                        /*/
                         * Marginable : Field 17
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(bool)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.marginable = TD_GetResponseValue(sizeof(bool), nMsg, 0, sizeof(char));


                        /*/
                         * Shortable : Field 18
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(bool)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.shortable = TD_GetResponseValue(sizeof(bool), nMsg, 0, sizeof(char));


                        /*/
                         * ISLAND BID : Field 19
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.islandbid = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                        /*/
                         * ISLAND ASK : Field 20
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.islandask = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                        /*/
                         * ISLAND ASK : Field 21
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(long)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        nMsg[4] = gzBuffer[nFieldNDX++];
                        nMsg[5] = gzBuffer[nFieldNDX++];
                        nMsg[6] = gzBuffer[nFieldNDX++];
                        nMsg[7] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.islandvolume = TD_GetResponseValue(sizeof(long), nMsg, nStartPos, 0);



                        /*/
                         * QUOTE DATE : Field 22
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.quotedate = TD_GetResponseValue(100, nMsg, nStartPos, 0);




                        /*/
                         * QUOTE DATE : Field 23
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.tradedate = TD_GetResponseValue(100, nMsg, nStartPos, 0);


                        /*/
                         * Volatility : Field 24
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.volatility = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                        /*/
                         * Description : Field 25
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(short)];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nStartPos = nFieldNDX;

                        int nStringlLength = BitConverter.ToInt16(nMsg, 0);
                        nFieldNDX = nFieldNDX + nStringlLength;

                        nMsg = new byte[nStringlLength];
                        Array.Copy(gzBuffer, nStartPos, nMsg, 0, nStringlLength);
                        oL1Quotes.description = TD_GetResponseValue(0, nMsg, 0, nStringlLength);




                        /*/
                         * Trade ID : Field 26
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(char)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);

                        nMsg = new byte[sizeof(char)];
                        Array.Copy(gzBuffer, nStartPos, nMsg, 0, sizeof(char));
                        oL1Quotes.trade_id = TD_GetResponseValue(0, nMsg, 0, sizeof(char));



                        /*/
                         * Digits : Field 27
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.digits = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                        /*/
                         * Open : Field 28
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.open = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                        /*/
                         * Change : Field 29
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.change = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                        /*/
                         * 52-Week High : Field 30
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.week_high_52 = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                        /*/
                         * 52-Week Low : Field 31
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.week_low_52 = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                        /*/
                         * PE-Ratio : Field 32
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.pe_ratio = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                        /*/
                         * Dividend Amount : Field 33
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.dividend_amt = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);




                        /*/
                         * Dividend Yield : Field 34
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.dividend_yield = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                        /*/
                         * ISLAND BID SIZE : Field 35
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.island_bid_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);




                        /*/
                         * ISLAND ASK SIZE : Field 36
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.island_ask_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                        /*/
                         * NAV : Field 37
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.nav = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);




                        /*/
                         * FUND : Field 38
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                        nStartPos = nFieldNDX;


                        nMsg = new byte[sizeof(float)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[2] = gzBuffer[nFieldNDX++];
                        nMsg[3] = gzBuffer[nFieldNDX++];
                        Array.Reverse(nMsg);
                        oL1Quotes.fund = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);




                        /*/
                         * EXCHANGE NAME : Field 39
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(short)];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nStartPos = nFieldNDX;

                        nStringlLength = BitConverter.ToInt16(nMsg, 0);
                        nFieldNDX = nFieldNDX + nStringlLength;

                        nMsg = new byte[nStringlLength];
                        Array.Copy(gzBuffer, nStartPos, nMsg, 0, nStringlLength);
                        oL1Quotes.exchange_name = TD_GetResponseValue(0, nMsg, 0, nStringlLength);



                        /*/
                         * DIVIDEND DATE : Field 40
                        /*/

                        nMsg = new byte[sizeof(Int32)];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                        nMsg = new byte[sizeof(short)];
                        nMsg[1] = gzBuffer[nFieldNDX++];
                        nMsg[0] = gzBuffer[nFieldNDX++];
                        nStartPos = nFieldNDX;

                        nStringlLength = BitConverter.ToInt16(nMsg, 0);
                        nFieldNDX = nFieldNDX + nStringlLength;

                        nMsg = new byte[nStringlLength];
                        Array.Copy(gzBuffer, nStartPos, nMsg, 0, nStringlLength);
                        oL1Quotes.dividend_date = TD_GetResponseValue(0, nMsg, 0, nStringlLength);


                        GC.Collect();


                    }

                    Monitor.Exit(oLockTD_GetLevel1Quote);
                    Monitor.Pulse(oLockTD_GetLevel1Quote);

                }
                catch (Exception exc) { }

            }

            return oL1Quotes;

        }





        public L1quotes TD_GetLevel1Quote2(string symbol, int nTimeOut)
        {
            byte[] bReplyValue = null;
            L1quotes oL1Quotes = new L1quotes();
            Object oLockTD_GetLevel1Quote2 = new object();


            string _streamerCommand = "S=QUOTE" + "&C=GET&P=" + symbol + "&T=0+1+2+3+4+5+6+7+8+9+10+11+12+13+14+15+16+17+18+19+20+21+22+23+24+25+26+27+28+29+30+31+32+33+34+35+36+37+38+39+40";

            try
            {

                if (this.TD_loginStatus == true)
                {

                    bReplyValue = new byte[65535];
                    XMLHTTP xmlHttp_ = new XMLHTTP();
                    StringBuilder cpostdata = new StringBuilder();
                    string lcPostUrl = string.Empty;

                    cpostdata.Append("!U=" + _accountid);
                    cpostdata.Append("&W=" + _token);
                    cpostdata.Append("&A=userid=" + _accountid);
                    cpostdata.Append("&token=" + _token);
                    cpostdata.Append("&company=" + _company);
                    cpostdata.Append("&segment=" + _segment);
                    cpostdata.Append("&cddomain=" + _cdDomain);
                    cpostdata.Append("&usergroup=" + _usergroup);
                    cpostdata.Append("&accesslevel=" + _accesslevel);
                    cpostdata.Append("&authorized=" + _authorized);
                    cpostdata.Append("&acl=" + _acl);
                    cpostdata.Append("&timestamp=" + _timestamp);
                    cpostdata.Append("&appid=" + _appid);
                    cpostdata.Append("|" + _streamerCommand);

                    string encodedString = Encode_URL(cpostdata.ToString());
                    cpostdata = new StringBuilder();
                    cpostdata.Append(encodedString);

                    lcPostUrl = "http://" + this._streamerurl + "/" + cpostdata;

                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(lcPostUrl);
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.Accept = "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*";
                    req.Headers.Add("Accept-Encoding: deflate, gzip");
                    req.Method = "GET";
                    req.Timeout = (nTimeOut == 0 ? 5000 : nTimeOut);
                    req.ReadWriteTimeout = (nTimeOut == 0 ? 5000 : nTimeOut);
                    req.ServicePoint.ConnectionLimit = 50;

                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();


                    // Retrieve response stream
                    Stream respStream = resp.GetResponseStream();

                    byte[] chunk = new byte[65535];
                    byte[] ChartByteArray = new byte[65535];

                    int bytesRead = respStream.Read(ChartByteArray, 0, ChartByteArray.Length);
                    string compressedText = Convert.ToBase64String(ChartByteArray);
                    byte[] gzBuffer = Convert.FromBase64String(compressedText);


                    respStream.Flush();
                    resp.Close();
                    respStream.Close();



                    MemoryStream ms = new MemoryStream();

                    int nFieldNDX = 0;
                    int nStartPos = 13;
                    int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                    ms.Write(gzBuffer, 64, gzBuffer.Length - 64);


                    byte[] nMsg = new byte[sizeof(Int32)];

                    ms.Close();

                    /*/
                     * S = Streaming
                     * N = Snapshot
                    /*/

                    nMsg[0] = gzBuffer[0];
                    string cRequestType = System.Text.Encoding.ASCII.GetString(nMsg, 0, 1);



                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[2];
                    nMsg[1] = gzBuffer[1];
                    int nNextFieldLength = BitConverter.ToInt32(nMsg, 0);



                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[5];
                    nMsg[1] = gzBuffer[4];
                    nMsg[2] = gzBuffer[3];

                    long nNextField = BitConverter.ToInt32(nMsg, 0);



                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[7];
                    nMsg[1] = gzBuffer[6];
                    int nMessageLength = BitConverter.ToInt32(nMsg, 0);



                    /*/
                     * 1 = L1 Quote
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[9];
                    nMsg[1] = gzBuffer[8];
                    int nQuoteType = BitConverter.ToInt32(nMsg, 0);


                    /*/
                     * Stock Symbol : Field 0
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[10];
                    int nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[12];
                    nMsg[1] = gzBuffer[11];
                    int nSymbolLength = BitConverter.ToInt32(nMsg, 0);
                    nMsg = new byte[nSymbolLength];
                    Array.Copy(gzBuffer, nStartPos, nMsg, 0, nSymbolLength);
                    oL1Quotes.stock = TD_GetResponseValue(0, nMsg, 0, nSymbolLength);



                    /*/
                     * Bid Price : Field 1
                    /*/

                    nFieldNDX = nStartPos + nSymbolLength;


                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.bid = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                    /*/
                     * Ask Price : Field 2
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.ask = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                    /*/
                     * Last Price : Field 3
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.last = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                    /*/
                     * Bid Size : Field 4
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.bid_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);


                    /*/
                     * Ask Size : Field 5
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.ask_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                    /*/
                     * Bid ID : Field 6
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(short)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.bid_id = TD_GetResponseValue(sizeof(short), nMsg, nStartPos, 0);


                    /*/
                     * Ask ID : Field 7
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(short)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.ask_id = TD_GetResponseValue(sizeof(short), nMsg, nStartPos, 0);


                    /*/
                     * Volume : Field 8
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(long)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    nMsg[4] = gzBuffer[nFieldNDX++];
                    nMsg[5] = gzBuffer[nFieldNDX++];
                    nMsg[6] = gzBuffer[nFieldNDX++];
                    nMsg[7] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.volume = TD_GetResponseValue(sizeof(long), nMsg, nStartPos, 0);




                    /*/
                     * Last Size : Field 9
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.lastsize = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                    /*/
                     * Trade Time : Field 10
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.tradetime = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                    /*/
                     * Quote Time : Field 11
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.quotetime = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                    /*/
                     * High : Field 12
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.high = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                    /*/
                     * Low : Field 13
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.low = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                    /*/
                     * Tick : Field 14
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(char)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);

                    nMsg = new byte[sizeof(char)];
                    Array.Copy(gzBuffer, nStartPos, nMsg, 0, sizeof(char));
                    oL1Quotes.tick = TD_GetResponseValue(0, nMsg, 0, sizeof(char));



                    /*/
                     * Close : Field 15
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.close = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);




                    /*/
                      * Exchange : Field 16
                     /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(char)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);

                    nMsg = new byte[sizeof(char)];
                    Array.Copy(gzBuffer, nStartPos, nMsg, 0, sizeof(char));
                    oL1Quotes.exchange = TD_GetResponseValue(0, nMsg, 0, sizeof(char));



                    /*/
                     * Marginable : Field 17
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(bool)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.marginable = TD_GetResponseValue(sizeof(bool), nMsg, 0, sizeof(char));


                    /*/
                     * Shortable : Field 18
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(bool)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.shortable = TD_GetResponseValue(sizeof(bool), nMsg, 0, sizeof(char));


                    /*/
                     * ISLAND BID : Field 19
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.islandbid = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                    /*/
                     * ISLAND ASK : Field 20
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.islandask = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                    /*/
                     * ISLAND ASK : Field 21
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(long)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    nMsg[4] = gzBuffer[nFieldNDX++];
                    nMsg[5] = gzBuffer[nFieldNDX++];
                    nMsg[6] = gzBuffer[nFieldNDX++];
                    nMsg[7] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.islandvolume = TD_GetResponseValue(sizeof(long), nMsg, nStartPos, 0);



                    /*/
                     * QUOTE DATE : Field 22
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.quotedate = TD_GetResponseValue(100, nMsg, nStartPos, 0);




                    /*/
                     * QUOTE DATE : Field 23
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.tradedate = TD_GetResponseValue(100, nMsg, nStartPos, 0);


                    /*/
                     * Volatility : Field 24
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.volatility = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                    /*/
                     * Description : Field 25
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(short)];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nStartPos = nFieldNDX;

                    int nStringlLength = BitConverter.ToInt16(nMsg, 0);
                    nFieldNDX = nFieldNDX + nStringlLength;

                    nMsg = new byte[nStringlLength];
                    Array.Copy(gzBuffer, nStartPos, nMsg, 0, nStringlLength);
                    oL1Quotes.description = TD_GetResponseValue(0, nMsg, 0, nStringlLength);




                    /*/
                     * Trade ID : Field 26
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(char)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);

                    nMsg = new byte[sizeof(char)];
                    Array.Copy(gzBuffer, nStartPos, nMsg, 0, sizeof(char));
                    oL1Quotes.trade_id = TD_GetResponseValue(0, nMsg, 0, sizeof(char));



                    /*/
                     * Digits : Field 27
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.digits = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                    /*/
                     * Open : Field 28
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.open = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                    /*/
                     * Change : Field 29
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.change = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                    /*/
                     * 52-Week High : Field 30
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.week_high_52 = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);


                    /*/
                     * 52-Week Low : Field 31
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.week_low_52 = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                    /*/
                     * PE-Ratio : Field 32
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.pe_ratio = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                    /*/
                     * Dividend Amount : Field 33
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.dividend_amt = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);




                    /*/
                     * Dividend Yield : Field 34
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.dividend_yield = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);



                    /*/
                     * ISLAND BID SIZE : Field 35
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.island_bid_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);




                    /*/
                     * ISLAND ASK SIZE : Field 36
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.island_ask_size = TD_GetResponseValue(100, nMsg, nStartPos, 0);



                    /*/
                     * NAV : Field 37
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.nav = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);




                    /*/
                     * FUND : Field 38
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);
                    nStartPos = nFieldNDX;


                    nMsg = new byte[sizeof(float)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[2] = gzBuffer[nFieldNDX++];
                    nMsg[3] = gzBuffer[nFieldNDX++];
                    Array.Reverse(nMsg);
                    oL1Quotes.fund = TD_GetResponseValue(sizeof(float), nMsg, nStartPos, 0);




                    /*/
                     * EXCHANGE NAME : Field 39
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(short)];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nStartPos = nFieldNDX;

                    nStringlLength = BitConverter.ToInt16(nMsg, 0);
                    nFieldNDX = nFieldNDX + nStringlLength;

                    nMsg = new byte[nStringlLength];
                    Array.Copy(gzBuffer, nStartPos, nMsg, 0, nStringlLength);
                    oL1Quotes.exchange_name = TD_GetResponseValue(0, nMsg, 0, nStringlLength);



                    /*/
                     * DIVIDEND DATE : Field 40
                    /*/

                    nMsg = new byte[sizeof(Int32)];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nFieldNumber = BitConverter.ToInt32(nMsg, 0);

                    nMsg = new byte[sizeof(short)];
                    nMsg[1] = gzBuffer[nFieldNDX++];
                    nMsg[0] = gzBuffer[nFieldNDX++];
                    nStartPos = nFieldNDX;

                    nStringlLength = BitConverter.ToInt16(nMsg, 0);
                    nFieldNDX = nFieldNDX + nStringlLength;

                    nMsg = new byte[nStringlLength];
                    Array.Copy(gzBuffer, nStartPos, nMsg, 0, nStringlLength);
                    oL1Quotes.dividend_date = TD_GetResponseValue(0, nMsg, 0, nStringlLength);


                    GC.Collect();


                }


            }
            catch (Exception exc)
            { }


            return oL1Quotes;

        }



        public string RemoveHTML(string in_HTML)
        {
            return Regex.Replace(in_HTML, "<(.|\n)*?>", "");
        }



        public void TD_geOrderStatusAndHistory(string _userid, string _password, string _source, string _version, string orderList, orderHistoryType OrderyHistoryType, ref List<ATradeArgument.tradeReplyDetails> oReplyDetails)
        {


            if (this.TD_loginStatus == true)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;

                cpostdata.Append("source=" + Encode_URL(_source));


                switch (OrderyHistoryType)
                {
                    case orderHistoryType.ALL_ORDERS:

                        cpostdata.Append("&type=" + Encode_URL("all"));
                        cpostdata.Append("&days=" + Encode_URL("0"));

                        lcPostUrl = "https://apis.tdameritrade.com/apps/100/OrderStatus?" + cpostdata.ToString();

                        break;


                    case orderHistoryType.OPEN_ORDERS:

                        cpostdata.Append("orderid=" + Encode_URL(orderList));
                        cpostdata.Append("type=" + Encode_URL("open"));
                        cpostdata.Append("days=" + Encode_URL("0"));

                        lcPostUrl = "https://apis.tdameritrade.com/apps/100/OrderStatus?source?" + cpostdata.ToString();

                        break;


                    case orderHistoryType.PENDING_ORDERS:

                        cpostdata.Append("orderid=" + Encode_URL(orderList));
                        cpostdata.Append("type=" + Encode_URL("pending"));
                        cpostdata.Append("days=" + Encode_URL("0"));

                        lcPostUrl = "https://apis.tdameritrade.com/apps/100/OrderStatus?source?" + cpostdata.ToString();

                        break;


                    case orderHistoryType.FILLED:

                        cpostdata.Append("orderid=" + Encode_URL(orderList));
                        cpostdata.Append("type=" + Encode_URL("filled"));
                        cpostdata.Append("days=" + Encode_URL("0"));

                        lcPostUrl = "https://apis.tdameritrade.com/apps/100/OrderStatus?source?" + cpostdata.ToString();

                        break;


                    case orderHistoryType.CANCELLED_ORDERS:

                        cpostdata.Append("orderid=" + Encode_URL(orderList));
                        cpostdata.Append("type=" + Encode_URL("canceled"));
                        cpostdata.Append("days=" + Encode_URL("0"));

                        lcPostUrl = "https://apis.tdameritrade.com/apps/100/OrderStatus?source?" + cpostdata.ToString();

                        break;


                }

                xmlHttp_.open("POST", lcPostUrl, false, _userid, _password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                string xmlData = xmlHttp_.responseText.ToString();
                string cResponseHeaders = xmlHttp_.getAllResponseHeaders();


                /*/
                 * Test Code : Use the following line to test the parsing of balances and positions
                 * 
                /*/

                StringReader reader = new StringReader(xmlData);
                XmlTextReader xml = new XmlTextReader(reader);


                // Check for errors.
                if (null == xmlData || "" == xmlData)
                {
                    // Throw an exception.
                    throw new Exception("Failed to connect, check username and password?");
                }


                string[,] oFillDetails = new string[1, 2];
                List<string[,]> oFillDetailsList = new List<string[,]>();
                ATradeArgument.tradeReplyDetails oTradeHistoryDetails = new ATradeArgument.tradeReplyDetails();
                string name = string.Empty;


                while (xml.Read())
                {
                    // Got an element?
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        // Get this node.
                        name = xml.Name;

                        // Initialize the start of the collection 
                        if (name.ToLower().ToString().CompareTo("orderstatus") == 0)
                        {
                            oTradeHistoryDetails = new ATradeArgument.tradeReplyDetails();
                            oFillDetailsList = new List<string[,]>();

                        }


                        // Get order-number
                        if (name.ToLower().ToString().CompareTo("order-number") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderNumber = xml.Value;
                        }


                        // Get cancelable
                        if (name.ToLower().ToString().CompareTo("cancelable") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.lIsCancelable = Convert.ToBoolean(xml.Value);
                        }

                        // Get editable
                        if (name.ToLower().ToString().CompareTo("editable") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.lIsEditable = Convert.ToBoolean(xml.Value);
                        }

                        // Get enhanced-order
                        if (name.ToLower().ToString().CompareTo("enhanced-order") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.lIsEnhancedOrder = Convert.ToBoolean(xml.Value);
                        }

                        // Get EnhancedOrderType
                        if (name.ToLower().ToString().CompareTo("enhanced-type") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.EnhancedOrderType = xml.Value;
                        }

                        // Get display-status
                        if (name.ToLower().ToString().CompareTo("display-status") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.DisplayStatus = xml.Value;
                        }

                        // Get order-routing-status
                        if (name.ToLower().ToString().CompareTo("order-routing-status") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderRoutingStatus = xml.Value;
                        }

                        // Get order-received-date-time
                        if (name.ToLower().ToString().CompareTo("order-received-date-time") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderReceivedTime = xml.Value;
                        }

                        // Get reported-time
                        if (name.ToLower().ToString().CompareTo("reported-time") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderReportedTime = xml.Value;
                        }


                        // Get remaining-quantity
                        if (name.ToLower().ToString().CompareTo("remaining-quantity") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderSharesRemianing = xml.Value;
                        }

                        // Get symbol
                        if (name.ToLower().ToString().CompareTo("symbol") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.cStockSymbol = xml.Value;
                        }

                        // Get asset-type
                        if (name.ToLower().ToString().CompareTo("asset-type") == 0)
                        {
                            xml.Read();
                            if (xml.Value.ToUpper().CompareTo("E") == 0)
                                oTradeHistoryDetails.AssetType = "Equity";

                            if (xml.Value.ToUpper().CompareTo("F") == 0)
                                oTradeHistoryDetails.AssetType = "Mutual Fund";

                            if (xml.Value.ToUpper().CompareTo("O") == 0)
                                oTradeHistoryDetails.AssetType = "Option";

                            if (xml.Value.ToUpper().CompareTo("B") == 0)
                                oTradeHistoryDetails.AssetType = "Bond";


                        }

                        // Get quantity
                        if (name.ToLower().ToString().CompareTo("quantity") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderShares = xml.Value;
                        }

                        // Get order-id
                        if (name.ToLower().ToString().CompareTo("order-id") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderID = xml.Value;
                        }

                        // Get action
                        if (name.ToLower().ToString().CompareTo("action") == 0)
                        {
                            xml.Read();
                            if (xml.Value.ToUpper().CompareTo("B") == 0)
                                oTradeHistoryDetails.Action = "Buy Long";

                            if (xml.Value.ToUpper().CompareTo("S") == 0)
                                oTradeHistoryDetails.Action = "Sell";

                            if (xml.Value.ToUpper().CompareTo("SS") == 0)
                                oTradeHistoryDetails.Action = "Sell Short";

                            if (xml.Value.ToUpper().CompareTo("BC") == 0)
                                oTradeHistoryDetails.Action = "Buy to Cover";


                        }

                        // Get trade-type
                        if (name.ToLower().ToString().CompareTo("trade-type") == 0)
                        {
                            xml.Read();
                            if (xml.Value.CompareTo("1") == 0)
                                oTradeHistoryDetails.TradeType = "Normal Market";

                            if (xml.Value.CompareTo("2") == 0)
                                oTradeHistoryDetails.TradeType = "External Hour Market";

                            if (xml.Value.CompareTo("4") == 0)
                                oTradeHistoryDetails.TradeType = "German Market";

                            if (xml.Value.CompareTo("8") == 0)
                                oTradeHistoryDetails.TradeType = "AM Session";

                            if (xml.Value.CompareTo("16") == 0)
                                oTradeHistoryDetails.TradeType = "Seamless Session";

                        }

                        // Get order-type
                        if (name.ToLower().ToString().CompareTo("order-type") == 0)
                        {
                            xml.Read();
                            if (xml.Value.ToUpper().CompareTo("M") == 0)
                                oTradeHistoryDetails.OrderType = "Market";

                            if (xml.Value.ToUpper().CompareTo("L") == 0)
                                oTradeHistoryDetails.OrderType = "Limit";

                            if (xml.Value.ToUpper().CompareTo("S") == 0)
                                oTradeHistoryDetails.OrderType = "Stop";

                            if (xml.Value.ToUpper().CompareTo("X") == 0)
                                oTradeHistoryDetails.OrderType = "Stop Limit";

                            if (xml.Value.ToUpper().CompareTo("D") == 0)
                                oTradeHistoryDetails.OrderType = "Debit";

                            if (xml.Value.ToUpper().CompareTo("C") == 0)
                                oTradeHistoryDetails.OrderType = "Credit";

                            if (xml.Value.ToUpper().CompareTo("T") == 0)
                                oTradeHistoryDetails.OrderType = "Trailing Stop";

                            if (xml.Value.ToUpper().CompareTo("EX") == 0)
                                oTradeHistoryDetails.OrderType = "Exercise Option";


                        }


                        // Get Fill(s) - Quantity
                        if (name.ToLower().ToString().CompareTo("fill-quantity") == 0)
                        {
                            xml.Read();
                            oFillDetails = new string[1, 2];
                            oFillDetails[0, 0] = xml.Value;

                        }


                        // Get Fill(s) - Price
                        if (name.ToLower().ToString().CompareTo("fill-price") == 0)
                        {
                            xml.Read();
                            oFillDetails[0, 1] = xml.Value;
                            oFillDetailsList.Add(oFillDetails);

                        }



                        // Get limit-price
                        if (name.ToLower().ToString().CompareTo("limit-price") == 0)
                        {
                            xml.Read();
                            oTradeHistoryDetails.OrderPrice = xml.Value;
                        }

                    }
                    else
                    {
                        if (xml.NodeType == XmlNodeType.EndElement)
                        {

                            if (xml.Name.ToString().ToLower().CompareTo("orderstatus") == 0)
                            {
                                Decimal nTotalDollarsExecuted = 0.00M;
                                Int32 nTotalSharesExecuted = 0;

                                if (oFillDetailsList.Count > 0)
                                {
                                    foreach (string[,] oDetail in oFillDetailsList)
                                    {
                                        nTotalSharesExecuted = nTotalSharesExecuted + Convert.ToInt32(Decimal.Round(Convert.ToDecimal(oDetail[0, 0]), 0));
                                        nTotalDollarsExecuted = nTotalDollarsExecuted + Convert.ToDecimal(Decimal.Round(Convert.ToDecimal(oDetail[0, 0]), 0) * Convert.ToDecimal(oDetail[0, 1]));

                                    }
                                    oTradeHistoryDetails.OrderPriceFilled = Convert.ToString(Decimal.Round(nTotalDollarsExecuted / nTotalSharesExecuted, 2));
                                    oTradeHistoryDetails.OrderQuantityFilled = Convert.ToString(nTotalSharesExecuted);
                                }
                                else
                                {
                                    oTradeHistoryDetails.OrderPriceFilled = "0.00";
                                    oTradeHistoryDetails.OrderQuantityFilled = "0";

                                }

                                oReplyDetails.Add(oTradeHistoryDetails);
                            }
                        }
                    }
                }
            }
        }



        #endregion

    }

}