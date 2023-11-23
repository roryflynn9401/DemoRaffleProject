using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using SportsRaffles.Data;
using SportsRaffles.Data.Models;
using SportsRaffles.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 102400000;
        options.EnableDetailedErrors = true;
    });

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredLocalStorage();

var connectionString = builder.Configuration.GetConnectionString("Dev");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));

builder.Services.AddIdentity<User, UserRole>()
    .AddRoles<UserRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        googleOptions.AccessDeniedPath = "/AccessDenied";

    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User", "Admin"));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/Logout";
    options.AccessDeniedPath = "/Error";
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<CompetitionService>();
StripeConfiguration.ApiKey = "sk_test_51NskjEE5C5eLEIJwVDF4OzGMWROs146Prut2AlqV12T7FRDBoQNW1tonEdA7uhbKrmDxAdFUARV53hfCsfiKcwAR00LOLXVGKr";

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseHttpsRedirection();
    app.UseMigrationsEndPoint();   
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapControllers();
app.MapFallbackToPage("/_Host");
app.Run();
