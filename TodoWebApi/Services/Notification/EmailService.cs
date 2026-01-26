using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace TodoWebApi.Services.Notification
{
    public static class EmailService
    {
        public static void SendOtp(string toEmail,string otp)
        {
            var fromEmail = ConfigurationManager.AppSettings["SmtpEmail"];
            var fromPassword = ConfigurationManager.AppSettings["SmtpPAssword"];
            string host = ConfigurationManager.AppSettings["SmtpHost"];
            int port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);

            MailMessage mail = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = "Your Logon OTP",
                Body = $"Your OTP is {otp}.Valid for 5 minutes"
            };

            mail.To.Add(toEmail);

            SmtpClient smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true
            };

            smtpClient.Send(mail);      
        }
    }
}