using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LoCoMPro.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using LoCoMPro.Areas.Identity.Pages.Account.Manage;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// builder.Services.AddMvc().AddRazorPagesOptions(options => options.AllowAreas = true);
builder.Services.AddDbContext<LoCoMProContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LoCoMProContext") ?? throw new InvalidOperationException("Connection string 'LoCoMProContext' not found.")));

// Added default IdentityUser and configured it to not require a confirmed account, also added custom signInManager that overrides PasswordSignInAsync()
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false).AddSignInManager<MySignInManager>().AddEntityFrameworkStores<LoCoMProContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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
