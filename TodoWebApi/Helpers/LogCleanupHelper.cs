using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace TodoWebApi.Helpers
{
    public class LogCleanupHelper
    {
        public static void CleanupOldLogs()
        {
            string basePath = HttpContext.Current.Server.MapPath(
                ConfigurationManager.AppSettings["DeviceLogFolder"]);

            if (!Directory.Exists(basePath))
                return;

            int retentionDays = int.Parse(
                ConfigurationManager.AppSettings["DeviceLogRetentionDays"]);

            foreach (var dir in Directory.GetDirectories(basePath))
            {
                string folderName = Path.GetFileName(dir);

                if (DateTime.TryParse(folderName, out DateTime folderDate))
                {
                    if (folderDate < DateTime.UtcNow.AddDays(-retentionDays))
                    {
                        try
                        {
                            Directory.Delete(dir, true);
                        }
                        catch (Exception ex)
                        {
                            // Optional: log deletion errors somewhere
                        }
                    }
                }
            }
        }
    }
}
