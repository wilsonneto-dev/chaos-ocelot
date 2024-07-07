using System.Text.Json;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("Ocelot.json");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOcelot();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.Use(async (context, next) =>
{
    // 0.3 -> 30%
    const double failOdds = 0.3;
    
    if (Random.Shared.NextDouble() < failOdds)
    {
        var logger = context.RequestServices.GetService<ILogger<Program>>();
        logger?.LogError("* Simulating Issue for route: {route}", context.Request.Path.Value);
        
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(JsonSerializer.Serialize(new{ Message = "Simulated fail" }));
        return;
    }

    await next(); 
});

app.UseOcelot().Wait();

app.Run();
