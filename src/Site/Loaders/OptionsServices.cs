using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Microsoft.Extensions.Options;
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

                        _types = new List<(OptionsEnum, Type)>();

                        foreach (var service in _services)
                        {
                            if (service.ServiceType.IsGenericType && service.ServiceType.GetGenericTypeDefinition() == typeof(IConfigureOptions<>))
                            {
                                var optionsType = service.ServiceType.GenericTypeArguments[0];
                                var instance = services.GetService(optionsType);
                                if (instance != null)
                                    _types.Add((OptionsEnum.Configuration, optionsType));
                            }
                        }

                        _services = null;
                    }

            return _types.Where(c => c.Item1 == option).Select(c => c.Item2);

        }

        private IServiceCollection _services;
        private List<(OptionsEnum, Type)> _types;
        private volatile object _lock = new object();

    }


    public enum OptionsEnum
    {
        Configuration,
    }

}
