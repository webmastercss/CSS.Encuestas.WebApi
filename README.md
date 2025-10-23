# 🧅 CSS.Encuestas

### API REST construida con Arquitectura de Cebolla (Onion Architecture), ADO.NET, EF Core, Inyección de Dependencias, Métodos de Extensión y Patrón Repository.

---

## 📖 Descripción del proyecto

**CSS.Encuestas** es una API REST desarrollada con **ASP.NET Core 8** que implementa la **Arquitectura de Cebolla (Onion Architecture)**.  
Su objetivo es ofrecer una estructura **modular, mantenible y desacoplada**, aplicando principios **SOLID**, **Inversión de Dependencias (DIP)** e **Inversión de Control (IoC)**, junto con **Inyección de Dependencias (DI)**.  

El sistema gestiona **encuestas preanalíticas**, utilizando dos enfoques de acceso a datos:  
- **ADO.NET**, para un control preciso y directo sobre las consultas SQL.  
- **Entity Framework Core**, para operaciones ORM y persistencia de entidades.  

Además, el proyecto utiliza **métodos de extensión** y **mapeadores (Mapper Pattern)** para mantener un código limpio, reutilizable y desacoplado.

---

## 🧩 Estructura de la solución

```
CSS.Encuestas.sln
│
├── CSS.Encuestas.Domain            → Entidades del dominio y contratos base
├── CSS.Encuestas.Application       → DTOs, casos de uso, servicios y mapeadores
├── CSS.Encuestas.Infrastructure    → Implementación de repositorios (ADO.NET / EF Core)
└── CSS.Encuestas.WebApi            → Capa de presentación (controladores y endpoints)
```

---

## ⚙️ Principales características

### 🧱 Arquitectura de Cebolla (Onion Architecture)
Organiza el código en capas concéntricas donde **las dependencias fluyen siempre hacia el dominio**.  
Esto garantiza independencia tecnológica, mejor mantenibilidad y facilidad para realizar cambios sin afectar otras capas.

---

### 💡 Inversión de Control (IoC)
Se implementa mediante el **contenedor de dependencias de ASP.NET Core**, que administra la creación, ciclo de vida y resolución de dependencias de todos los componentes.

```csharp
builder.Services.AddScoped<IEncuestaRepository, EncuestaAdoRepository>();
builder.Services.AddScoped<IEncuestaService, EncuestaService>();
```

---

### 🧩 Inyección de Dependencias (DI)
Las dependencias se reciben a través del constructor, evitando acoplamientos directos y favoreciendo la testabilidad y el cumplimiento del **principio D (Dependency Inversion)** de SOLID.

```csharp
public class EncuestaAdoRepository(IConfiguration config) : IEncuestaRepository
{
    private readonly string _connectionString = config.GetConnectionString("local")!;
}
```

---

### 🔄 Inversión de Dependencia (DIP)
El dominio depende de **abstracciones (interfaces)**, no de implementaciones concretas.  
Esto permite intercambiar libremente tecnologías de acceso a datos (EF Core ↔ ADO.NET) sin modificar la lógica del dominio.

```csharp
public interface IEncuestaRepository
{
    Task<int> AddAsync(EncuestaPreanalitica entity, CancellationToken ct = default);
}
```

---

### 🗂️ Patrón Repository
Encapsula la lógica de acceso a datos, separando la persistencia de la lógica de negocio.  
El proyecto implementa **ADO.NET** y **Entity Framework Core** bajo el mismo contrato (`IEncuestaRepository`).

**Repositorio ADO.NET**
```csharp
public class EncuestaAdoRepository : IEncuestaRepository
{
    public async Task<int> AddAsync(EncuestaPreanalitica e, CancellationToken ct = default)
    {
        const string sql = @"
INSERT INTO dbo.EncuestaPreanalitica (
  Fecha, NumeroRecepcion, NombreCompleto, Identificacion, Edad, Sexo, Telefono,
  Embarazada, PeriodoMenstrual, Ayuno, SintomasRespiratorios, Rechazo, MotivoRechazo,
  EnfermedadesBase, Medicamentos, ResponsableToma, HoraAtencion, Consentimiento,
  PacienteNombreFirma, PacienteDocFirma, RepNombre, RepDoc, FirmaPac, FirmaRep, CreadoAt
)
VALUES (
  @Fecha, @NumeroRecepcion, @NombreCompleto, @Identificacion, @Edad, @Sexo, @Telefono,
  @Embarazada, @PeriodoMenstrual, @Ayuno, @SintomasRespiratorios, @Rechazo, @MotivoRechazo,
  @EnfermedadesBase, @Medicamentos, @ResponsableToma, @HoraAtencion, @Consentimiento,
  @PacienteNombreFirma, @PacienteDocFirma, @RepNombre, @RepDoc, @FirmaPac, @FirmaRep, SYSUTCDATETIME()
);

SELECT CAST(SCOPE_IDENTITY() AS int);";

        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync();
        using var cmd = new SqlCommand(sql, con);
        // parámetros omitidos por brevedad...
        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }
}
```

