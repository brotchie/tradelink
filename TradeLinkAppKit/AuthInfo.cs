using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.AppKit
{
    /// <summary>
    /// hold authentication information.
    /// </summary>
    public struct AuthInfo
    {
        /// <summary>
        /// program
        /// </summary>
        public string Program;
        /// <summary>
        /// username
        /// </summary>
        public string Username;
        /// <summary>
        /// password
        /// </summary>
        public string Password;
        /// <summary>
        /// isvalid auth information
        /// </summary>
        public bool isValid { get { return (Username != null) && (Password != null); } }
        public const string AuthFile = "\\_AuthInfo.txt";
        public static bool SetProgramAuth(string program, AuthInfo ai) { return SetProgramAuth(program, ai, null); }
        public static bool SetProgramAuth(string program, AuthInfo ai, DebugDelegate deb) { return SetProgramAuth(Common.Util.ProgramData(program),program, ai, deb); }
        public static bool SetProgramAuth(string basepath, string program, AuthInfo ai, DebugDelegate deb)
        {
            return SetAuthInfo(basepath+Auth.AuthFile, ai, deb);
        }
        public static bool SetAuthInfo(string file, AuthInfo ai) { return SetAuthInfo(file, ai, null); }
        public static bool SetAuthInfo(string file, AuthInfo ai, DebugDelegate deb)
        {
            try
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(file, false);
                sw.WriteLine(ai.Username);
                sw.WriteLine(ai.Password);
                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (deb != null)
                {
                    deb("error saving file: " + file + " with: " + ex.Message + ex.StackTrace);
                }
            }
            return false;
        }

        /// <summary>
        /// get authentication information from a file with username in first line and password in the second.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static AuthInfo GetAuthInfo(string filepath) { return GetAuthInfo(filepath, null); }
        public static AuthInfo GetAuthInfo(string filepath,DebugDelegate deb)
        {
            AuthInfo ai = new AuthInfo();
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(filepath);
                // skip user
                ai.Username = sr.ReadLine();
                // get password
                ai.Password = sr.ReadLine();
                sr.Close();
                return ai;
            }
            catch (Exception ex)
            {
                if (deb != null)
                {
                    deb("exception opening: "+filepath+" "+ex.Message + ex.StackTrace);
                }
            }
            return ai;
        }
        /// <summary>
        /// get authentication information in the program path of PROGRAM
        /// </summary>
        /// <param name="PROGRAM"></param>
        /// <returns></returns>
        public static AuthInfo GetProgramAuth(string PROGRAM) { AuthInfo ai = GetProgramAuth(TradeLink.Common.Util.ProgramData(PROGRAM), PROGRAM); ai.Program = PROGRAM; return ai; }
        public static AuthInfo GetProgramAuth(string basepath, string PROGRAM)
        {
            string filepath = basepath+ TradeLink.AppKit.Auth.AuthFile;
            AuthInfo ai = GetAuthInfo(filepath);
            // set program
            ai.Program = PROGRAM;
            return ai;
        }
    }
}
