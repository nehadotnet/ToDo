using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoWebApi.Models.Auth
{
    public class RefreshTokenRequestModel
    {
        public string RefreshToken { get; set; }
    }
}