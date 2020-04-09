using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.IO;

namespace QRCodeRedirects.BL
{
    public class ErrorHandling
    {
        public static void SendErrorNotification(string errorString)
        {
            MailMessage message = new MailMessage();
            message.To.Add("rdunnings@crestron.com");
            message.Subject = "QR Code Error Occurred!";
            message.From = new MailAddress("DO.NOT.REPLY@crestron.com");
            message.Body = errorString;
            message.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient("SMTP");
            smtp.Send(message);
            message.Dispose();
            smtp.Dispose();
        }
    }
}