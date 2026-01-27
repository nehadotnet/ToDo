using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoWebApi.Models.Auth
{
    public class ResetPasswordRequestModel
    {
        public string LoginId { get; set; }    
        public string OTP { get; set; }
        public string NewPassword { get; set; }
    }
}