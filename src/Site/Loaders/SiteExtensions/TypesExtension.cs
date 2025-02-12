using Bb.ComponentModel;
using Bb.ComponentModel.Factories;

namespace Site.Loaders.SiteExtensions
{

    public static class TypesExtension
    {


        public static void AppendConfiguration(this ConfigurationManager configuration, LocalServiceProvider serviceProvider)
        {
            ConstantsCore.Configuration.DiscoverTypeExposedByAttribute(type =>
            {
                configuration.ResolveConfiguration(type, (t, n, c) =>
                {
                });
            });
        }


        public static WebApplicationBuilder SetAllIoc(this WebApplicationBuilder builder, Func<Type, string, bool> filter)
        {

            var services = builder.Services;

            services.AddOptions();
            services.AddSingleton(typeof(OptionsServices), new OptionsServices(services));


            // Auto discover all types with attribute [ExposeClass] and register in ioc.
            services.UseTypeExposedByAttribute(builder.Configuration, ConstantsCore.Configuration, filter, c =>
            {
                services.BindConfiguration(c, builder.Configuration); // Bind the configuration before register the type in the ioc
            });

            services.UseTypeExposedByAttribute(builder.Configuration, ConstantsCore.Model, filter, c =>
            {

            });

            services.UseTypeExposedByAttribute(builder.Configuration, ConstantsCore.Service, filter, c =>
            {

            });

            return builder;

        }

    }

}
