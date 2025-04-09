using Auto.Data;
using Auto.Models;
using Auto.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<CustomUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireITRole", policy => policy.RequireRole("IT"));
    options.AddPolicy("RequireWarehouseRole", policy => policy.RequireRole("Warehouse"));
    options.AddPolicy("RequireProcurementRole", policy => policy.RequireRole("Procurement"));
    options.AddPolicy("RequireAdministrationRole", policy => policy.RequireRole("Administration"));
});

builder.Services.AddControllersWithViews();

// Register PdfService and IConverter
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddTransient<PdfService>();

// Указание пути к библиотеке libwkhtmltox
var wkHtmlToPdfPath = Path.Combine(builder.Environment.ContentRootPath, "runtimes", "win-x64", "native", "libwkhtmltox.dll");
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !File.Exists(wkHtmlToPdfPath))
{
    throw new FileNotFoundException("Не удалось найти библиотеку libwkhtmltox.dll", wkHtmlToPdfPath);
}

var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

// Логирование длины пути
logger.LogInformation($"Длина пути до библиотеки libwkhtmltox.dll: {wkHtmlToPdfPath.Length} символов");

if (wkHtmlToPdfPath.Length > 255)
{
    throw new PathTooLongException("Путь до библиотеки libwkhtmltox.dll превышает 255 символов");
}



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await InitData.InitializeAsync(serviceProvider);
    await RoleInitializer.InitializeAsync(serviceProvider);
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
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();


