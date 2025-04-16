using BlogProject.Data;
using BlogProject.Data.Concrete.EfCore;
using BlogProject.Data.Abstract;
using BlogProject.Services.Abstract;
using BlogProject.Services.Concrete;
using BlogProject.Entities;
using BlogProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Email ayarlarını yapılandır
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, MailKitEmailService>();

// Loglama için dosya oluştur
var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs.txt");
System.IO.File.WriteAllText(logFilePath, "Application Logs\n" + DateTime.Now.ToString() + "\n\n");

// DbContext ve Identity servislerini ekle
builder.Services.AddDbContext<BlogContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    
    // Development ortamında detaylı loglama etkinleştir
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging()
               .EnableDetailedErrors()
               .LogTo(message => {
                   Console.WriteLine(message);
                   System.IO.File.AppendAllText(logFilePath, message + "\n");
               });
    }
});

builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    
    // Claims güncelleme davranışı
    options.ClaimsIdentity.UserIdClaimType = System.Security.Claims.ClaimTypes.NameIdentifier;
    options.ClaimsIdentity.UserNameClaimType = System.Security.Claims.ClaimTypes.Name;
})
.AddEntityFrameworkStores<BlogContext>()
.AddDefaultTokenProviders()
.AddUserValidator<CustomUserValidator>();

// SignInManager özelleştirmesi
builder.Services.AddScoped<SignInManager<User>, CustomSignInManager>();

// Cookie ayarlarını yapılandır
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
});

// Repository servisleri
builder.Services.AddRepositories();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.InitializeAsync(services).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seed data oluşturulurken bir hata oluştu.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Admin area route tanımı
app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

// Önce spesifik controller action route'ları
app.MapControllerRoute(
    name: "post-actions",
    pattern: "Post/{action}/{id?}",
    defaults: new { controller = "Post" });

// Sonra SEO dostu URL yapılandırması
app.MapControllerRoute(
    name: "post-details",
    pattern: "post/{url}",
    defaults: new { controller = "Post", action = "ByUrl" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
