using System;
using System.Collections.Generic;
using SeasideResearch.LibCurlNet;
using System.Text;

namespace TradeLink.AppKit
{
    class qc
    {

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
