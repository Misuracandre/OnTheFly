using Microsoft.Extensions.Options;
using OnTheFlyApp.AirCraftService.config;
using OnTheFlyApp.AirCraftService.Service;
using Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configuration Singleton and AppSeting parameters.
builder.Services.Configure<AirCraftServiceSettings>(builder.Configuration.GetSection("AirCraftServiceSettings"));
builder.Services.AddSingleton<IAirCraftServiceSettings>(s => s.GetRequiredService<IOptions<AirCraftServiceSettings>>().Value);
builder.Services.AddSingleton<AirCraftsService>();
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
