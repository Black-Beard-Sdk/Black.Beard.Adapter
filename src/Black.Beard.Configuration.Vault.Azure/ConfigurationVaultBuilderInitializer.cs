using Azure.Identity;
using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Site.Loaders
{


    /// <summary>
    /// Load configuration from Vault
    /// </summary>
    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<IConfigurationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class ConfigurationVaultBuilderInitializer : IInjectBuilder<IConfigurationBuilder>
    {


        public ConfigurationVaultBuilderInitializer()
        {

        }

        public string FriendlyName => typeof(ConfigurationVaultBuilderInitializer).Name;

        public Type Type => typeof(ConfigurationVaultBuilderInitializer);

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

            _configuration = VaultConfiguration.GetFromConfiguration(builtConfig);
            var result = _configuration.IsValid();

            var r2 = result ? "will be executed" : "is not configured";
            Console.WriteLine($"{FriendlyName} {r2}");

            return result;

        }

        public object Execute(IConfigurationBuilder context)
        {

            var builtConfig = context.Build();
            var keyVaultEndpoint = new Uri(_configuration.VaultRemoteUrl);
            var credential = new DefaultAzureCredential();
            context.AddAzureKeyVault(keyVaultEndpoint, credential);

            return context;

        }

        private VaultConfiguration _configuration;

    }

}

