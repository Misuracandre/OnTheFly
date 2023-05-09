using Microsoft.Extensions.Options;
using OnTheFlyApp.PassengerService.config;
using OnTheFlyApp.PassengerService.Service;
using Utility;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configuration Singleton and AppSeting parameters.
builder.Services.Configure<PassengerServiceSettings>(builder.Configuration.GetSection("PassengerServiceSettings"));
builder.Services.AddSingleton<IPassengerServiceSettings>(s => s.GetRequiredService<IOptions<PassengerServiceSettings>>().Value);
builder.Services.AddSingleton<PassengersService>();
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
