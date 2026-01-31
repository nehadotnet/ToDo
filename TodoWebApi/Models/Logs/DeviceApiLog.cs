using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoWebApi.Models.Logs
{
    public class DeviceApiLog
    {
        public string DeviceId { get; set; }
        public string RequestId { get; set; }

        public string Method { get; set; }
        public string Endpoint { get; set; }
        public string BaseUrl { get; set; }

        public Dictionary<string, IEnumerable<string>> RequestHeaders { get; set; }
        public string RequestBody { get; set; }

        public int StatusCode { get; set; }
        public string ResponseBody { get; set; }

        public long ResponseTimeMs { get; set; }
        public DateTime Timestamp { get; set; }
    }
}