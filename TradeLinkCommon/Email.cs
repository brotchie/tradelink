using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace TradeLink.Common
{
    /// <summary>
    /// email from tradelink
    /// </summary>
    public static class Email
    {
        /// <summary>
        /// Sends the specified email.
        /// </summary>
        /// <param name="to">To address.</param>
        /// <param name="from">From address.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        public static void Send(string to, string subject, string message)
        {
            Send(to, subject, message, new SendCompletedEventHandler(s_SendCompleted));
        }
        /// <summary>
        /// Sends the specified email.
        /// </summary>
        /// <param name="to">To address.</param>
        /// <param name="from">From address.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="CompletedCallBack">The completed call back.</param>
        public static void Send(string to, string subject, string message, SendCompletedEventHandler CompletedCallBack)
        {
            SmtpClient s = new SmtpClient("smtp.gmail.com");
            s.EnableSsl = true;
            s.Credentials = new System.Net.NetworkCredential(Util.decode(data), Util.decode(DATA));
            s.SendCompleted += new SendCompletedEventHandler(s_SendCompleted);
            s.SendAsync(to, Util.decode(data)+"@gmail.com", subject, message, null);
        }
        static string data = "74726164656C696E6B6D61696C";
        static string DATA = "74726164336C31636B6D61316C";
        static void s_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Email sent. ("+e.Error+")");
        }

    }
}
