using CSS.Encuestas.Application.Dtos;
using CSS.Encuestas.Application.Interfaces.Repositories;
using CSS.Encuestas.Application.Interfaces.Services;
using CSS.Encuestas.Application.Services;
using CSS.Encuestas.Infrastructure.Data;
using CSS.Encuestas.Infrastructure.Options;
using CSS.Encuestas.Infrastructure.Persistence;
using CSS.Encuestas.Infrastructure.Repositories;
using CSS.Encuestas.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<EncuestasDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("EncuestasDb")));


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






//Inyección de opciones 
builder.Services.Configure<ApiIpsOptions>(builder.Configuration.GetSection("Ips"));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

//Inyección de repositorios
//builder.Services.AddScoped<IEncuestaRepository, EncuestaAdoRepository>();
builder.Services.AddScoped<IEncuestaRepository, EncuestaEfCoreRepository>();

//Inyección de servicios
builder.Services.AddScoped<IEncuestaService, EncuestaService>();




builder.Services.AddHttpClient<IIpsService, IpsService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});


//builder.Services.AddScoped<IIpsService, IpsService>();
//builder.Services.AddScoped<IEmailService, IpsService>();
builder.Services.AddScoped<IEmailService, NetMailEmailService>();


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

var app = builder.Build();


app.MapPost("Emails/", async (IEmailService emailSender, EnviarCorreoDto dto) =>
{

    await emailSender.SendAsync(
        dto.Destinatarios,
        "",
        dto.Asunto,
       dto.Cuerpo
    );

    return Results.Ok(new { message = "Correo enviado" });
});


app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            message = "Ocurrió un error interno en el servidor."
        });
    });
});



// Usa la política de CORS
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
