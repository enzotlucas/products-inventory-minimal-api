var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationConfigurations(typeof(Program));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseApplicationConfigurations();

app.Run();