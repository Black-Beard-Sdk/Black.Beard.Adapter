using Bb.ComponentModel;

namespace Site.Loaders.SiteExtensions
{
    public static class TypesExtension
    {

        public static void SetIoc(this WebApplicationBuilder builder)
        {

            var services = builder.Services;
            services.AddSingleton(typeof(OptionsServices), new OptionsServices(services));

            // Auto discover all types with attribute [ExposeClass] and register in ioc.
            services
                .UseTypeExposedByAttribute(builder.Configuration, ConstantsCore.Configuration, c =>
                {
                    services.BindConfiguration(c, builder.Configuration); // Bind the configuration before register the type in the ioc
                })
                .UseTypeExposedByAttribute(builder.Configuration, ConstantsCore.Model, c =>
                {

                })
                .UseTypeExposedByAttribute(builder.Configuration, ConstantsCore.Service, c =>
                {

                });


        }

    }
}
