using Bb;
using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;
using Bb.ComponentModel;
using System.Reflection;
using NLog;
using Black.Beard.Configuration.Git;

namespace Site.Loaders.SiteExtensions
{


    public static class ConfigurationExtension
    {

        static ConfigurationExtension()
        {
            _logger = LogManager.GetLogger(nameof(ConfigurationExtension));
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
            builder.WebHost.LoadConfiguration();
            return builder;
        }

        public static void LoadConfiguration(this ConfigureWebHostBuilder builder)
        {

            var paths = new List<string>
            {
                "Configs"
            };

            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {

                config.LoadConfigurationFile(paths.ToArray(), null, null)
                      .ConfigureApplication(hostingContext);

            });

        }


        public static IConfigurationBuilder ConfigureApplication(this IConfigurationBuilder config, WebHostBuilderContext hostingContext)
        {

            var provider = new LocalServiceProvider()
                .Add(typeof(WebHostBuilderContext), hostingContext)
                .Add(typeof(Logger), _logger)
                ;

            var loader = new InjectionLoader<IConfigurationBuilder>(ConstantsCore.Initialization, provider)
                .LoadModules()
                .Execute(config);

            return config;

        }


        public static IConfigurationBuilder LoadConfigurationFile(this IConfigurationBuilder config,
            string[] paths,
            string pattern = null,
            Func<FileInfo, bool> filter = null)
        {


            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var contentRootPath = Assembly.GetEntryAssembly()
                .Location
                .AsFile()
                .Directory.FullName;

            var dirs = new List<DirectoryInfo>()
            {
                contentRootPath.AsDirectory()
            };

            if (paths != null)
                foreach (var path in paths)
                {

                    DirectoryInfo c;

                    if (path.FilePathIsAbsolute())
                        c = path.AsDirectory();
                    else
                    {
                        var p = contentRootPath;
                        if (!string.IsNullOrEmpty(path))
                            p = p.Combine(path);
                        c = p.AsDirectory();
                    }

                    if (c != null && c.Exists)
                        dirs.Add(c);

                }

            if (string.IsNullOrEmpty(pattern))
                pattern = $"*.{environmentName}.json";

            foreach (var dir in dirs)
            {
                foreach (var file in dir.GetFiles(pattern))
                    if (filter == null || filter(file))
                    {
                        config.AddJsonFile(file.FullName, optional: false, reloadOnChange: false);
                        _logger.Debug($"configuration file {file.FullName} is loaded.");
                    }
            }


            config.AddUserSecrets(Assembly.GetEntryAssembly())
                  .AddEnvironmentVariables()
            //.AddCommandLine(_args)
            ;

            return config;

        }

        public static bool FilePathIsAbsolute(this string path)
        {

            if (!string.IsNullOrEmpty(path))
            {
                var f = path.AsFile();
                if (f.FullName == path)
                    return true;
            }

            return false;

        }

        public static bool DirectoryPathIsAbsolute(this string path)
        {

            if (!string.IsNullOrEmpty(path))
            {
                var f = path.AsDirectory();
                if (f.FullName == path)
                    return true;
            }

            return false;

        }

        private static readonly Logger _logger;

    }
}
