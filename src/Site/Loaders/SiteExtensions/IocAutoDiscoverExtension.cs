using Bb;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Factories;
using Site.Services;
using System.Diagnostics;
using System.Reflection;

namespace Site.Loaders.SiteExtensions
{


    public static class IocAutoDiscoverExtension
    {

        static IocAutoDiscoverExtension()
        {
            _methodRegister = typeof(IocAutoDiscoverExtension).GetMethod(nameof(AddType), BindingFlags.NonPublic | BindingFlags.Static);
            _methodOptionConfiguration = typeof(IocAutoDiscoverExtension).GetMethod(nameof(BindConfiguration), BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static void DiscoverTypeExposedByAttribute(this string contextKey, Action<Type> action = null)
        {

            if (action != null)
                foreach (var type in GetExposedTypes(contextKey))
                    action(type);

        }

        public static IServiceCollection UseTypeExposedByAttribute(this IServiceCollection services, IConfiguration configuration, string contextKey, Func<Type, string, bool> filter, Action<Type> action = null)
        {

            if (filter == null)
                filter = (c, d) => true;

            DiscoverTypeExposedByAttribute(contextKey, type =>
            {

                if (filter(type, contextKey))
                {

                    _methodRegister.MakeGenericMethod(type).Invoke(null, new object[] { services, configuration });

                    if (action != null)
                        action(type);
                }

            });

            return services;

        }

        public static IServiceCollection BindConfiguration(this IServiceCollection self, Type type, IConfiguration configuration)
        {
            _methodOptionConfiguration.MakeGenericMethod(type)
                .Invoke(self, new object[] { self, configuration });
            return self;
        }

        /// <summary>
        /// Gets the exposed types in loaded assemblies.
        /// </summary>
        /// <param name="contextName">Name of the context.</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetExposedTypes(string contextName)
        {
            var items = Bb.ComponentModel.TypeDiscovery.Instance
                .GetTypesWithAttributes<ExposeClassAttribute>(typeof(object), c => c.Context == contextName);
            return items;
        }

        public static IEnumerable<Type> GetExposedTypes(Func<ExposeClassAttribute, bool> filter)
        {
            var items = Bb.ComponentModel.TypeDiscovery.Instance
                .GetTypesWithAttributes<ExposeClassAttribute>(typeof(object), c => filter(c));
            return items;
        }


        private static void BindConfiguration<TOptions>(this IServiceCollection self, IConfiguration configuration)
            where TOptions : class
        {

            configuration.ResolveConfiguration(typeof(TOptions), (type, sectionName, section) =>
            {

                if (section != null)
                {
                    var opt = self.AddOptions<TOptions>();
                    opt.Bind(section)
                       .ValidateDataAnnotations();
                }

                else
                    Trace.TraceWarning("section {0} not found", sectionName);

            });

        }

        public static void ResolveConfiguration(this IConfiguration configuration, Type type, Action<Type, string, IConfigurationSection> action)
        {

            SchemaGenerator.GenerateSchema(type);

            var id = new Uri("http://example.com/schema/TestGeneratedSchema");
            var schema = type.GenerateSchemaForConfiguration(id);

            var attribute = type.GetCustomAttribute<ExposeClassAttribute>();
            var sectionName = !string.IsNullOrEmpty(attribute?.Name) ? attribute.Name : type.Name;

            var section = configuration.GetSection(sectionName);

            action(type, sectionName, section);

        }

        private static void AddType<T>(this IServiceCollection services, IConfiguration configuration)
            where T : class
        {
            if (typeof(IInitialize).IsAssignableFrom(typeof(T)))
            {
                var serviceBuilder = ObjectCreatorByIoc.GetActivator<T>();
                Func<IServiceProvider, T> _func = (serviceProvider) => serviceBuilder.Call(null, serviceProvider);
                services.RegisterType(_func);
            }
            else
                services.RegisterType<T>();
        }

        private static void RegisterType<T>(this IServiceCollection services, Func<IServiceProvider, T> func)
            where T : class
        {

            var attribute = typeof(T).GetCustomAttribute<ExposeClassAttribute>();
            var exposed = attribute?.ExposedType ?? typeof(T);

            switch (attribute.LifeCycle)
            {

                case IocScopeEnum.Transiant:
                    services.AddTransient(exposed, func);
                    break;

                case IocScopeEnum.Scoped:
                    services.AddScoped(exposed, func);
                    break;

                case IocScopeEnum.Singleton:
                default:
                    if (attribute.ExposedType != null)
                        exposed = attribute.ExposedType;
                    services.AddSingleton(exposed, func);
                    break;
            }

            Trace.TraceInformation("registered {contextModel} {type} exposed by {exposed} with lifecycle {lifeCycle}", attribute.Context, typeof(T).Name, exposed.Name, attribute.LifeCycle.ToString());

        }

        private static void RegisterType<T>(this IServiceCollection services)
            where T : class
        {

            var attribute = typeof(T).GetCustomAttribute<ExposeClassAttribute>();
            var exposed = attribute?.ExposedType ?? typeof(T);

            switch (attribute.LifeCycle)
            {

                case IocScopeEnum.Singleton:
                    if (attribute.ExposedType != null)
                        exposed = attribute.ExposedType;
                    services.AddSingleton(exposed, typeof(T));
                    break;

                case IocScopeEnum.Scoped:
                    services.AddScoped(exposed, typeof(T));
                    break;

                case IocScopeEnum.Transiant:
                default:
                    services.AddTransient(exposed, typeof(T));
                    break;

            }

            Trace.TraceInformation("registered {0} {1} exposed by {2} with lifecycle {3}"
                , attribute.Context
                , typeof(T).Name
                , exposed.Name
                , attribute.LifeCycle.ToString());

        }

        private static readonly MethodInfo? _methodRegister;
        private static readonly MethodInfo? _methodOptionConfiguration;
        private static readonly MethodInfo? _methodConfiguration;
        private static readonly MethodInfo? _methodModel;
        private static readonly MethodInfo? _methodService;

    }


}
