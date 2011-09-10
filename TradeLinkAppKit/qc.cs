using System;
using System.Collections.Generic;
using SeasideResearch.LibCurlNet;
using System.Text;

namespace TradeLink.AppKit
{
    class qc
    {

        public static bool gomultipartpost(string url, string user, string password, string fname, string fpath, int ticketid,int maxwaitsec, bool showprogress, TradeLink.API.DebugDelegate deb, out string result)
        {
            debs = deb;
            Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL);

            Easy easy = new Easy();
            rresult = new StringBuilder();
            hasresult = false;

            MultiPartForm mf = new MultiPartForm();


            // <input name="frmFileOrigPath">
            mf.AddSection(CURLformoption.CURLFORM_COPYNAME, "document[name]",
                CURLformoption.CURLFORM_COPYCONTENTS, fname,
                CURLformoption.CURLFORM_END);



            // <input type="File" name="f1">
            mf.AddSection(CURLformoption.CURLFORM_COPYNAME, "document[file]",
                CURLformoption.CURLFORM_FILE, fpath,
                CURLformoption.CURLFORM_CONTENTTYPE, "application/octet-stream",
                CURLformoption.CURLFORM_END);

            // <input name="frmFileDate">
            if (ticketid != 0)
                mf.AddSection(CURLformoption.CURLFORM_COPYNAME, "document[ticket_id]",
                    CURLformoption.CURLFORM_COPYCONTENTS, ticketid.ToString(),
                    CURLformoption.CURLFORM_END);

            if (showprogress)
            {
                Easy.ProgressFunction pf = new Easy.ProgressFunction(OnProgress);
                easy.SetOpt(CURLoption.CURLOPT_PROGRESSFUNCTION, pf);
            }


            Easy.WriteFunction wf = new Easy.WriteFunction(OnWritePostData);
            easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, wf);
            easy.SetOpt(CURLoption.CURLOPT_HTTPPOST, mf);
            Easy.DebugFunction df = new Easy.DebugFunction(OnDebug);
            easy.SetOpt(CURLoption.CURLOPT_DEBUGFUNCTION, df);

            // simple post - with a string
            //easy.SetOpt(CURLoption.CURLOPT_POSTFIELDS,data);
            Slist sl = new Slist();
            //sl.Append("Content-Type:multipart/form-data");
            sl.Append("Accept: application/xml");
            easy.SetOpt(CURLoption.CURLOPT_HTTPHEADER, sl);
            easy.SetOpt(CURLoption.CURLOPT_SSL_VERIFYPEER, false);
            easy.SetOpt(CURLoption.CURLOPT_USERAGENT,
                "Mozilla 4.0 (compatible; MSIE 6.0; Win32");
            easy.SetOpt(CURLoption.CURLOPT_FOLLOWLOCATION, true);
            easy.SetOpt(CURLoption.CURLOPT_USERPWD, user + ":" + password);
            CURLhttpAuth authflag = CURLhttpAuth.CURLAUTH_BASIC;
            easy.SetOpt(CURLoption.CURLOPT_HTTPAUTH, authflag);
            easy.SetOpt(CURLoption.CURLOPT_URL, url);

            //easy.SetOpt(CURLoption.CURLOPT_POST, true);
            if (debs != null)
                easy.SetOpt(CURLoption.CURLOPT_VERBOSE, true);



            CURLcode err = easy.Perform();
            int waits = 0;
            int maxwaits = 100*maxwaitsec;
            while (!hasresult && (waits++ < maxwaits))
                System.Threading.Thread.Sleep(10);

            int rcodei = 0;
            CURLcode rcode = easy.GetInfo(CURLINFO.CURLINFO_RESPONSE_CODE, ref rcodei);


            if (!hasresult && (deb != null))
                deb(easy.StrError(err));

            easy.Cleanup();
            mf.Free();


            Curl.GlobalCleanup();
            result = rresult.ToString();



