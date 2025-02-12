using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Bb.Configuration;

namespace Bb.Configuration.Git
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
    public class ConfigurationGitBuilderInitializer : InjectBuilderBase<IConfigurationBuilder>
    {

        public override bool CanExecute(IConfigurationBuilder context)
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

        public override object Execute(IConfigurationBuilder context)
        {

            // Download configuration from git
            var loader = new ConfigurationLoader(_configuration);
            loader.Refresh(_targetFolder);

            // Load downloaded configuration
            var paths = _targetFolder.AsDirectory().GetDirectories().Select(c => c.FullName);
            context.LoadConfiguration();

            return context;

        }


        private GitConfiguration _configuration;
        private readonly string _targetFolder;

    }

}
