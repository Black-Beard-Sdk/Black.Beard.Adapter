using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;
using Bb;

namespace Site.Loaders
{

    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IApplicationBuilderInitializer<WebApplication>), LifeCycle = IocScopeEnum.Transiant)]
    public class WebApplicationInitializer : ApplicationInitializerBase<WebApplication>
    {

        public override void Execute(WebApplication app)
        {

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            int port = 5000;
            app.Urls
                .AddLocalhostUrlWithDynamicPort("localhost", ref port)
                .AddLocalhostSecureUrlWithDynamicPort("localhost", ref port);

        }

    }

}
