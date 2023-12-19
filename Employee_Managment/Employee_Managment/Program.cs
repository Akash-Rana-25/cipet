using Employee_Managment.bg_services;
using Employee_Managment.Context;
using Employee_Managment.DTO;
using Employee_Managment.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);

//var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//// Check if the roles exist, and create them if not.
//if (!await roleManager.RoleExistsAsync("Admin"))
//{
//    await roleManager.CreateAsync(new IdentityRole("Admin"));
//}

//if (!await roleManager.RoleExistsAsync("User"))
//{
//    await roleManager.CreateAsync(new IdentityRole("User"));
//}

//var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
//var user = await userManager.FindByNameAsync("admin@example.com");

//if (user != null)
//{
//    await userManager.AddToRoleAsync(user, "Admin");
//} 

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IPunchEventRepository, PunchEventRepository>();
builder.Services.AddSingleton<IHostedService, MonthlyResetService> ();
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders()
//      .AddRoles<IdentityRole>();

//builder.Services.Configure<IdentityOptions>(options =>
//{
//    // Configure password requirements, lockout settings, etc.
//});

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    // Configure cookie settings
//});


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
     pattern: "{controller=Employee}/{action=Index}/{id?}");
app.Run();
