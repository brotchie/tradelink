using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace TradeLib
{
    public static class Email
    {
        public static void Send(string to, string from, string subject, string message)
        {
            Send(to, from, subject, message, new SendCompletedEventHandler(s_SendCompleted));
        }
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
