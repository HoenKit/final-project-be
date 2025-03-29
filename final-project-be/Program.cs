using final_project_be.DAO;
using final_project_be.Data;
using final_project_be.Interface;
using final_project_be.Repository;
using final_project_be.Service.Mapping;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

// Config Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection")));

// Config Logger
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
// Config Mapper
builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();
// Add services to the container.

builder.Services.AddControllers();
    

// Config DAO
builder.Services.AddScoped<CommentDAO>();
builder.Services.AddScoped<PostDAO>();
builder.Services.AddScoped<NotificationDAO>();
builder.Services.AddScoped<PollOptionDAO>();
builder.Services.AddScoped<ReportCommentDAO>();
builder.Services.AddScoped<ReportPostDAO>();
builder.Services.AddScoped<ReportDAO>();
builder.Services.AddScoped<UserManagerDAO>();
builder.Services.AddScoped<PostDAO>();
builder.Services.AddScoped<CategoryDAO>();
builder.Services.AddScoped<SubCategoryDAO>();
builder.Services.AddScoped<PollOptionVoteDAO>();
// Config Repository
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IPollOptionRepository, PollOptionRepository>();
builder.Services.AddScoped<IReportCommentRepository, ReportCommentRepository>();
builder.Services.AddScoped<IReportPostRepository, ReportPostRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IUserManagerRepository, UserManagerRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
builder.Services.AddScoped<IPollOptionVoteRepository, PollOptionVoteRepository>();
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
