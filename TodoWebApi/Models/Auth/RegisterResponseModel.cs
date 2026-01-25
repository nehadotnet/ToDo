using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoWebApi.Models.Auth
{
    public class RegisterResponseModel
    {
        public int status { get; set; }
        public string message { get; set; }
        public string errorMsg { get; set; }
        public string loginMessage { get; set; }    
    }
}