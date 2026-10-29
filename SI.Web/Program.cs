using Microsoft.EntityFrameworkCore;
using SI.Web.Data;
using SI.Web.Data.Entities;
using SI.Web.Interfaces;
using SI.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");

builder.Services.AddDbContext<InventarioDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Inicializar la base de datos y datos semilla en desarrollo (Escenario E-01 para pruebas)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<InventarioDbContext>();
    try
    {
        context.Database.EnsureCreated();

        if (!context.Productos.Any())
        {
            var prod1 = new Producto
            {
                Codigo = "PROD-E01",
                Nombre = "Tornillo de Acero 1/4 (Escenario E-01)",
                Descripcion = "Producto de prueba para validación de PEPS y Promedio",
                UnidadMedida = "pieza",
                Activo = true
            };
            var prod2 = new Producto
            {
                Codigo = "PROD-002",
                Nombre = "Clavo Concreto 2 pulg",
                Descripcion = "Material de fijación",
                UnidadMedida = "caja",
                Activo = true
            };

            context.Productos.AddRange(prod1, prod2);
            context.SaveChanges();

            // Aplicar Escenario E-01: E1 (10@20), E2 (10@30), S1 (5)
            var invService = scope.ServiceProvider.GetRequiredService<IInventarioService>();
            invService.RegistrarEntradaAsync(prod1.Id, DateTime.UtcNow.AddDays(-2), 10m, 20m).GetAwaiter().GetResult();
            invService.RegistrarEntradaAsync(prod1.Id, DateTime.UtcNow.AddDays(-1), 10m, 30m).GetAwaiter().GetResult();
            invService.RegistrarSalidaAsync(prod1.Id, DateTime.UtcNow, 5m).GetAwaiter().GetResult();
        }
    }
    catch
    {
        // Si la BD no está disponible al iniciar, se omite el seeding
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
