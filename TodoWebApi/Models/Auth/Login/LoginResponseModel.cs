using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoWebApi.Models.Auth
{
    public class LoginResponseModel
    {
        public int status { get; set; }
        public string message { get; set; }
        public User data { get; set; }
        public string errorMsg { get; set; }
    }

    public class User
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