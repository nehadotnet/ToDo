using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoWebApi.Models.Auth
{
    public class RefreshTokenResponseModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ErrorMsg { get; set; }
    }
}