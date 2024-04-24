using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;

namespace Bb.Loaders
{



    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IApplicationBuilderInitializer<WebApplicationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class ModuleInitializeWebApplicationBuilder : ApplicationInitializerBase<WebApplicationBuilder>
    {
               
        public override void Execute(WebApplicationBuilder builder)
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


        }

    }

}
