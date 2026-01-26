using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoWebApi.Models.Auth.LoginWithOtp
{
    public class SendOtpResponseModel
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string ErrorMsg { get; set; }
    }
}