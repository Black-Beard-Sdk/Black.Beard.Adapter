using Bb;
using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace Black.Beard.Configuration.Git
{


    /// <summary>
    /// Load configuration from git
    /// </summary>
    /// <example>
    /// If you want to load configuration from git, you must add variables in your environment
    /// GitRemoteUrl : the url of the git repository
    /// GitUserName : the user name
    /// GitEmail : the user email
    /// GitPassword : the user password
    /// GitBranch : the branch to use. by default the main branch is used.
    /// </example>
    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<IConfigurationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class ConfigurationGitBuilderInitializer : IInjectBuilder<IConfigurationBuilder>
    {


        public string FriendlyName => typeof(ConfigurationGitBuilderInitializer).Name;

        public Type Type => typeof(ConfigurationGitBuilderInitializer);

        public ConfigurationGitBuilderInitializer()
        {

            _targetFolder = Assembly.GetEntryAssembly()
                .Location
                .AsFile()
                .Directory.Combine("uploadedConfiguration");

        }

        public object Execute(object context)
        {
            return Execute((IConfigurationBuilder)context);
        }

        public bool CanExecute(object context)
        {
            return CanExecute((IConfigurationBuilder)context);
        }

        public bool CanExecute(IConfigurationBuilder context)
        {
            var builtConfig = context.Build();
            var canExecute = builtConfig["Initializer:" + FriendlyName];
            if (canExecute != null)
                if (!Convert.ToBoolean(canExecute))
                {
                    Console.WriteLine($"{FriendlyName} is deactivated");
                    return false;
                }

            _configuration = GitConfiguration.GetFromConfiguration(builtConfig);
            var result = _configuration.IsValid();

            var r2 = result ? "will be executed" : "is not configured";
            Console.WriteLine($"{FriendlyName} {r2}");

            return result;

        }

        public object Execute(IConfigurationBuilder context)
        {

            // Download configuration from git
            var loader = new ConfigurationLoader(_configuration);
            loader.Refresh(_targetFolder, _configuration.GitBranch ?? "main");

            // Load downloaded configuration
            var paths = _targetFolder.AsDirectory().GetDirectories().Select(c => c.FullName);
            context.LoadConfigurationFile(paths.ToArray(), null, null);

            return context;

        }


        private GitConfiguration _configuration;
        private readonly string _targetFolder;

    }

}