            return hasresult && (rcodei==201);
        }

        public static Int32 OnProgress(Object extraData, Double dlTotal,
    Double dlNow, Double ulTotal, Double ulNow)
        {
            if (debs != null)
            {
                string msg = string.Format("Progress: {0} {1} {2} {3}",
                dlTotal, dlNow, ulTotal, ulNow);
                debs(msg);
            }
            return 0; // standard return from PROGRESSFUNCTION
        }

        public static bool goget(string url, string user, string password, string data, TradeLink.API.DebugDelegate deb, out string result)
        {
            debs = deb;
            Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL);

            Easy easy = new Easy();
            rresult = new StringBuilder();
            hasresult = false;

            Easy.WriteFunction wf = new Easy.WriteFunction(OnWritePostData);
            easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, wf);
            Easy.DebugFunction df = new Easy.DebugFunction(OnDebug);
            easy.SetOpt(CURLoption.CURLOPT_DEBUGFUNCTION, df);

            // simple post - with a string
            Slist sl = new Slist();
            //sl.Append("Content-Type:application/xml");
            sl.Append("Accept: application/xml");
            easy.SetOpt(CURLoption.CURLOPT_HTTPHEADER, sl);
            easy.SetOpt(CURLoption.CURLOPT_SSL_VERIFYPEER, false);
            easy.SetOpt(CURLoption.CURLOPT_USERAGENT,
                "Mozilla 4.0 (compatible; MSIE 6.0; Win32");
            easy.SetOpt(CURLoption.CURLOPT_FOLLOWLOCATION, true);
            easy.SetOpt(CURLoption.CURLOPT_USERPWD, user + ":" + password);
            CURLhttpAuth authflag = CURLhttpAuth.CURLAUTH_BASIC;
            easy.SetOpt(CURLoption.CURLOPT_HTTPAUTH, authflag);
            easy.SetOpt(CURLoption.CURLOPT_URL, url);
            

            if (debs != null)
                easy.SetOpt(CURLoption.CURLOPT_VERBOSE, true);



            CURLcode err = easy.Perform();
            int waits = 0;
            int maxwaits = 200;
            while (!hasresult && (waits++ < maxwaits))
                System.Threading.Thread.Sleep(10);

            int rcodei = 0;
            CURLcode rcode = easy.GetInfo(CURLINFO.CURLINFO_RESPONSE_CODE, ref rcodei);


            if (!hasresult && (deb != null))
                deb(easy.StrError(err));

            easy.Cleanup();



            Curl.GlobalCleanup();
            result = rresult.ToString();



            return hasresult;
        }

        public static bool goput(string url, string user, string password, string data, TradeLink.API.DebugDelegate deb, out string result)
        {
            debs = deb;
            Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL);

            Easy easy = new Easy();
            rresult = new StringBuilder();
            hasresult = false;

            Easy.WriteFunction wf = new Easy.WriteFunction(OnWritePostData);
            easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, wf);
            Easy.DebugFunction df = new Easy.DebugFunction(OnDebug);
            easy.SetOpt(CURLoption.CURLOPT_DEBUGFUNCTION, df);

            // simple post - with a string
            Slist sl = new Slist();
            sl.Append("Content-Type:application/xml");
            sl.Append("Accept: application/xml");
            easy.SetOpt(CURLoption.CURLOPT_HTTPHEADER, sl);
            easy.SetOpt(CURLoption.CURLOPT_SSL_VERIFYPEER, false);
            easy.SetOpt(CURLoption.CURLOPT_USERAGENT,
                "Mozilla 4.0 (compatible; MSIE 6.0; Win32");
            easy.SetOpt(CURLoption.CURLOPT_FOLLOWLOCATION, true);
            easy.SetOpt(CURLoption.CURLOPT_USERPWD, user + ":" + password);
            CURLhttpAuth authflag = CURLhttpAuth.CURLAUTH_BASIC;
            easy.SetOpt(CURLoption.CURLOPT_HTTPAUTH, authflag);
            easy.SetOpt(CURLoption.CURLOPT_URL, url);
            easy.SetOpt(CURLoption.CURLOPT_UPLOAD,true);

            if (debs != null)
                easy.SetOpt(CURLoption.CURLOPT_VERBOSE, true);



            CURLcode err = easy.Perform();
            int waits = 0;
            int maxwaits = 200;
            while (!hasresult && (waits++ < maxwaits))
                System.Threading.Thread.Sleep(10);

            int rcodei = 0;
            CURLcode rcode = easy.GetInfo(CURLINFO.CURLINFO_RESPONSE_CODE, ref rcodei);


            if (!hasresult && (deb != null))
                deb(easy.StrError(err));

            easy.Cleanup();



            Curl.GlobalCleanup();
            result = rresult.ToString();



            return hasresult;
        }

        static TradeLink.API.DebugDelegate debs;
        public static bool gopost(string url, string user, string password,  string data, TradeLink.API.DebugDelegate deb, out string result)
        {
            debs = deb;
            Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL);

            Easy easy = new Easy();
            rresult = new StringBuilder();
            hasresult = false;

            Easy.WriteFunction wf = new Easy.WriteFunction(OnWritePostData);
            easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, wf);
            Easy.DebugFunction df = new Easy.DebugFunction(OnDebug);
            easy.SetOpt(CURLoption.CURLOPT_DEBUGFUNCTION, df);

            // simple post - with a string
            easy.SetOpt(CURLoption.CURLOPT_POSTFIELDS,
                data);
            Slist sl = new Slist();
            sl.Append("Content-Type:application/xml");
            sl.Append("Accept: application/xml");
            easy.SetOpt(CURLoption.CURLOPT_HTTPHEADER, sl);
            easy.SetOpt(CURLoption.CURLOPT_SSL_VERIFYPEER, false);
            easy.SetOpt(CURLoption.CURLOPT_USERAGENT,
                "Mozilla 4.0 (compatible; MSIE 6.0; Win32");
            easy.SetOpt(CURLoption.CURLOPT_FOLLOWLOCATION, true);
            easy.SetOpt(CURLoption.CURLOPT_USERPWD, user + ":" + password);
            CURLhttpAuth authflag = CURLhttpAuth.CURLAUTH_BASIC;
            easy.SetOpt(CURLoption.CURLOPT_HTTPAUTH, authflag);
            easy.SetOpt(CURLoption.CURLOPT_URL,url);
            
            easy.SetOpt(CURLoption.CURLOPT_POST, true);
            if (debs!=null)
                easy.SetOpt(CURLoption.CURLOPT_VERBOSE, true);
            


            CURLcode err = easy.Perform();
            int waits = 0;
            int maxwaits = 200;
            while (!hasresult && (waits++<maxwaits))
                System.Threading.Thread.Sleep(10);

            int rcodei = 0;
            CURLcode rcode = easy.GetInfo(CURLINFO.CURLINFO_RESPONSE_CODE, ref rcodei);


            if (!hasresult && (deb != null))
                deb(easy.StrError(err));

            easy.Cleanup();



            Curl.GlobalCleanup();
            result = rresult.ToString();



            return hasresult;
        }

        public static void OnDebug(CURLINFOTYPE infoType,
            String message, Object extraData)
        {
            if (debs != null)
            {
                string final = string.Empty ;
                debs(infoType + ": " + message + final);
            }
        }

        static StringBuilder rresult = new StringBuilder();
        static bool hasresult = false;
        public static Int32 OnWritePostData(Byte[] buf, Int32 size, Int32 nmemb,
        Object extraData)
        {
            string newdata = System.Text.Encoding.UTF8.GetString(buf);
            if (newdata != null)
            {
                rresult.Append(newdata);
                hasresult = true;
            }
            return size * nmemb;
        }


    }
}
