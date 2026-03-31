using FluentValidation;
using FreeBilling.Web.Apis;
using FreeBilling.Web.Data;
using FreeBilling.Web.Data.Entities;
using FreeBilling.Web.Services;
using FreeBilling.Web.Validators;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("FreeBillingDBConnectionString") ?? throw new InvalidOperationException("Connection string 'BillingContextConnection' not found.");

IConfigurationBuilder configBuilder = builder.Configuration;
configBuilder.Sources.Clear();
configBuilder.AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", true)
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .AddEnvironmentVariables()
    .AddCommandLine(args);

builder.Services.AddDbContext<BillingContext>(o => o.UseSqlite(
    builder.Configuration["ConnectionStrings:FreeBillingDBConnectionString"]));

builder.Services.AddDefaultIdentity<TimeBillUser>
    (options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 8;
    }).AddEntityFrameworkStores<BillingContext>();


builder.Services.AddAuthentication()
  .AddJwtBearer(cfg =>
  {
      cfg.TokenValidationParameters = new TokenValidationParameters()
      {
          ValidIssuer = builder.Configuration["Token:Issuer"],
          ValidAudience = builder.Configuration["Token:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(builder.Configuration["Token:Key"]!))
      };
  });

builder.Services.AddAuthorization(cfg =>
{
    cfg.AddPolicy("ApiPolicy", bldr =>
    {
        bldr.RequireAuthenticatedUser();
        bldr.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
    });
});
builder.Services.AddScoped<IBillingRepository, BillingRepository>();

builder.Services.AddRazorPages();
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<TimeBillModelValidator>();

TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetEntryAssembly()!);
var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

//app.Run(async ctx =>
//{
//    await ctx.Response.WriteAsync("Welcome to FreeBilling");
//});

TimeBillsApi.Register(app);
AuthApi.Register(app);
EmployeesApi.Register(app);

app.MapControllers();

app.MapFallbackToPage("/customerBilling");

app.Run();
