using System.Reflection;
using Microsoft.OpenApi;
using ProductosApi.Data;
using ProductosApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Conexión a la base de datos y repositorio (inyección de dependencias)
builder.Services.AddSingleton<ConexionDb>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// Swagger para documentar y probar la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Productos API",
        Version = "v1",
        Description = "CRUD de productos con ASP.NET Core, ADO.NET y SQL Server."
    });

    var archivoXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opciones.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, archivoXml));
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
