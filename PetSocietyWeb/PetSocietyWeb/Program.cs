using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetSocietyWeb;
using PetSocietyWeb.Areas.Class.Repositories;
using PetSocietyWeb.Areas.Class.Services;
using PetSocietyWeb.Data;
using PetSocietyWeb.Models.Generated;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 註冊資料內容類別
builder.Services.AddDbContext<PetSocietyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PetSocietyContext")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// [新增] 明確加入 Cookie 驗證服務
// 這是為了讓我們的 SSO Login 可以手動發出 Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // 如果有人沒登入想偷闖後台，踢去這個頁面
        options.LoginPath = "/Home/NoPermission";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Cookie 60分鐘有效
    });

builder.Services.AddControllersWithViews();

// 註冊 MVC 服務
builder.Services.AddCustomMvcOptions();

// Course 註冊 Repository & Service
builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<CourseService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseRouting();

// [注意] 順序非常重要！必須先驗證 (Authentication) 再授權 (Authorization)
app.UseAuthentication(); // 少了這行，Cookie 登入會無效
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();