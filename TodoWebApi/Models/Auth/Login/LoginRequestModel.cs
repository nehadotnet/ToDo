using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TodoWebApi.Models.Auth
{
    public class LoginRequestModel
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}