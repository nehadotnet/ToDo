using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoWebApi.Models.Auth.LoginWithOtp
{
    public class VerifyOtpRequestModel
    {
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string OTP { get; set; }
    }
}