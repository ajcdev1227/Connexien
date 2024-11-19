using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Connexien.Lib
{
    public static class Exceptions
    {
        private static readonly ILogger _logger;

        static Exceptions()
        {
            string basedir = AppDomain.CurrentDomain.BaseDirectory;

            _logger = new LoggerConfiguration()
                    .WriteTo.RollingFile(pathFormat: basedir + "/Logs/connexien-{Date}.log")
                    .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces)
                    .CreateLogger();
        }

        public static void Log(Exception ex, string ip = "", string note = "")
        {
            try
            {
                _logger.Error(ex, note);

                using (var db = new ConnexienEntities())
                {
                    db.Errors.Add(new Error
                    {
                        Created = DateTime.UtcNow,
                        Exception = ex.Message,
                        InnerException = ex.InnerException != null ? ex.InnerException.Message : "",
                        Ip = ip,
                        Note = note
                    });
                    db.SaveChanges();
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error during logging exception");
            }
        }

        public static void Info(string message)
        {
            _logger.Information(message);
        }
    }
}
