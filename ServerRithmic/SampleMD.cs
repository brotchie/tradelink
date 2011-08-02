/*   =====================================================================

Copyright (c) 2011 by Omnesys Technologies, Inc.  All rights reserved.

Warning :
        This Software Product is protected by copyright law and international
        treaties.  Unauthorized use, reproduction or distribution of this
        Software Product (including its documentation), or any portion of it,
        may result in severe civil and criminal penalties, and will be
        prosecuted to the maximum extent possible under the law.

        Omnesys Technologies, Inc. will compensate individuals providing
        admissible evidence of any unauthorized use, reproduction, distribution
        or redistribution of this Software Product by any person, company or 
        organization.

This Software Product is licensed strictly in accordance with a separate
Software System License Agreement, granted by Omnesys Technologies, Inc., which
contains restrictions on use, reverse engineering, disclosure, confidentiality 
and other matters.

     =====================================================================   */

using System;
using System.Collections.Generic;
using System.Text;
using com.omnesys.omne.om;
using com.omnesys.rapi;

/*   =====================================================================   */

namespace SampleMDNamespace
     {
     /*   ================================================================   */

     class MyAdmCallbacks : AdmCallbacks
          {
          public override void Alert(AlertInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }
          }

     /*   ================================================================   */

     class MyCallbacks : RCallbacks
          {
          public bool LoggedIntoMd
               {
               get { return PRI_bLoggedIntoMd; }
               }

          public bool GotPriceIncrInfo
               {
               get { return PRI_bGotPriceIncrInfo; }
               }

          public MyCallbacks()
               {
               PRI_bLoggedIntoMd = false;
               PRI_bGotPriceIncrInfo = false;
               }

          public override void Alert(AlertInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);

               if (oInfo.AlertType == AlertType.LoginComplete &&
                   oInfo.ConnectionId == ConnectionId.MarketData)
                    {
                    PRI_bLoggedIntoMd = true;
                    }
               }

          public override void AskQuote(AskInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void BestAskQuote(AskInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void BestBidQuote(BidInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void BidQuote(BidInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void ClosePrice(ClosePriceInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void ClosingIndicator(ClosingIndicatorInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void EndQuote(EndQuoteInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void EquityOptionList(EquityOptionListInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void EquityOptionStrategyList(EquityOptionStrategyListInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }
          public override void ExchangeList(ExchangeListInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void HighPrice(HighPriceInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void LimitOrderBook(OrderBookInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void LowPrice(LowPriceInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void MarketMode(MarketModeInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void OpenPrice(OpenPriceInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void OptionList(OptionListInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void OpeningIndicator(OpeningIndicatorInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void PriceIncrUpdate(PriceIncrInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);

               if (oInfo.RpCode == 0)
                    {
                    PRI_bGotPriceIncrInfo = true;
                    }
               }

          public override void RefData(RefDataInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void SettlementPrice(SettlementPriceInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void Strategy(StrategyInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void StrategyList(StrategyListInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void TradeCondition(TradeInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void TradePrint(TradeInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void TradeVolume(TradeVolumeInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          /*   ----------------------------------------------------------------   */

          public override void TimeBar(TimeBarInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void TimeBarReplay(TimeBarReplayInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          public override void TradeReplay(TradeReplayInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               Console.Out.Write(sb);
               }

          /*   -----------------------------------------------------------   */

          public override void AccountList(AccountListInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               sb.AppendFormat("\n");
               System.Console.Out.Write(sb);
               }

          public override void PasswordChange(PasswordChangeInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               sb.AppendFormat("\n");
               System.Console.Out.Write(sb);
               }

          public override void ProductRmsList(ProductRmsListInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               sb.AppendFormat("\n");
               System.Console.Out.Write(sb);
               }

          public override void ExecutionReplay(ExecutionReplayInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               sb.AppendFormat("\n");
               System.Console.Out.Write(sb);
               }

          public override void LineUpdate(LineInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();

               oInfo.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }
          public override void OpenOrderReplay(OrderReplayInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               sb.AppendFormat("\n");
               System.Console.Out.Write(sb);
               }

          public override void OrderReplay(OrderReplayInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               sb.AppendFormat("\n");
               System.Console.Out.Write(sb);
               }

           public override void PnlReplay(PnlReplayInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               
               oInfo.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void PnlUpdate(PnlInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();

               oInfo.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void SodUpdate(SodReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          /*   -----------------------------------------------------------   */

          public override void BustReport(OrderBustReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void CancelReport(OrderCancelReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void FailureReport(OrderFailureReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void FillReport(OrderFillReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void ModifyReport(OrderModifyReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void NotCancelledReport(OrderNotCancelledReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void NotModifiedReport(OrderNotModifiedReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void OtherReport(OrderReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void RejectReport(OrderRejectReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void StatusReport(OrderStatusReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void TradeCorrectReport(OrderTradeCorrectReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void TriggerPulledReport(OrderTriggerPulledReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          public override void TriggerReport(OrderTriggerReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               System.Console.Out.Write(sb);
               }

          private bool PRI_bLoggedIntoMd;
          private bool PRI_bGotPriceIncrInfo;
          }

     /*   ================================================================   */

class Program
     {
     static void Main(string[] args)
          {
          string USAGE = "SampleMD adm_cnnct_pt " +
                         "user password md_cnnct_pt ts_cnnct_pt " +
                         "exchange symbol flags";

          if (args.Length < 8)
               {
               System.Console.Out.WriteLine(USAGE);
               return;
               }

          /*   -----------------------------------------------------------   */

          string sAdmCnnctPt = args[0];
          string sUser       = args[1];
          string sPassword   = args[2];
          string sMdCnnctPt  = args[3];
          string sTsCnnctPt  = args[4];
          string sExchange   = args[5];
          string sSymbol     = args[6];
          string sFlags      = args[7];

          /*   -----------------------------------------------------------   */

          MyAdmCallbacks oAdmCallbacks = new MyAdmCallbacks();
          MyCallbacks    oCallbacks    = new MyCallbacks();
          REngineParams  oParams       = new REngineParams();
          REngine        oEngine;

          /*   ----------------------------------------------------------   */
          /*   You may need to change some values, such as the CertFile     */
          /*   ----------------------------------------------------------   */

          oParams.AdmCnnctPt = sAdmCnnctPt;
          oParams.AppName      = "SampleMD.NET";
          oParams.AppVersion   = "1.0.0.0";
          oParams.AdmCallbacks = oAdmCallbacks;
          oParams.CertFile     = "c:\\data\\rithmiccerts\\RithmicCertificate.pk12";
          oParams.DmnSrvrAddr  = "rituz01000.01.rithmic.com:65000";
          oParams.DomainName   = "rithmic_uat_01_dmz_domain";
          oParams.LicSrvrAddr  = "rituz01000.01.rithmic.com:56000";
          oParams.LocBrokAddr  = "rituz01000.01.rithmic.com:64100";
          oParams.LoggerAddr   = "rituz01000.01.rithmic.com:45454";

          /*   -----------------------------------------------------------   */

          try
               {
               /*   ------------------------------------------------------   */
               /*   Instantiate the REngine.                                 */
               /*   ------------------------------------------------------   */

               oEngine = new REngine(oParams);

               /*   ------------------------------------------------------   */
               /*   Initiate the login.                                      */
               /*   ------------------------------------------------------   */

               oEngine.login(oCallbacks, 
                    sUser, 
                    sPassword, 
                    sMdCnnctPt, 
                    sTsCnnctPt, 
                    string.Empty,
                    string.Empty);

               /*   ------------------------------------------------------   */
               /*   After calling REngine::login, RCallbacks::Alert will be  */
               /*   called a number of times.  Wait for when the login to    */
               /*   the MdCnnctPt and TsCnnctPt is complete.  (See           */
               /*   MyCallbacks::Alert() for details).                       */
               /*   ------------------------------------------------------   */

               while (!oCallbacks.LoggedIntoMd)
                    {
                    System.Threading.Thread.Sleep(1000);
                    }

               /*   ------------------------------------------------------   */
               /*   Subscribe to the instrument of interest.  To express     */
               /*   interest in different types of updates, see              */
               /*   SubscriptionFlags.                                       */
               /*   ------------------------------------------------------   */

               oEngine.subscribe(sExchange, 
                    sSymbol, 
                    (SubscriptionFlags)Convert.ToInt32(sFlags), 
                    null);

               /*   ------------------------------------------------------   */
               /*   At this point the callback routines will start firing    */
               /*   on a different thread.  This main thread will wait       */
               /*   until a key is pressed before continuing.                */
               /*   ------------------------------------------------------   */

               Console.Read();

               /*   ------------------------------------------------------   */
               /*   We are done, so log out...                               */
               /*   ------------------------------------------------------   */

               oEngine.logout();

               /*   ------------------------------------------------------   */
               /*   and shutdown the REngine instance.                       */
               /*   ------------------------------------------------------   */

               oEngine.shutdown();
               }
          catch (OMException oEx)
               {
               System.Console.Out.WriteLine("error : {0}", oEx.Message);
               }
          catch (Exception e)
               {
               System.Console.Out.WriteLine("exception : {0}", e.Message);
               }

          /*   -----------------------------------------------------------   */

          return;
          }
     }
}
