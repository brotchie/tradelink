using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace TradeLib
{
    public static class Email
    {
        /// <summary>
        /// Sends the specified email.
        /// </summary>
        /// <param name="to">To address.</param>
        /// <param name="from">From address.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        public static void Send(string to, string from, string subject, string message)
        {
            Send(to, from, subject, message, new SendCompletedEventHandler(s_SendCompleted));
        }
        /// <summary>
        /// Sends the specified email.
        /// </summary>
        /// <param name="to">To address.</param>
        /// <param name="from">From address.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="CompletedCallBack">The completed call back.</param>
        public static void Send(string to, string from, string subject, string message, SendCompletedEventHandler CompletedCallBack)
        {
            SmtpClient s = new SmtpClient("smtp.gmail.com");
            s.EnableSsl = true;
            s.Credentials = new System.Net.NetworkCredential("tradelinkmail", "trad3l1nkma1l");
            s.SendAsync(to, from, subject, message, null);
            s.SendCompleted += new SendCompletedEventHandler(s_SendCompleted);
        }

        static void s_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Email sent. ("+e.Error+")");
        }

    }
}
