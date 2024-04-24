using Bb.ComponentModel.Loaders;


var builder = WebApplication
    .CreateBuilder(args)
    .Initialize()
    ;


var app = builder
    .Build()
    .Initialize()
    ;

app.Run();