using Betalgo.Ranul.OpenAI.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Class.Implemetations;
using PetSociety.API.Repositories.Class.Interfaces;
using PetSociety.API.Repositories.Events;
using PetSociety.API.Repositories.Events.Interfaces;
using PetSociety.API.Repositories.User.Implemetations;
using PetSociety.API.Repositories.User.Interfaces;
using PetSociety.API.Services.BackgroundTasks;
using PetSociety.API.Services.Class.Implementations;
using PetSociety.API.Services.Class.Interfaces;
using PetSociety.API.Services.Events;
using PetSociety.API.Services.Events.Interfaces;
using PetSociety.API.Services.Shared.Implementations;
using PetSociety.API.Services.Shared.Interfaces;
using PetSociety.API.Services.Shop;
using PetSociety.API.Services.User;
using PetSociety.API.Services.User.Implementations;
using PetSociety.API.Services.User.Interfaces;
using QuestPDF.Infrastructure;
using System.Text;
using static PetSociety.API.Repositories.Events.ActivityCommentRepository;


var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Add services to the container.
//註冊資料內容類別 注入給(controller, view)
builder.Services.AddDbContext<PetSocietyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PetSocietyContext")));

// 設定 QuestPDF License
QuestPDF.Settings.License = LicenseType.Community;

// 註冊 HttpContextAccessor，讓 Service 層也能拿到網址
builder.Services.AddHttpContextAccessor();
// 註冊 OutputCache 服務
builder.Services.AddOutputCache();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // 這一行是關鍵！告訴 Swagger 如果遇到一樣名字的類別，要用「全名 (FullName)」來區分
    options.CustomSchemaIds(type => type.FullName);
});

// 註冊 JWT 服務
builder.Services.AddScoped<IJwtService, JwtService>();

// 註冊OpenAI服務
builder.Services.AddOpenAIService();
// 註冊Activity服務
builder.Services.AddScoped<IActivityCalenderService, ActivityCalenderService>();
builder.Services.AddScoped<IActivityGuideService, ActivityGuideService>();
builder.Services.AddScoped<IActivityGuideRepository, ActivityGuideRepository>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IActivityCommentService, ActivityCommentService>();
builder.Services.AddScoped<IActivityCommentRepository, CommentRepository>();
builder.Services.AddScoped<IContentModerator, ContentModerator>();

// 註冊 Course 服務
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ICourseSubscribeService, CourseSubscribeService>();
builder.Services.AddScoped<ICourseSubscribeRepository, CourseSubscribeRepository>();
builder.Services.AddScoped<ICourseRecordRepository, CourseRecordRepository>();
builder.Services.AddScoped<IQuizRecordRepository, QuizRecordRepository>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
builder.Services.AddScoped<IEcpayPaymentService, EcpayPaymentService>();
builder.Services.AddScoped<ICourseOrderService, CourseOrderService>();
builder.Services.AddScoped<IChapterQuizRepository, ChapterQuizRepository>();
// 註冊 Course 單例服務
builder.Services.AddSingleton<ICourseCacheService, CourseCacheService>();
// 註冊背景 Worker
builder.Services.AddHostedService<CourseStatsWorker>(); // .NET 會自動呼叫 StartAsync，並主動執行在背景執行內部的 ExecuteAsync 方法
// 註冊綠界資訊
builder.Services.Configure<EcpaySettings>(builder.Configuration.GetSection("EcpaySettings"));


// 註冊 Member 服務
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IFavoritesRepository, FavoritesRepository>();
builder.Services.AddScoped<IFavoritesService, FavoritesService>();



// 註冊 Order 服務
builder.Services.AddScoped<IOrderService, OrderService>();

// JWT 驗證設定
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret must be configured.");


// 啟用認證服務
builder.Services.AddAuthentication(options =>
{
    // 定義驗證要使用 JWT Bearer 方案
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // 定義對未經授權的請求發出 Challenge(質詢)時，要使用 JWT Bearer 方案
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => // 定義驗證JWT需要的規則
{
    options.RequireHttpsMetadata = false;   // 開發和測試環境下，允許不使用 HTTPS連線也接受Token
    options.SaveToken = true;               // 將原始的 JWT 儲存在當前的 HttpContext 中
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,                         // 驗證Token發行者
        ValidateAudience = true,                       // 驗證Token接收者
        ValidateLifetime = true,                       // 驗證Token有效期限
        ValidateIssuerSigningKey = true,               // 驗證Token簽名密鑰

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ClockSkew = TimeSpan.Zero

    };
});





// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

}

// UseCors
app.UseCors("AllowAngular");

app.UseHttpsRedirection();
app.UseStaticFiles(); // 讓前端可以讀取 wwwroot 裡的檔案

app.UseOutputCache(); // 啟用 OutputCache 中介軟體

app.UseAuthentication(); // 有 token 建立 User，沒有 token User = Anonymous
app.UseAuthorization(); // 判斷權限 [Authorize] / [AllowAnonymous]

app.MapControllers();

app.Run();
