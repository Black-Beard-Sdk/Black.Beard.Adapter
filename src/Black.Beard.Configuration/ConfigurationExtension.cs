using Bb;
using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;
using Bb.ComponentModel;
using System.Reflection;
using System.Diagnostics;

namespace Bb.Configuration
{


    public static class ConfigurationExtension
    {

        static ConfigurationExtension()
        {

        }

        /// <summary>
        /// Load configuration and discover all methods for loading configuration
        /// </summary>
        /// <param name="builder"><see cref="WebApplicationBuilder"/> </param>
        /// <example>
        /// <code lang="Csharp">
        /// var builder = WebApplication.CreateBuilder(args).LoadConfiguration();
        /// </code>
        /// If you want adding configuration append a new class with the attribute <see cref="ExposeClassAttribute"/> and implement the interface <see cref="IInjectBuilder{IConfigurationBuilder}"/>
        /// <code lang="Csharp">
        /// [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<IConfigurationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
        /// public class ConfigurationInitializer : IInjectBuilder<IConfigurationBuilder>
        /// {
        /// 
        ///     public string FriendlyName => typeof(ConfigurationInitializer).Name;
        ///     
        ///     public Type Type => typeof(ConfigurationInitializer);
        /// 
        ///     public object Execute(object context)
        ///     {
        ///         return Execute((IConfigurationBuilder) context);
        ///     }
        ///     
        ///     public bool CanExecute(object context)
        ///     {
        ///         return CanExecute((IConfigurationBuilder)context);
        ///     }
        /// 
        ///     public bool CanExecute(IConfigurationBuilder context)
        ///     {
        ///         var builtConfig = context.Build();
        ///         var canExecute = builtConfig["Initializer:" + FriendlyName];
        ///         if (canExecute != null)
        ///             if (!Convert.ToBoolean(canExecute))
        ///                 return false;
        ///         // place your code here
        ///         return true;
        ///     }
        /// 
        ///     public object Execute(IConfigurationBuilder context)
        ///     {
        ///         // place your code here
        ///         return context;
        ///     }
        ///     
        /// }
        /// </code>
        /// If you want desactivate a configuration loader, you can add a key in your configuration file like appsettings.json
        /// <code lang="json">
        /// "Initializer": {
        ///   "ConfigurationGitBuilderInitializer": true,
        ///   "ConfigurationVaultBuilderInitializer": true,
        /// },
        /// </code>
        /// </example>
        /// <returns></returns>
        public static WebApplicationBuilder LoadConfiguration(this WebApplicationBuilder builder)
        {

            //ILogger logger = LoggerFactory.Create(builder =>
            //{
            //}).CreateLogger<WebApplicationBuilder>();

            //logger.LogTrace("Load configuration");

            builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.LoadConfiguration(null, null);   // Load all files in the paths.
                config.ConfigureApplication(hostingContext, builder);       // Resolve all injection class for loading configuration
            });

            return builder;

        }

        public static IConfigurationBuilder LoadConfiguration(this IConfigurationBuilder builder, string pattern = null,
            Func<FileInfo, bool> filter = null)
        {

            builder.LoadConfigurationFile(pattern, filter)   // Load all files in the paths.
                  .AddUserSecrets(Assembly.GetEntryAssembly())
                  .AddCommandLine(Environment.GetCommandLineArgs())
                  .AddEnvironmentVariables()
                  .AddCommandLine(Environment.GetCommandLineArgs())
                  ;

            return builder;
        }

        private static IConfigurationBuilder ConfigureApplication(this IConfigurationBuilder config, WebHostBuilderContext hostingContext, WebApplicationBuilder builder)
        {

            config.Initialize(new LocalServiceProvider(builder.Services.BuildServiceProvider())
                .Add(typeof(WebHostBuilderContext), hostingContext));

            return config;

        }

        private static IConfigurationBuilder LoadConfigurationFile(this IConfigurationBuilder config,
            string pattern = null,
            Func<FileInfo, bool> filter = null)
        {

            var files = new ConfigurationLoader(pattern);

            foreach (var file in files)
            {

                var c = file.Count();
                FileInfo f = null;
                if (c == 1)
                    f = file.FirstOrDefault().FileInfo;

                else if (c == 2)
                {                   
                    
                    var f1 = file.FirstOrDefault(c => string.IsNullOrEmpty(c.Environment));
                    if (f1.FileInfo != null)
                        Load(config, f1.FileInfo);

                    var f2 = file.FirstOrDefault(c => !string.IsNullOrEmpty(c.Environment));
                    if (f2.FileInfo != null)
                        Load(config, f2.FileInfo);

                }

                if (f != null)
                    Load(config, f);

            }

            return config;

        }

        private static void Load(IConfigurationBuilder config, FileInfo f)
        {

            var type = FileContentTypeDetector.DetectFileType(f);
            switch (type)
            {

                case "JSON":
                    config.AddJsonFile(f.FullName, optional: false, reloadOnChange: false);
                    Trace.WriteLine($"configuration file {f.FullName} is loaded.");
                    break;

                case "XML":
                    config.AddXmlFile(f.FullName, optional: false, reloadOnChange: false);
                    Trace.WriteLine($"configuration file {f.FullName} is loaded.");
                    break;

                case "INI":
                    config.AddIniFile(f.FullName, optional: false, reloadOnChange: false);
                    Trace.WriteLine($"configuration file {f.FullName} is loaded.");
                    break;

                case "PerKey":
                    config.AddKeyPerFile(f.FullName, optional: false, reloadOnChange: false);
                    Trace.WriteLine($"configuration file {f.FullName} is loaded.");
                    break;

                default:
                    Trace.WriteLine($"configuration file {f.FullName} is is not recognized.");
                    break;

            }
        }

        private static bool FilePathIsAbsolute(this string path)
        {

            if (!string.IsNullOrEmpty(path))
            {
                var f = path.AsFile();
                if (f.FullName == path)
                    return true;
            }

            return false;

        }

        private static bool DirectoryPathIsAbsolute(this string path)
        {

            if (!string.IsNullOrEmpty(path))
            {
                var f = path.AsDirectory();
                if (f.FullName == path)
                    return true;
            }

            return false;

        }

    }

}
