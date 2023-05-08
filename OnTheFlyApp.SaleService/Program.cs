using Microsoft.Extensions.Options;
using OnTheFlyApp.SaleService.config;
using OnTheFlyApp.SaleService.Services;
using Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configuration Singleton and AppSeting parameters.
builder.Services.Configure<SaleServiceSettings>(builder.Configuration.GetSection("SaleServiceSettings"));
builder.Services.AddSingleton<ISaleServiceSettings>(s => s.GetRequiredService<IOptions<SaleServiceSettings>>().Value);
builder.Services.AddSingleton<SalesService>();
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
