using Bb;
using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;
using Bb.ComponentModel;
using System.Reflection;

namespace Site.Loaders.SiteExtensions
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

            var paths = new List<string>
            {
                "Configs"
            };

            builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
            {

                config.LoadConfigurationFile(paths.ToArray(), null, null)   // Load all files in the paths.
                      .AddUserSecrets(Assembly.GetEntryAssembly())
                      .AddCommandLine(Environment.GetCommandLineArgs())
                      .AddEnvironmentVariables();

                config.ConfigureApplication(hostingContext, builder);       // Resolve all injection class for loading configuration
                
            });

            return builder;
        }


        private static IConfigurationBuilder ConfigureApplication(this IConfigurationBuilder config, WebHostBuilderContext hostingContext, WebApplicationBuilder builder)
        {

            config.Initialize(new LocalServiceProvider(builder.Services.BuildServiceProvider())
                .Add(typeof(WebHostBuilderContext), hostingContext));

            return config;

        }

        private static IConfigurationBuilder LoadConfigurationFile(this IConfigurationBuilder config,
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
                        Console.WriteLine($"configuration file {file.FullName} is loaded.");
                    }
            }

            return config;

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
