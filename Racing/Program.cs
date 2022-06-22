//using Racing.Services;

using Infrastructure.Contracts;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Racing.Infrastructure.DataAccess;
using Racing.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddScoped<IDbContext, DbContext>(ctx => new DbContext("Data Source=racing.db"));
builder.Services.AddScoped<IRaceRepository, RaceRepository>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
    dbContext.Seed();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<RacingService>();
app.MapGet("/",
           () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.Run();