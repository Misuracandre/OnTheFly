using Microsoft.Extensions.Options;
using OnTheFlyApp.FlightService.Config;
using OnTheFlyApp.FlightService.Services;
using Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configuration Singleton and AppSeting parameters.
builder.Services.Configure<FlightServiceSettings>(builder.Configuration.GetSection("FlightServiceSettings"));
builder.Services.AddSingleton<IFlightServiceSettings>(s => s.GetRequiredService<IOptions<FlightServiceSettings>>().Value);
builder.Services.AddSingleton<FlightsService>();
builder.Services.AddSingleton<Util>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
