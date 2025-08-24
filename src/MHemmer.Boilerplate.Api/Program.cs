using MHemmer.Boilerplate.Api.Setup;
using MHemmer.Boilerplate.Infra;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<DomainDbContext>();

builder.SetupTelemetry()
    .SetupHealthCheck<DomainDbContext>();

var app = builder.Build()
    .MapHealthCheck();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Run();
