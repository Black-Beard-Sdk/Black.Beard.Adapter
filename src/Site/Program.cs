using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;
using Site.Loaders.SiteExtensions;

// Run all initializer in the libs
_Initializer.PrepareInitialization();

var builder = WebApplication.CreateBuilder(args)
                            .LoadConfiguration()
                            .ConfigureTrace();

// Pre-load the services
var provider = new LocalServiceProvider(builder.Services.BuildServiceProvider());
builder.Initialize(provider);

// Load the services
provider = new LocalServiceProvider(builder.Services.BuildServiceProvider());
var app = builder.Build()
                 .Initialize(provider)
                 ;

app.Run();