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
            SmtpClient s = new SmtpClient("smtp.gmail.com");
            s.Credentials = new System.Net.NetworkCredential("tradelinkmail", "trad3l1nkma1l");
            s.SendAsync(to, from, subject, message, null);
            s.SendCompleted += new SendCompletedEventHandler(s_SendCompleted);
        }

        static void s_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

    }
}
