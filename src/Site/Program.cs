using Bb.Configuration;
using Bb.ComponentModel.Loaders;
using Site.Loaders.SiteExtensions;


// Run all initializer in the libs referenced
//      - download configuration from git   (Black.Beard.Configuration.Git) ConfigLoaderInitializer
//      - Init logger Nlog                  (Black.Beard.Logging.NLog)      NLogInitializer
//          - NLogInitializer initialize Nlog & redirect trace on Nlog tracer
string typeToExcludeInProviderService = "Bb.Wizards.WizardModel;Bb.UIComponents.Guards.GuardPolicy";
var provider = _Initializer.PrepareInitialization(typeToExcludeInProviderService);


// Create a new web application
var builder = WebApplication
    .CreateBuilder(args)
    .LoadConfiguration()
    .ConfigureTrace()
    ;

// Append the services
builder.Initialize(provider);


// Load & configure the services
var app = builder.Build();
app.Initialize(app.Services);

app.Run();