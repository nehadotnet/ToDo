using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace TodoWebApi.Services.Notification
{
    public class SmsService
    {
        public static void SendOtp(string mobileNumber, string otp)
        {
            // TEMP: Mock SMS (for development)
            Debug.WriteLine($"[SMS] OTP {otp} sent to {97997979}");

            // Later replace with:
            // Twilio / Firebase / AWS SNS
        }
    }
}