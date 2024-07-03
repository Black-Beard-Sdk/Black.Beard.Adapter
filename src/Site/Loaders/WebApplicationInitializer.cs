using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;
using Bb;

namespace Site.Loaders
{

    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<WebApplication>), LifeCycle = IocScopeEnum.Transiant)]
    public class WebApplicationInitializer : IInjectBuilder<WebApplication>
    {

        public string FriendlyName => typeof(WebApplicationInitializer).Name;

        public Type Type => typeof(WebApplication);

        public bool CanExecute(WebApplication context)
        {
            return true;
        }

        public bool CanExecute(object context)
        {
            return CanExecute((WebApplication)context);
        }

        public object Execute(WebApplication app)
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


            //var srv = app.Services.GetService<OptionsServices>();
            //var i = srv.Items(app.Services, OptionsEnum.Configuration).ToList();
            //foreach (var item in i)
            //{
            //    var s1 = app.Services.GetService(item);
            //}
            
            
            return null;

        }

        public object Execute(object context)
        {
            return Execute((WebApplication)context);
        }


    }

}
