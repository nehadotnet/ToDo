using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using TodoWebApi.Models.Logs;

namespace TodoWebApi.Controllers
{
    [RoutePrefix("api/logs")]
    public class LogsController : ApiController
    {
        private string GetBasePath()
        {
            return HttpContext.Current.Server.MapPath(
                WebConfigurationManager.AppSettings["DeviceLogFolder"]);
        }

        [HttpGet]
        [Route("get_logs")]
        public HttpResponseMessage GetLogs(string date = null, string deviceid = null)
        {
            var basePath = GetBasePath();

            // 1️⃣ No params → show dates
            if (string.IsNullOrWhiteSpace(date))
            {
                var dates = Directory.Exists(basePath)
                    ? Directory.GetDirectories(basePath)
                        .Select(Path.GetFileName)
                        .OrderByDescending(d => d)
                        .ToList()
                    : Enumerable.Empty<string>();

                return BuildHtmlPage(
                    "Log Dates",
                    dates.Select(d =>
                        ($"/api/logs/get_logs?date={d}", d))
                );
            }

            var dateFolder = Path.Combine(basePath, date);

            if (!Directory.Exists(dateFolder))
                return Request.CreateResponse(System.Net.HttpStatusCode.NotFound, "Date not found");

            // 2️⃣ Date only → show devices
            if (string.IsNullOrWhiteSpace(deviceid))
            {
                var devices = Directory.GetFiles(dateFolder, "*.log")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .OrderBy(d => d)
                    .ToList();

                return BuildHtmlPage(
                    $"Devices for {date}",
                    devices.Select(d =>
                        ($"/api/logs/get_logs?date={date}&deviceid={d}", d)),
                    backUrl: "/api/logs/get_logs"
                );
            }

            // 3️⃣ Date + device → return JSON logs
            var filePath = Path.Combine(dateFolder, $"{deviceid}.log");

            if (!File.Exists(filePath))
                return Request.CreateResponse(System.Net.HttpStatusCode.NotFound, "Device log not found");

            var logs = File.ReadAllLines(filePath)
                .Select(line => JsonSerializer.Deserialize<DeviceApiLog>(line))
                .ToList();

            return Request.CreateResponse(System.Net.HttpStatusCode.OK, logs, Configuration.Formatters.JsonFormatter);
        }

        // 🧱 HTML builder (small + reusable)
        private HttpResponseMessage BuildHtmlPage(
            string title,
            IEnumerable<(string url, string text)> links,
            string backUrl = null)
        {
            var sb = new StringBuilder();

            sb.Append("<html><head>");
            sb.Append($"<title>{title}</title>");
            sb.Append("<style>");
            sb.Append("body{font-family:Arial;margin:40px}");
            sb.Append("ul{list-style:none;padding:0}");
            sb.Append("li{margin:8px 0}");
            sb.Append("a{text-decoration:none;color:#0066cc;font-size:16px}");
            sb.Append("a:hover{text-decoration:underline}");
            sb.Append("</style>");
            sb.Append("</head><body>");

            sb.Append($"<h2>{title}</h2>");

            if (backUrl != null)
                sb.Append($"<p><a href='{backUrl}'>← Back</a></p>");

            sb.Append("<ul>");
            foreach (var (url, text) in links)
            {
                sb.Append($"<li><a href='{url}'>{text}</a></li>");
            }
            sb.Append("</ul>");

            sb.Append("</body></html>");

            return new HttpResponseMessage
            {
                Content = new StringContent(sb.ToString(), Encoding.UTF8, "text/html")
            };
        }
    }
}
