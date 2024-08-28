using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Youtube_Shorts.Data;
using Youtube_Shorts.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddScoped<IYouTubeService, YouTubeUploadService>();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = "998799973915-ousa164pt8bgiaicotkbc3jgebrc28lo.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.SaveTokens = true;
    options.CallbackPath = new PathString("/signin-google");
    options.Scope.Add("https://www.googleapis.com/auth/youtube.upload");
    options.Scope.Add("https://www.googleapis.com/auth/youtube");
    options.Scope.Add("https://www.googleapis.com/auth/youtubepartner");
    options.Scope.Add("https://www.googleapis.com/auth/youtube.force-ssl");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.AccessType = "offline";
});

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

app.UseAuthentication(); // Ensure authentication middleware is added
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
