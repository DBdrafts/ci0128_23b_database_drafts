using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LoCoMPro.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using LoCoMPro.Areas.Identity.Pages.Account.Manage;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using LoCoMPro.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// builder.Services.AddMvc().AddRazorPagesOptions(options => options.AllowAreas = true);
builder.Services.AddDbContext<LoCoMProContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LoCoMProContext") ?? throw new InvalidOperationException("Connection string 'LoCoMProContext' not found.")));

// Added default IdentityUser and configured it to not require a confirmed account, also added custom signInManager that overrides PasswordSignInAsync()
builder.Services.AddDefaultIdentity<User>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 !*-._@+";
}).AddSignInManager<MySignInManager>().AddEntityFrameworkStores<LoCoMProContext>();

builder.Services.AddTransient<IEmailSender, EmailSender>(i => 
    new EmailSender(
        builder.Configuration["EmailSender:Host"],
        builder.Configuration.GetValue<int>("EmailSender:Port"),
        builder.Configuration.GetValue<bool>("EmailSender:EnableSSL"),
        builder.Configuration["EmailSender:UserName"],
        builder.Configuration["EmailSender:Password"]
    )
);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

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
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
