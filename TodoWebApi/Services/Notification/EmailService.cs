using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using TodoWebApi.Enums;

namespace TodoWebApi.Services.Notification
{
    public static class EmailService
    {
        public static void SendOtp(string toEmail,string otp, OtpPurpose otpPurpose)
        {
            var fromEmail = ConfigurationManager.AppSettings["SmtpEmail"];
            var fromPassword = ConfigurationManager.AppSettings["SmtpPAssword"];
            string host = ConfigurationManager.AppSettings["SmtpHost"];
            int port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            string emailSubject = string.Empty;
            string emailBody = string.Empty;

            switch (otpPurpose)
            {
                case OtpPurpose.Login:
                    emailSubject = "Your Login OTP";
                    emailBody = $"Your login OTP is {otp}. It is valid for 5 minutes.";
                    break;

                case OtpPurpose.ForgetPassword:
                    emailSubject = "Reset Your Password";
                    emailBody = $"Your password reset OTP is {otp}. Do not share this with anyone.";
                    break;

                default:
                    emailSubject = "Your OTP";
                    emailBody = $"Your OTP is {otp}.";
                    break;
            }


            MailMessage mail = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = emailSubject,
                Body = emailBody
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