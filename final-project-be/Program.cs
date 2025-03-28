using final_project_be.Service.Mapping;
using Serilog;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    
    .WriteTo.File(
        new JsonFormatter(), 
        path: "logs/console/console-.log",
        rollingInterval: RollingInterval.Day, 
        retainedFileCountLimit: 7) 
                                   
    .WriteTo.File(
        new JsonFormatter(),
        path: "logs/error/error-.log",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, 
        retainedFileCountLimit: 7)
    .CreateLogger();
builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
