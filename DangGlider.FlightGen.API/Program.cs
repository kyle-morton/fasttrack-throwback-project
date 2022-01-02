using DangGlider.FlightGen.API;
using DangGlider.FlightGen.API.Hubs;
using DangGlider.FlightGen.Core.Data;
using DangGlider.FlightGen.Core.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FlightGenDbContext>(options =>
    //options.UseSqlServer(connectionString)
    options.UseInMemoryDatabase("DangGliderFlightGen")
);


builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddHostedService<FlightBackgroundService>();

builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<FlightHub>("/hubs/flight");
    endpoints.MapControllers();
});

DbInitializer.Populate(app, app.Environment);

app.Run();
