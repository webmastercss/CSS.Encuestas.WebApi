using CSS.Encuestas.Application.Interfaces.Repositories;
using CSS.Encuestas.Application.Interfaces.Services;
using CSS.Encuestas.Application.Services;
using CSS.Encuestas.Infrastructure.Options;
using CSS.Encuestas.Infrastructure.Persistence;
using CSS.Encuestas.Infrastructure.Repositories;
using CSS.Encuestas.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("local");
// EF Core
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(connectionString));

//Perzonalizar respuestas
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        // Extraer todos los errores
        var errores = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(kvp => kvp.Value!.Errors.Select(e => new
            {
                Campo = kvp.Key,
                Mensaje = e.ErrorMessage
            }))
            .ToList();

        // Crear respuesta personalizada
        var respuesta = new
        {
            mensaje = "Hay errores en los datos enviados.",
            detalles = errores.Select(e => $"El campo '{e.Campo}': {e.Mensaje}")
        };

        return new BadRequestObjectResult(respuesta);
    };
});


//builder.Services.AddScoped<IEncuestaRepository, EncuestaAdoRepository>();
builder.Services.AddScoped<IEncuestaRepository, EncuestaEfCoreRepository>();
builder.Services.AddScoped<IEncuestaService, EncuestaService>();

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, MailKitEmailService>();


// Habilita CORS para todos los orígenes
builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliticaLibre", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



// Usa la política de CORS



var app = builder.Build();


app.MapGet("/", async (IEmailService emailSender) =>
{
    await emailSender.SendAsync(
        "joseriospacheco@gmail.com",
        "Prueba de correo",
        "<h1>¡Hola desde MailKit!</h1><p>Este es un mensaje de prueba.</p>"
    );
    return "Correo enviado ";
});


app.UseCors("PoliticaLibre");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
