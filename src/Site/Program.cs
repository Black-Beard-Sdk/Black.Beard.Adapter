using Bb.ComponentModel;
using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;
using Bb.Logging.NLog;
using Microsoft.AspNetCore.Components;
using Site.Loaders;
using Site.Loaders.SiteExtensions;
using Site.Pages.Pages.Authentication;
using Site.Services;


/*

With the git configuration you can store your configuration in separated git branch. the configuration will be downloaded and loaded. 
For configure git add the following configuration in the appsettings.json file or in environment variables or in the command line
    - GitRemoteUrl : the url of the git repository
    - GitUserName : the user name
    - GitEmail : the user email
    - GitPassword : the user password
    - GitBranch : the branch to use. by default the main branch is used.

 */

// configuration Storage
var ConfigPaths = new List<string> { "Configs" };
var SchemasPaths = "Configs\\schemas";
var idTemplate = "http://Black.Beard.com/schema/{0}";

SchemaGenerator.Initialize(SchemasPaths, idTemplate);
ObjectCreatorByIoc.SetInjectionAttribute<InjectAttribute>();
Assemblies.Load(); // Ensure all required assemblies are loaded
Initializer.Initialize(args);

var builder = WebApplication.CreateBuilder(args)
                            .LoadConfiguration(ConfigPaths)
                            .ConfigureTrace();

// Pre load the services
var provider = new LocalServiceProvider(builder.Services.BuildServiceProvider());
builder.Initialize(provider);

// Load the services
provider = new LocalServiceProvider(builder.Services.BuildServiceProvider());
var app = builder.Build()
                 .Initialize(provider)
                 ;

app.Run();