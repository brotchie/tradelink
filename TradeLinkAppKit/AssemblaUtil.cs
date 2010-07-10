using System;
using System.Collections.Generic;
using TradeLink.Common;
using TradeLink.API;
using Ionic.Zip;

namespace TradeLink.AppKit
{
    public class AssemblaUtil
    {
        /// <summary>
        /// upload todays tickdata to a portal
        /// </summary>
        /// <param name="space"></param>
        /// <param name="un"></param>
        /// <param name="pw"></param>
        /// <param name="ticket"></param>
        /// <param name="workpath"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static bool UploadTodaysTicks(string space, string un, string pw, int ticket, string workpath, DebugDelegate debug)
        {
            bool r = true;
            try
            {
                List<string> files = TikUtil.GetFilesFromDate();
                string fn = workpath + "\\TickData." + Util.ToTLDate() + ".zip";
                r &= ZipFile(fn, files, string.Empty, debug);
                if (TradeLink.AppKit.AssemblaDocument.Create(space, un, pw, fn, ticket, false))
                {
                    if (debug != null)
                    {
                        debug("tick data upload succeeded for: " + fn);
                    }
                }
                else
                {
                    if (debug != null)
                    {
                        r &= false;
                        debug("tick data upload failed for: " + fn);
                    }
                }
                if (System.IO.File.Exists(fn))
                {
                    try
                    {
                        System.IO.File.Delete(fn);
                    }
                    catch (Exception ex)
                    {
                        r &= false;
                        if (debug != null)
                        {
                            debug("unable to delete local copy: " + fn);
                            debug(ex.Message + ex.StackTrace);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                r &= false;
                if (debug != null)
                {
                    debug("unknown error uploading ticks. ");
                    debug(ex.Message + ex.StackTrace);
                }
            }
            return r;
        }

        /// <summary>
        /// zip together a bunch of files
        /// </summary>
        /// <param name="zipfilepath"></param>
        /// <param name="filepaths"></param>
        /// <returns></returns>
        public static bool ZipFile(string zipfilepath, List<string> filepaths) { return ZipFile(zipfilepath, filepaths, string.Empty, null); }
        /// <summary>
        /// zip together a bunch of files, using specified path in the zip file
        /// </summary>
        /// <param name="zipfilepath"></param>
        /// <param name="filepaths"></param>
        /// <param name="prependpath"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static bool ZipFile(string zipfilepath, List<string> filepaths, string prependpath, DebugDelegate debug)
        {
            bool ok = true;
            try
            {
                ZipFile zf = new ZipFile(zipfilepath);
                foreach (string file in filepaths)
                    if ((file != null) && (file != string.Empty))
                    {
                        try
                        {
                            if (prependpath != string.Empty)
                                zf.AddFile(file, prependpath);
                            else
                                zf.AddFile(file);
                        }
                        catch (Exception ex)
                        {
                            ok &= false;
                            if (debug != null)
                            {
                                debug("exception zipping: " + zipfilepath + " containing: " + string.Join(",", filepaths.ToArray()));
                                debug("error: " + ex.Message + ex.StackTrace);
                            }
                        }
                    }
                zf.Save();
            }
            catch (Exception ex)
            {
                ok &= false;
                if (debug != null)
                {
                    debug("exception zipping: " + zipfilepath + " containing: " + string.Join(",", filepaths.ToArray()));
                    debug("error: " + ex.Message + ex.StackTrace);
                }
            }
            return ok;
        }
    }
}
