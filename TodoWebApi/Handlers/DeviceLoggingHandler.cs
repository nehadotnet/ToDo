using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using TodoWebApi.Models.Logs;

namespace TodoWebApi.Handlers
{
    public class DeviceLoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Debug.WriteLine("DeviceLoggingHandler triggered for: " + request.RequestUri);

            // Read device-id
            var deviceId = request.Headers.Contains("device-id") ?
                            request.Headers.GetValues("device-id").FirstOrDefault() : null;

            if (string.IsNullOrEmpty(deviceId))
                return await base.SendAsync(request, cancellationToken);

            var requestId = Guid.NewGuid().ToString();
            var stopwatch = Stopwatch.StartNew();

            // Capture request body
            string requestBody = null;
            if (request.Content != null)
            {
                requestBody = await request.Content.ReadAsStringAsync();
            }

            // Execute API
            var response = await base.SendAsync(request, cancellationToken);

            stopwatch.Stop();

            // Capture response
            string responseBody = null;
            if (response.Content != null)
            {
                responseBody = await response.Content.ReadAsStringAsync();
            }

            // Create log
            var log = new DeviceApiLog
            {
                DeviceId = deviceId,
                RequestId = requestId,
                Method = request.Method.Method,
                Endpoint = request.RequestUri.AbsolutePath,
                BaseUrl = request.RequestUri.GetLeftPart(UriPartial.Authority),
                RequestHeaders = request.Headers.ToDictionary(h => h.Key, h => h.Value),
                RequestBody = requestBody,
                StatusCode = (int)response.StatusCode,
                ResponseBody = responseBody,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            };

            Debug.WriteLine("DeviceLoggingHandler Logs for: " + log);


            WriteLogToFile(log);

            return response;
        }

        private void WriteLogToFile(DeviceApiLog log)
        {
            string basePath = HttpContext.Current.Server.MapPath(WebConfigurationManager.AppSettings["DeviceLogFolder"]);

            string dateFolder = log.Timestamp.ToString("yyyy-MM-dd");
            string folderPath = Path.Combine(basePath, dateFolder);

            Debug.WriteLine("Device Log Base Path: " + folderPath);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, $"{log.DeviceId}.log");

            string json = JsonSerializer.Serialize(log);

            try
            {
                File.AppendAllText(filePath, json + Environment.NewLine);

                Debug.WriteLine("Device Log Base Path: " + filePath);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to write log: " + ex.Message);
            }
        }
    }
}