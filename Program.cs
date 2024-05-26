using FFMonitorWeb.Data;
using FFMonitorWeb.Models;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDbSettings = FFMonitorWeb.Data.MongoDbSettings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Services for the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    });

// Configure MongoDB settings
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection(nameof(MongoDbSettings)));

// Register MongoDbContext
builder.Services.AddSingleton<MongoDbContext>();

// Configure Identity services to use MongoDB with ObjectId
builder.Services.AddIdentity<User, MongoIdentityRole<ObjectId>>()
    .AddMongoDbStores<User, MongoIdentityRole<ObjectId>, ObjectId>(
        builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>().ConnectionString,
        builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>().DatabaseName)
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // User settings.
    options.User.RequireUniqueEmail = true;
    // Sign-in settings.
    options.SignIn.RequireConfirmedEmail = true; // Require email confirmation
});

// Configure SendGrid email service
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

// Add Authentication and Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "FFMonitorWeb.AuthCookie"; // Replace with your application's name
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configuring the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Logging configuration
app.Logger.LogInformation("Application Starting...");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map Controller Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

