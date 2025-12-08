var builder = WebApplication.CreateBuilder(args);
string? connectionString = builder.Configuration.GetConnectionString("GestionServiceDB");
// --- 1. CAPAS DE REPOSITORIOS (Acceso a Datos) ---
builder.Services.AddScoped<GESTIONSUBSCRIPCIONES.Repositories.IPlanSuscripcionRepository, GESTIONSUBSCRIPCIONES.Repositories.PlanSuscripcionRepository>();
builder.Services.AddScoped<GESTIONSUBSCRIPCIONES.Repositories.IUsuarioRepository, GESTIONSUBSCRIPCIONES.Repository.UsuarioRepository>();
builder.Services.AddScoped<GESTIONSUBSCRIPCIONES.Repositories.IFacturaRepository, GESTIONSUBSCRIPCIONES.Repository.FacturaRepository>();
builder.Services.AddScoped<GESTIONSUBSCRIPCIONES.Repositories.IMetodoPagoRepository, GESTIONSUBSCRIPCIONES.Repository.MetodoPagoRepository>();
builder.Services.AddScoped<GESTIONSUBSCRIPCIONES.Repositories.INotificacionRepository, GESTIONSUBSCRIPCIONES.Repository.NotificacionRepository>();

// --- 2. CAPAS DE SERVICIOS (Lógica de Negocio) ---
builder.Services.AddScoped<GESTIONSUBSCRIPCIONES.Services.IPlanSuscripcionService, GESTIONSUBSCRIPCIONES.Services.PlanSuscripcionService>();
builder.Services.AddScoped<GESTIONSUBSCRIPCIONES.Services.IUsuarioservices, GESTIONSUBSCRIPCIONES.Services.UsuarioService>();
builder.Services.AddScoped<GESTIONSUBSCRIPCIONES.Services.IFacturaService, GESTIONSUBSCRIPCIONES.Services.FacturaService>();
builder.Services.AddScoped<GESTIONSUBSCRIPCIONES.Services.IMetodoPagoService, GESTIONSUBSCRIPCIONES.Services.MetodoPagoService>();
builder.Services.AddScoped<GESTIONSUBSCRIPCIONES.Services.INotificacionService, GESTIONSUBSCRIPCIONES.Services.NotificacionService>();

// --- 3. CONFIGURACIÓN DE SWAGGER/OPENAPI ---
builder.Services.AddEndpointsApiExplorer();

// Usamos SwaggerGen para personalizar el título y versión.
builder.Services.AddSwaggerGen(c =>
{
    // Leemos la variable de entorno inyectada por Docker Compose.
    var apiVersion = builder.Configuration["API_VERSION"] ?? "Desarrollo Local"; 
    
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = $"GESTIÓN SUBSCRIPCIONES API (v1)", 
        Version = $"v1 ({apiVersion})", // Incluimos la variable de entorno aquí
        Description = "API RESTful para la gestión de planes, usuarios, facturas y métodos de pago.",
    });
});
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    
    // Configuración para Docker (Asegura que Swagger UI use el puerto 8305)
    app.UseSwaggerUI(c =>
    {
        // Forzamos el endpoint de la API al puerto donde Docker la expone (http://localhost:8305)
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GESTIÓN SUBSCRIPCIONES API v1");
    });
}

app.UseHttpsRedirection();

// 💡 SECCIÓN WEATHERFORECAST ELIMINADA
app.MapControllers();

app.Run();

// 💡 DEFINICIÓN RECORD WEATHERFORECAST ELIMINADA