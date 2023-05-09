using Microsoft.Extensions.Options;
using OnTheFlyApp.CompanyService.Config;
using OnTheFlyApp.CompanyService.Service;
using Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configuration Singleton and AppSeting parameters.
builder.Services.Configure<CompanyServiceSettings>(builder.Configuration.GetSection("CompanyServiceSettings"));
builder.Services.AddSingleton<ICompanyServiceSettings>(s => s.GetRequiredService<IOptions<CompanyServiceSettings>>().Value);
builder.Services.AddSingleton<CompaniesService>();
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
