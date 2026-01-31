using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoWebApi.Models.Common
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string ErrorMsg { get; set; }
        public T Data { get; set; }
    }
}