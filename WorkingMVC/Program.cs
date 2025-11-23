using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WorkingMVC.Constants;
using WorkingMVC.Data;
using WorkingMVC.Data.Entities;
using WorkingMVC.Data.Entities.Idenity;
using WorkingMVC.Interfaces;
using WorkingMVC.Repositories;
using WorkingMVC.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<MyAppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
    .AddEntityFrameworkStores<MyAppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

var dirImageName = builder.Configuration.GetValue<string>("DirImageName") ?? "test";

var path = Path.Combine(Directory.GetCurrentDirectory(), dirImageName);
Directory.CreateDirectory(path);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(path),
    RequestPath = $"/{dirImageName}"
});

app.MapAreaControllerRoute(
    name: "MyAreaPigAdmin",
    areaName: "Admin",
    pattern: "admin/{controller=Dashboards}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}")
    .WithStaticAssets();



using (var scoped = app.Services.CreateScope())
{
    var serviceProvider = scoped.ServiceProvider;
    var myAppDbContext = serviceProvider.GetRequiredService<MyAppDbContext>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<RoleEntity>>();

    try
    {
        myAppDbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка застосування міграцій: {ex.Message}");
    }


    if (!myAppDbContext.Roles.Any())
    {
        foreach (var roleName in Roles.AllRoles)
        {
            var role = new RoleEntity(roleName);
            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                Console.WriteLine($"-----Створили роль {roleName}-----");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine("+++Проблема " + error.Description);
                }
                Console.WriteLine($"++++Проблеми створення ролі {roleName}++++");
            }
        }
    }

    if (!myAppDbContext.OrderStatuses.Any())
    {
        List<string> names = new List<string>() {
            "Нове", "Очікує оплати", "Оплачено",
            "В обробці", "Готується до відправки",
            "Відправлено", "У дорозі", "Доставлено",
            "Завершено", "Скасовано (вручну)", "Скасовано (автоматично)",
            "Повернення", "В обробці повернення" };

        var orderStatuses = names.Select(name => new OrderStatusEntity { Name = name }).ToList();

        await myAppDbContext.OrderStatuses.AddRangeAsync(orderStatuses);
        await myAppDbContext.SaveChangesAsync();
    }


}

app.Run();