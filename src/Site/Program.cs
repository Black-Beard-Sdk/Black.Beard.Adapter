using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;
using Microsoft.AspNetCore.Components;
using Site.Loaders;
using Site.Loaders.SiteExtensions;


/*

With the git configuration you can store your configuration in spearated store branch. the configuration will be downloaded and loaded. 
For configure git add the following configuration in the appsettings.json file or in environment variables or in the command line
    - GitRemoteUrl : the url of the git repository
    - GitUserName : the user name
    - GitEmail : the user email
    - GitPassword : the user password
    - GitBranch : the branch to use. by default the main branch is used.



 */


ObjectCreatorByIoc.SetInjectionAttribute<InjectAttribute>();
Assemblies.Load(); // Ensure all required assemblies are loaded


var builder = WebApplication.CreateBuilder(args)
                            .LoadConfiguration()
                            .ConfigureTrace();

var provider = new LocalServiceProvider(builder.Services.BuildServiceProvider());

builder.Initialize(null);

var app = builder.Build()
                 .Initialize()
                 ;


app.Run();