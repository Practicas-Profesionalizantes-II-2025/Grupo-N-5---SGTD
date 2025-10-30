using Microsoft.AspNetCore.Authentication.Cookies;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Index"; // página de login
        options.AccessDeniedPath = "/Auth/AccesoDenegado"; // si no tiene rol
    });

builder.Services.AddAuthorization();


// Add services to the container.
builder.Services.AddControllersWithViews();



// Configuración de HttpClient para tus APIs
builder.Services.AddHttpClient("ProveedoresApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5079/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient("ProductosApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5079/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient("ReportesApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5079/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient("UsuariosApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5079/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("RubrosApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5079/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("RolesApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5079/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("DisciplinasApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5079/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("ClientesApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5079/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpMetrics();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
   
app.MapMetrics();
// Mapear rutas MVC
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();

