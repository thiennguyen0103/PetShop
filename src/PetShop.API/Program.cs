using PetShop.API.Extensions;
using PetShop.Application;
using PetShop.Infrastructure;
using PetShop.Infrastructure.Identity.Seeds;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerExtension();
builder.Services.AddApiVersioningExtension();
builder.Services.AddHealthChecks();
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

// Read Configuration from appSettings
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Initialize Logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

var app = builder.Build();
await app.InitializeDatabaseAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerExtension();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.UseErrorHandlingMiddleware();
app.UseHealthChecks("/health");

app.Run();