**Repositorio EF Core**
```csharp
public class EncuestaEfRepository : IEncuestaRepository
{
    private readonly ApplicationDbContext _context;
    public EncuestaEfRepository(ApplicationDbContext context) => _context = context;

    public async Task<int> AddAsync(EncuestaPreanalitica e, CancellationToken ct = default)
    {
        _context.Encuestas.Add(e);
        await _context.SaveChangesAsync(ct);
        return e.Id;
    }
}
```

---

### 📦 Patrón DTO (Data Transfer Object)
Los **DTOs** se usan para intercambiar datos entre la capa de presentación y la de aplicación, sin exponer las entidades del dominio.  
Incluyen **Data Annotations** para validación automática de los modelos.

```csharp
public class CrearEncuestaDto
{
    [Required] public DateTime Fecha { get; init; }
    [Required, StringLength(100)] public string NombreCompleto { get; init; }
    [Required, Phone] public string Telefono { get; init; }
}
```

---

### 🧭 Métodos de Extensión
Se utilizan **métodos de extensión** para mantener un código limpio y reutilizable, agregando funcionalidad sin modificar las clases originales.

```csharp
public static class StringExtensions
{
    public static string ToUpperTrim(this string value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpper();
}
```

---

### 🔁 Mapeadores (Mapper Pattern)
Implementación del **Patrón Mapper** con **Mapster** (o AutoMapper opcionalmente) para convertir DTOs en entidades del dominio.

```csharp
public static class EncuestaMapper
{
    public static EncuestaPreanalitica ToEntity(this CrearEncuestaDto dto)
        => new()
        {
            Fecha = dto.Fecha,
            NombreCompleto = dto.NombreCompleto,
            Telefono = dto.Telefono
        };
}
```

---

### ✅ Validación automática personalizada
ASP.NET Core valida automáticamente los modelos con **Data Annotations**, y se configuró una respuesta JSON personalizada para devolver errores legibles en español.

```json
{
  "mensaje": "Hay errores en los datos enviados.",
  "detalles": [
    "El campo 'Telefono': Debe ingresar un número de teléfono válido."
  ]
}
```

---

## 🧠 Principios aplicados

- **Single Responsibility Principle (SRP)**  
- **Dependency Inversion Principle (DIP)**  
- **Open/Closed Principle (OCP)**  
- **Inversión de Control (IoC)**  
- **Inyección de Dependencias (DI)**  

---

## 🧰 Tecnologías utilizadas

| Tecnología | Propósito |
|-------------|------------|
| **ASP.NET Core 8.0** | Capa de presentación (API REST) |
| **C# 12** | Lenguaje base |
| **Entity Framework Core 8** | ORM y persistencia |
| **ADO.NET** | Acceso directo a SQL Server |
| **SQL Server 2022** | Base de datos |
| **Mapster / AutoMapper** | Conversión entre DTOs y entidades |
| **Data Annotations** | Validación automática |
| **Swagger / Swashbuckle** | Documentación interactiva |
| **Microsoft.Extensions.DependencyInjection** | Inyección de dependencias |
| **Métodos de Extensión** | Código limpio y reutilizable |

---

## 🧾 Flujo general de ejecución

```
Controller (WebApi)
   ↓
Service (Application)
   ↓
Mapper + Extension Methods
   ↓
Repository (Infrastructure)
   ↓
Database (SQL Server via ADO.NET / EF Core)
```

---

## 🧱 Beneficios de esta arquitectura

- Alta mantenibilidad y escalabilidad.  
- Bajo acoplamiento entre capas.  
- Dominio independiente de frameworks externos.  
- Fácil reemplazo entre ADO.NET y EF Core.  
- Código más limpio mediante mapeadores y extensiones.  
- Validación robusta y centralizada.  

---

## 🧪 Pruebas

El diseño desacoplado permite crear **tests unitarios e integrados** fácilmente, inyectando dependencias simuladas (`mock repositories`) mediante **xUnit**, **Moq** o **NSubstitute**.

---

## 🧾 Licencia

Este proyecto se distribuye bajo licencia **MIT**, permitiendo su uso, modificación y redistribución libremente, citando la fuente original.
