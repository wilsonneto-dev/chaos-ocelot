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
    const double failOdds = 0.5;
    var random = new Random();
    if (random.NextDouble() < failOdds)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(JsonSerializer.Serialize(new{ Message = "Simulated fail" }));
        return;
    }

    await next(); 
});

app.UseOcelot().Wait();

app.Run();
