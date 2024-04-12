//https://learn.microsoft.com/vi-vn/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-6.0&tabs=visual-studio
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.MapControllerRoute(
    name: "News",
    pattern: "/news",
    defaults: new { controller = "News", action = "Index" });
app.MapControllerRoute(
    name: "AboutUs",
    pattern: "/aboutus",
    defaults: new { controller = "AboutUs", action = "Index" });
app.MapControllerRoute(
    name: "pricing",
    pattern: "/pricing",
    defaults: new { controller = "Pricing", action = "Index" });
app.MapControllerRoute(
    name: "home",
    pattern: "/home",
    defaults: new { controller = "Home", action = "Index" });
app.MapControllerRoute(
    name: "Contact",
    pattern: "/contact",
    defaults: new { controller = "Contact", action = "Index" });
app.MapControllerRoute(
    name: "faq",
    pattern: "/faq",
    defaults: new { controller = "FAQ", action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();