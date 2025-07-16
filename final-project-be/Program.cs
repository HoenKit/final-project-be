using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.VariantTypes;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Service.CloudinaryService;
using final_project_be_Application.Service.EmailService;
using final_project_be_Application.Service.Mapping;
using final_project_be_Application.Service.OpenAIService;
using final_project_be_Application.Ultils;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Configuration;
using Serilog;
using Serilog.Formatting.Json;
using System.Text;

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
builder.Services.AddScoped<ICommentDAO, CommentDAO>();
builder.Services.AddScoped<IPostDAO, PostDAO>();
builder.Services.AddScoped<INotificationDAO, NotificationDAO>();
builder.Services.AddScoped<IPollOptionDAO, PollOptionDAO>();
builder.Services.AddScoped<IReportCommentDAO, ReportCommentDAO>();
builder.Services.AddScoped<IReportPostDAO, ReportPostDAO>();
builder.Services.AddScoped<IReportDAO, ReportDAO>();
builder.Services.AddScoped<IUserDAO, UserDAO>();
builder.Services.AddScoped<ICategoryDAO, CategoryDAO>();
builder.Services.AddScoped<IPollOptionVoteDAO, PollOptionVoteDAO>();
builder.Services.AddScoped<IReportUserDAO, ReportUserDAO>();
builder.Services.AddScoped<IPostFileDAO, PostFileDAO>();
builder.Services.AddScoped<IAnswerDAO, AnswerDAO>();
builder.Services.AddScoped<IAssignmentDAO, AssignmentDAO>();
builder.Services.AddScoped<ICouponDAO, CouponDAO>();
builder.Services.AddScoped<ICourseCouponDAO, CourseCouponDAO>();
builder.Services.AddScoped<ICourseDAO, CourseDAO>();
builder.Services.AddScoped<IEventDAO, EventDAO>();
builder.Services.AddScoped<ILessonDAO, LessonDAO>();
builder.Services.AddScoped<IMembershipPlanDAO, MembershipPlanDAO>();
builder.Services.AddScoped<IMentorDAO, MentorDAO>();
builder.Services.AddScoped<IMentorCertificateDAO, MentorCertificateDAO>();
builder.Services.AddScoped<IModuleDAO, ModuleDAO>();
builder.Services.AddScoped<IPaymentDAO, PaymentDAO>();
builder.Services.AddScoped<IPaymentPlanDAO, PaymentPlanDAO>();
builder.Services.AddScoped<IPaymentCourseDAO, PaymentCourseDAO>();
builder.Services.AddScoped<IQuestionDAO, QuestionDAO>();
builder.Services.AddScoped<IReviewDAO, ReviewDAO>();
builder.Services.AddScoped<IScheduleDAO, ScheduleDAO>();
builder.Services.AddScoped<ITransactionDAO, TransactionDAO>();
builder.Services.AddScoped<IUserAnswerDAO, UserAnswerDAO>();
builder.Services.AddScoped<IUserAssignmentDAO, UserAssignmentDAO>();
builder.Services.AddScoped<IUserCourseDAO, UserCourseDAO>();
builder.Services.AddScoped<IUserLessonDAO, UserLessonDAO>();
builder.Services.AddScoped<IUserModuleDAO, UserModuleDAO>();
builder.Services.AddScoped<IUserScheduleDAO, UserScheduleDAO>();
builder.Services.AddScoped<IUserWorshopDAO, UserWorshopDAO>();
builder.Services.AddScoped<IWorkshopDAO, WorkshopDAO>();
builder.Services.AddScoped<ICourseEmbeddingDAO, CourseEmbeddingDAO>();
builder.Services.AddScoped<IUserEmbeddingDAO, UserEmbeddingDAO>();
builder.Services.AddScoped<IWithdrawDAO, WithdrawDAO>();

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
builder.Services.AddScoped<ILearningRepository, LearningRepository>();
builder.Services.AddScoped<IPaymentRepositoty, PaymentRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IWithdrawRepository, WithdrawRepository>();
//Config Service
builder.Services.AddScoped<IBlobStorageService,BlobStorageService>();
builder.Services.AddScoped<IOpenAIEmbeddingService, OpenAIEmbeddingService>();
builder.Services.AddHttpClient<IOpenAIEmbeddingService, OpenAIEmbeddingService>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<ICloudinaryService,CloudinaryService>();
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

builder.Services.Configure<GoogleSettings>(builder.Configuration.GetSection("Authentication:Google"));

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
