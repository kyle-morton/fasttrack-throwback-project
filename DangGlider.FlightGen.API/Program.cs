using DangGlider.FlightGen.API;
using DangGlider.FlightGen.API.Hubs;
using DangGlider.FlightGen.Core.Data;
using DangGlider.FlightGen.Core.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FlightGenDbContext>(options =>
    //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    options.UseInMemoryDatabase("DangGliderFlightGen")
);


builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddHostedService<FlightBackgroundService>();

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
