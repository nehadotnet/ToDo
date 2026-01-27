using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TodoWebApi.Enums;

namespace TodoWebApi.Models.Auth.LoginWithOtp
{
    public class SendOtpRequestModel
    {
        public string Email { get; set; }
        public string Mobilenumber { get; set; }

        public OtpPurpose OtpPurpose { get; set; }
    }
}