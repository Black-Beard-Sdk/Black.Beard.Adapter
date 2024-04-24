using Bb;
using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;
using Microsoft.AspNetCore.Components;
using Site.SiteExtensions;


ObjectCreatorByIoc.SetInjectionAttribute<InjectAttribute>();
var Logger = Loggers.InitializeLogger();

Assemblies.Load();

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