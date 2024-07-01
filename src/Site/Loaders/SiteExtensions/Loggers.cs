using Bb;
using NLog;
using System.Collections;

namespace Site.Loaders.SiteExtensions
{

    public static class Loggers
    {


        static Loggers()
        {
            DirectoryToTrace = Directory.GetCurrentDirectory().Combine("Logs");
        }


        public static Logger InitializeLogger()
        {

            // target folder where store logs
            DirectoryToTrace.CreateFolderIfNotExists();            
            GlobalDiagnosticsContext.Set("web_log_directory", DirectoryToTrace);


            // push environment variables in the log
            foreach (DictionaryEntry item in Environment.GetEnvironmentVariables())
                if (item.Key != null
                    && !string.IsNullOrEmpty(item.Key.ToString())
                    && item.Key.ToString().StartsWith("web_log_"))
                    GlobalDiagnosticsContext.Set(item.Key.ToString(), item.Value?.ToString());

            // load the configuration file
            var configLogPath = Directory.GetCurrentDirectory().Combine("nlog.config");
            if (File.Exists(configLogPath))
                LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(configLogPath);

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
