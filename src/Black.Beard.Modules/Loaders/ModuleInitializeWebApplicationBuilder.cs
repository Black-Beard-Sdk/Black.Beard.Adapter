using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;

namespace Bb.Loaders
{



    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<WebApplicationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class ModuleInitializeWebApplicationBuilder : IInjectBuilder<WebApplicationBuilder>
    {
        public string FriendlyName => typeof(ModuleInitializeWebApplicationBuilder).Name;

        public Type Type => typeof(WebApplicationBuilder);

        public bool CanExecute(WebApplicationBuilder context)
        {
            return true;
        }

        public bool CanExecute(object context)
        {
            return CanExecute((WebApplicationBuilder)context);
        }

        public object Execute(WebApplicationBuilder builder)
        {

            var services = builder.Services;

            services.AddAuthorization(options =>
            {

                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole("Admin");
                });

                //options.AddPolicy("Admin", policy => policy.RequireRole("Admin").);
                //options.AddPolicy("User", policy => policy.RequireRole("User"));
                //options.AddPolicy("Guest", policy => policy.RequireRole("Guest"));

            });

            return null;

        }

        public object Execute(object context)
        {
            return Execute((WebApplicationBuilder)context);
        }
    }

}
