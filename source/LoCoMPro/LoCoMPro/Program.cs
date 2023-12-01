using Microsoft.EntityFrameworkCore;
using LoCoMPro.Data;
using Microsoft.AspNetCore.Identity;
using LoCoMPro.Areas.Identity.Pages.Account.Manage;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using LoCoMPro.Services;
using SendGrid;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// builder.Services.AddMvc().AddRazorPagesOptions(options => options.AllowAreas = true);
builder.Services.AddDbContext<LoCoMProContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("LoCoMProContext") ?? throw new InvalidOperationException("Connection string 'LoCoMProContext' not found."),
        x => x.UseNetTopologySuite());
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("es-ES"),
        new CultureInfo("en-US")
    };

    options.DefaultRequestCulture = new RequestCulture("es-ES");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Added default IdentityUser and configured it to not require a confirmed account, also added custom signInManager that overrides PasswordSignInAsync()
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 !*-._@+";
}).AddSignInManager<MySignInManager>().AddRoles<IdentityRole>().AddEntityFrameworkStores<LoCoMProContext>().AddDefaultTokenProviders();

builder.Services.AddTransient<IEmailSender, SendGridEmailSender>(i =>
{
   return new SendGridEmailSender(builder.Configuration);
}
    
);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<LoCoMProContext>();
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<User>>();

    if (!await roleManager.RoleExistsAsync("Moderator"))
    {
        await roleManager.CreateAsync(new IdentityRole("Moderator"));

        User moderador = await userManager.FindByIdAsync("498fc1e4-db15-4411-94e3-6511dff4a758");
        User moderador2 = await userManager.FindByIdAsync("277a7be2-a41d-4d22-bb4c-849a020e64d3");
        User automatic = await userManager.FindByIdAsync("7d5b4e6b-28eb-4a70-8ee6-e7378e024aa4");

        string role = "Moderator";

        await userManager.AddToRoleAsync(moderador, role);
        await userManager.AddToRoleAsync(moderador2, role);
        await userManager.AddToRoleAsync(automatic, role);
    }

    var locOptions = services.GetService<IOptions<RequestLocalizationOptions>>();
    app.UseRequestLocalization(locOptions.Value);

    var cultureInfo = new CultureInfo("es-ES"); // Reemplaza con la cultura que desees
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
