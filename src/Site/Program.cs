using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;
using Microsoft.AspNetCore.Components;
using Site.Loaders;
using Site.Loaders.SiteExtensions;

ObjectCreatorByIoc.SetInjectionAttribute<InjectAttribute>();
var Logger = Loggers.InitializeLogger();
Assemblies.Load(); // Ensure all required assemblies are loaded

var builder = WebApplication
    .CreateBuilder(args)
    .Initialize(null, c =>
    {
        //c.Map(Logger);
    })
    ;


var app = builder
    .Build()
    .Initialize()
    ;

app.Run();