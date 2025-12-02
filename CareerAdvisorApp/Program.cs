using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CareerAdvisorApp.Models.Interfaces;
using CareerAdvisorApp.Models.Services;
using CareerAdvisorApp.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlite(connectionString));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<ICareerService, CareerService>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddAuthorization(options =>
{
    // Admin only: must have Admin role and FullAccess claim
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin")
              .RequireClaim("Permission", "FullAccess"));

    // User only: must have User role and BasicAccess claim
    options.AddPolicy("UserOnly", policy =>
        policy.RequireRole("User")
              .RequireClaim("Permission", "BasicAccess"));

    // Either Admin or User: must have either claim
    options.AddPolicy("AdminOrUser", policy =>
        policy.RequireClaim("Permission", "FullAccess", "BasicAccess"));

});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; 
});


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // log all messages
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) 
    .WriteTo.File("logs/error.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, rollingInterval: RollingInterval.Day) 
    .CreateLogger();
builder.Logging.AddConsole();
builder.Host.UseSerilog();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await IdentitySeed.SeedRolesAndAdmin(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
