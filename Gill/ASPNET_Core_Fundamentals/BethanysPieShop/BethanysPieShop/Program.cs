using BethanysPieShop.Data;
using BethanysPieShop.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BethanysPieShopDbContext>(o => o.UseSqlite(
	builder.Configuration["ConnectionStrings:PieShopConnectionString"]));

builder.Services.AddScoped<ICategoryRepository, MockCategoryRepository>();
builder.Services.AddScoped<IPieRepository, PieRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.MapDefaultControllerRoute(); // => "{controller=Home}/{action=Index}/{id?}"

// app.MapControllerRoute(
// 	name: "default",
// 	pattern: "{controller=Home}/{action=Index}/{id?}");


DbInitializer.Seed(app);

app.Run();
