using Bb;
using NLog;
using NLog.Config;
using System.Collections;
using System.Text.RegularExpressions;

namespace Bb.Logging.NLog
{

    public static class Loggers
    {

        static Loggers()
        {
            DirectoryToTrace = Directory.GetCurrentDirectory().Combine("Logs");
        }


        public static Logger InitializeLogger()
        {

            string web_log_directory = "web_log_directory";

            // target folder where store logs
            DirectoryToTrace.CreateFolderIfNotExists();
            GlobalDiagnosticsContext.Set(web_log_directory, DirectoryToTrace);

            var configLogPath = Directory.GetCurrentDirectory().Combine("nlog.config");
            if (File.Exists(configLogPath))
            {

                var payload = configLogPath.LoadFromFile();
                var reg = new Regex("\\${gdc:\\w+}", RegexOptions.CultureInvariant | RegexOptions.Multiline);
                foreach (Match item in reg.Matches(payload))
                {
                    var variableName = item.Value.Substring(6, item.Value.Length - 7);
                    var v = Environment.GetEnvironmentVariable(variableName);
                    if (!string.IsNullOrEmpty(v))
                        GlobalDiagnosticsContext.Set(variableName, v);

                    else if (variableName != web_log_directory)
                    {

                        Console.WriteLine($"the variable '{variableName}' in the configuration file {configLogPath} can't be resolved");
                    
                    }
                }

                LogManager.Configuration = new XmlLoggingConfiguration(configLogPath);

            }

            // Initialize log
            var logger = LogManager
                .Setup()
                .SetupExtensions(s => { })
                .GetCurrentClassLogger()
                ;

            logger.Debug("log initialized");

            return logger;
        }

        public static string DirectoryToTrace { get; set; }

    }
}
