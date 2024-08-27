            using Microsoft.AspNetCore.Identity;
            using Microsoft.EntityFrameworkCore;
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

            builder.Services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "998799973915-ousa164pt8bgiaicotkbc3jgebrc28lo.apps.googleusercontent.com";
                options.ClientSecret = "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";
                options.Scope.Add("https://www.googleapis.com/auth/youtube.upload");
                // Add these lines to request profile and email scopes
                options.Scope.Add("profile");
                options.Scope.Add("email");
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
