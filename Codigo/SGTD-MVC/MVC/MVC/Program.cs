using Microsoft.AspNetCore.Authentication.Cookies;

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

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseSession();
    app.UseAuthentication();
    app.UseAuthorization();
   

// Mapear rutas MVC
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=Login}/{id?}");

    app.Run();


//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();


//builder.Services.AddHttpClient("ProveedoresApi", client =>
//{
//    client.BaseAddress = new Uri("http://localhost:5079/api/"); // URL de tu API
//    client.DefaultRequestHeaders.Add("Accept", "application/json");
//});

//builder.Services.AddHttpClient("ProductosApi", client =>
//{
//    client.BaseAddress = new Uri("http://localhost:5079/api/"); // URL de tu API
//    client.DefaultRequestHeaders.Add("Accept", "application/json");
//});

//builder.Services.AddHttpClient("ReportesApi", client =>
//{
//    client.BaseAddress = new Uri("http://localhost:5079/api/"); // URL de tu API
//    client.DefaultRequestHeaders.Add("Accept", "application/json");
//});

//builder.Services.AddHttpClient("UsuariosApi", client =>
//{
//    client.BaseAddress = new Uri("http://localhost:5079/api/"); // URL de tu API
//    client.DefaultRequestHeaders.Add("Accept", "application/json");
//});

//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(30);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

//var app = builder.Build();

//app.UseStaticFiles();
//app.UseRouting();

//app.UseSession(); // <-- Esto es clave


//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();
