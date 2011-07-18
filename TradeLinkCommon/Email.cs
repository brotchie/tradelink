using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// email from tradelink
    /// </summary>
    public static class Email
    {
        

        static DebugDelegate d;

        static void debug(string msg)
        {
            if (d != null)
                d(msg);
        }

        /// <summary>
        /// Sends the specified email.
        /// </summary>
        /// <param name="to">To address.</param>
        /// <param name="from">From address.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        public static void Send(string gmailuser, string password, string to, string subject, string message) { Send(gmailuser, password, to, subject, message, null); }
        public static void Send(string gmailuser, string password, string to, string subject, string message, TradeLink.API.DebugDelegate deb)
        {
            d = deb;
            try
            {
                MailMessage mail = new MailMessage();
                System.Net.NetworkCredential cred = new System.Net.NetworkCredential
    (gmailuser, password);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.From = new MailAddress(gmailuser);
                mail.IsBodyHtml = true;
                mail.Body = message;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Credentials = cred;
                smtp.Port = 587;
                smtp.SendCompleted += new SendCompletedEventHandler(s_SendCompleted);
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                debug("Error sending email from: " + gmailuser + " to: " + to + " subject: " + subject + " err: " + ex.Message + ex.StackTrace);
            }
        }
        static string data = "74726164656C696E6B6D61696C21";
        static string DATA = "74726164656C696E6B6D61696C";
        static void s_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            debug("Email sent. ("+e.Error+")");
        }

    }
}
