using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoWebApi.Models.Auth
{
    public class LoginResponseModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

    }
}