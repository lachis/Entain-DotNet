using API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc()
       .AddJsonTranscoding();

builder.Services.AddGrpcClient<Racing.Racing.RacingClient>(o => o.Address = new Uri("http://localhost:9000"));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRouting();

app.UseEndpoints(endpoints => { endpoints.MapGrpcService<RacingService>(); });

app.Run();