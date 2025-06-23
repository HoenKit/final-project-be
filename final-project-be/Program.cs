using Microsoft.AspNetCore.Authentication.JwtBearer;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.Data;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Json;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using final_project_be_Application.Ultils;
using Microsoft.Extensions.Azure;
using final_project_be_Application.Service.Mapping;
using final_project_be_Application.Service.EmailService;
using NuGet.Configuration;
using final_project_be_Application.Service.CloudinaryService;
using final_project_be_Application.Service.AimlService;

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
// AddAsync services to the container.

builder.Services.AddControllers();


// Config URL
builder.Services.Configure<ClientSettings>(builder.Configuration.GetSection("ClientSettings"));
// Config SignalR

builder.Services.AddSignalR();

// Config DAO
builder.Services.AddScoped<CommentDAO>();
builder.Services.AddScoped<PostDAO>();
builder.Services.AddScoped<NotificationDAO>();
builder.Services.AddScoped<PollOptionDAO>();
builder.Services.AddScoped<ReportCommentDAO>();
builder.Services.AddScoped<ReportPostDAO>();
builder.Services.AddScoped<ReportDAO>();
builder.Services.AddScoped<UserDAO>();
builder.Services.AddScoped<PostDAO>();
builder.Services.AddScoped<CategoryDAO>();
builder.Services.AddScoped<PollOptionVoteDAO>();
builder.Services.AddScoped<ReportUserDAO>();
builder.Services.AddScoped<PostFileDAO>();
builder.Services.AddScoped<AnswerDAO>();
builder.Services.AddScoped<AssignmentDAO>();
builder.Services.AddScoped<CouponDAO>();
builder.Services.AddScoped<CourseCouponDAO>();
builder.Services.AddScoped<CourseDAO>();
builder.Services.AddScoped<EventDAO>();
builder.Services.AddScoped<LessonDAO>();
builder.Services.AddScoped<MembershipPlanDAO>();
builder.Services.AddScoped<MentorDAO>();
builder.Services.AddScoped<MentorCertificateDAO>();
builder.Services.AddScoped<ModuleDAO>();
builder.Services.AddScoped<PaymentDAO>();
builder.Services.AddScoped<PaymentPlanDAO>();
builder.Services.AddScoped<PaymentCourseDAO>();
builder.Services.AddScoped<QuestionDAO>();
builder.Services.AddScoped<ReviewDAO>();
builder.Services.AddScoped<ScheduleDAO>();
builder.Services.AddScoped<TransactionDAO>();
builder.Services.AddScoped<UserAnswerDAO>();
builder.Services.AddScoped<UserAssignmentDAO>();
builder.Services.AddScoped<UserCourseDAO>();
builder.Services.AddScoped<UserLessonDAO>();
builder.Services.AddScoped<UserModuleDAO>();
builder.Services.AddScoped<UserScheduleDAO>();
builder.Services.AddScoped<UserWorshopDAO>();
builder.Services.AddScoped<WorkshopDAO>();
// Config Repository
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IPollOptionRepository, PollOptionRepository>();
builder.Services.AddScoped<IReportCommentRepository, ReportCommentRepository>();
builder.Services.AddScoped<IReportPostRepository, ReportPostRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPollOptionVoteRepository, PollOptionVoteRepository>();
builder.Services.AddScoped<IUserAuthRepository, UserAuthRepository>();
builder.Services.AddScoped<IReportUserRepository, ReportUserRepository>();
builder.Services.AddScoped<IPostFileRepository, PostFileRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IMentorCertificateRepository, MentorCertificateRepository>();
builder.Services.AddScoped<IMentorRepository, MentorRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
//Config Service
builder.Services.AddScoped<BlobStorageService>();
builder.Services.AddHttpClient<AimlService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<CloudinaryService>();
builder.Services.Configure<CloudinarySettings>(
	builder.Configuration.GetSection("Cloudinary"));

//config class
builder.Services.AddScoped<Validate>();

builder.Services.AddScoped<Caculator>();

//Config Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Cookies["AccessToken"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
//Config Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
});

//Config Email:
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

//Config Cookie
builder.Services.AddHttpContextAccessor();

//Config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7191",
            "https://phronesis-c2dzfzakfwe9eyf0.southeastasia-01.azurewebsites.net",
            "https://phronesis-fe-esd4fvddb4d8cnc4.eastasia-01.azurewebsites.net"
        )
              .AllowAnyHeader()
              .AllowAnyMethod().
              AllowCredentials();
    });

});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAzureClients(clientBuilder =>
{
	clientBuilder.AddBlobServiceClient(builder.Configuration["ConnectionStrings:MyConnection:blob"]!, preferMsi: true);
	clientBuilder.AddQueueServiceClient(builder.Configuration["ConnectionStrings:MyConnection:queue"]!, preferMsi: true);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
		c.RoutePrefix = "swagger";
	});
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    Secure = CookieSecurePolicy.Always
});

app.UseCors("AllowAll");


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();
app.MapHub<SignalRHub>("/postHub");
app.MapControllers();

app.Run();
