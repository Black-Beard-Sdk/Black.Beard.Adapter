using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using static MudBlazor.CategoryTypes;

namespace Site.Loaders
{


    public class OptionsServices
    {

        public OptionsServices(IServiceCollection services)
        {
            _services = services;
        }

        public IEnumerable<Type> Items(IServiceProvider services, OptionsEnum option)
        {

            if (_services != null)
                lock (_lock)
                    if (_services != null)
                    {

                        _types = new List<(OptionsEnum, Type, Type)>();

                        foreach (var service in _services)
                            if (service.ServiceType.IsGenericType && service.ServiceType.GetGenericTypeDefinition() == typeof(IConfigureOptions<>))
                            {
                                var optionsType = service.ServiceType.GenericTypeArguments[0];

                                var attribute = optionsType.GetCustomAttribute<ExposeClassAttribute>();
                                if (attribute != null)
                                {

                                    var instance = services.GetService(service.ServiceType);
                                    if (instance != null)
                                        _types.Add((OptionsEnum.Configuration, service.ServiceType, optionsType));
                             
                                }
                            }

                        _services = null;
                    
                    }

            return _types.Where(c => c.Item1 == option).Select(c => c.Item2);

        }

        private IServiceCollection _services;
        private List<(OptionsEnum, Type, Type)> _types;
        private volatile object _lock = new object();

    }


    public enum OptionsEnum
    {
        Configuration,
    }

}
