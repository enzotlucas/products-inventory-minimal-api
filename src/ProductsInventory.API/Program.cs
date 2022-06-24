var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationConfigurations(typeof(Program));

var app = builder.Build();

app.UseApplicationConfigurations();

app.Run();