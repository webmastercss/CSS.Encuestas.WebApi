# 馃 CSS.Encuestas

### API REST construida con Arquitectura de Cebolla (Onion Architecture), ADO.NET, EF Core, Inyecci贸n de Dependencias, M茅todos de Extensi贸n y Patr贸n Repository.

---

## 馃摉 Descripci贸n del proyecto

**CSS.Encuestas** es una API REST desarrollada con **ASP.NET Core 8** que implementa la **Arquitectura de Cebolla (Onion Architecture)**.  
Su objetivo es ofrecer una estructura **modular, mantenible y desacoplada**, aplicando principios **SOLID**, **Inversi贸n de Dependencias (DIP)** e **Inversi贸n de Control (IoC)**, junto con **Inyecci贸n de Dependencias (DI)**.  

El sistema gestiona **encuestas preanal铆ticas**, utilizando dos enfoques de acceso a datos:  
- **ADO.NET**, para un control preciso y directo sobre las consultas SQL.  
- **Entity Framework Core**, para operaciones ORM y persistencia de entidades.  

Adem谩s, el proyecto utiliza **m茅todos de extensi贸n** y **mapeadores (Mapper Pattern)** para mantener un c贸digo limpio, reutilizable y desacoplado.

---

## 馃З Estructura de la soluci贸n

```
CSS.Encuestas.sln
鈹?鈹溾攢鈹€ CSS.Encuestas.Domain            鈫?Entidades del dominio y contratos base
鈹溾攢鈹€ CSS.Encuestas.Application       鈫?DTOs, casos de uso, servicios y mapeadores
鈹溾攢鈹€ CSS.Encuestas.Infrastructure    鈫?Implementaci贸n de repositorios (ADO.NET / EF Core)
鈹斺攢鈹€ CSS.Encuestas.WebApi            鈫?Capa de presentaci贸n (controladores y endpoints)
```

---

## 鈿欙笍 Principales caracter铆sticas

### 馃П Arquitectura de Cebolla (Onion Architecture)
Organiza el c贸digo en capas conc茅ntricas donde **las dependencias fluyen siempre hacia el dominio**.  
Esto garantiza independencia tecnol贸gica, mejor mantenibilidad y facilidad para realizar cambios sin afectar otras capas.

---

### 馃挕 Inversi贸n de Control (IoC)
Se implementa mediante el **contenedor de dependencias de ASP.NET Core**, que administra la creaci贸n, ciclo de vida y resoluci贸n de dependencias de todos los componentes.

```csharp
builder.Services.AddScoped<IEncuestaRepository, EncuestaAdoRepository>();
builder.Services.AddScoped<IEncuestaService, EncuestaService>();
```

---

### 馃З Inyecci贸n de Dependencias (DI)
Las dependencias se reciben a trav茅s del constructor, evitando acoplamientos directos y favoreciendo la testabilidad y el cumplimiento del **principio D (Dependency Inversion)** de SOLID.

```csharp
public class EncuestaAdoRepository(IConfiguration config) : IEncuestaRepository
{
    private readonly string _connectionString = config.GetConnectionString("local")!;
}
```

---

### 馃攧 Inversi贸n de Dependencia (DIP)
El dominio depende de **abstracciones (interfaces)**, no de implementaciones concretas.  
Esto permite intercambiar libremente tecnolog铆as de acceso a datos (EF Core 鈫?ADO.NET) sin modificar la l贸gica del dominio.

```csharp
public interface IEncuestaRepository
{
    Task<int> AddAsync(EncuestaPreanalitica entity, CancellationToken ct = default);
}
```

---

### 馃梻锔?Patr贸n Repository
Encapsula la l贸gica de acceso a datos, separando la persistencia de la l贸gica de negocio.  
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
        // par谩metros omitidos por brevedad...
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

### 馃摝 Patr贸n DTO (Data Transfer Object)
Los **DTOs** se usan para intercambiar datos entre la capa de presentaci贸n y la de aplicaci贸n, sin exponer las entidades del dominio.  
Incluyen **Data Annotations** para validaci贸n autom谩tica de los modelos.

```csharp
public class CrearEncuestaDto
{
    [Required] public DateTime Fecha { get; init; }
    [Required, StringLength(100)] public string NombreCompleto { get; init; }
    [Required, Phone] public string Telefono { get; init; }
}
```

---

### 馃Л M茅todos de Extensi贸n
Se utilizan **m茅todos de extensi贸n** para mantener un c贸digo limpio y reutilizable, agregando funcionalidad sin modificar las clases originales.

```csharp
public static class StringExtensions
{
    public static string ToUpperTrim(this string value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpper();
}
```

---

### 馃攣 Mapeadores (Mapper Pattern)
Implementaci贸n del **Patr贸n Mapper** con **Mapster** (o AutoMapper opcionalmente) para convertir DTOs en entidades del dominio.

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

### 鉁?Validaci贸n autom谩tica personalizada
ASP.NET Core valida autom谩ticamente los modelos con **Data Annotations**, y se configur贸 una respuesta JSON personalizada para devolver errores legibles en espa帽ol.

```json
{
  "mensaje": "Hay errores en los datos enviados.",
  "detalles": [
    "El campo 'Telefono': Debe ingresar un n煤mero de tel茅fono v谩lido."
  ]
}
```

---

## 馃 Principios aplicados

- **Single Responsibility Principle (SRP)**  
- **Dependency Inversion Principle (DIP)**  
- **Open/Closed Principle (OCP)**  
- **Inversi贸n de Control (IoC)**  
- **Inyecci贸n de Dependencias (DI)**  

---

## 馃О Tecnolog铆as utilizadas

| Tecnolog铆a | Prop贸sito |
|-------------|------------|
| **ASP.NET Core 8.0** | Capa de presentaci贸n (API REST) |
| **C# 12** | Lenguaje base |
| **Entity Framework Core 8** | ORM y persistencia |
| **ADO.NET** | Acceso directo a SQL Server |
| **SQL Server 2022** | Base de datos |
| **Mapster / AutoMapper** | Conversi贸n entre DTOs y entidades |
| **Data Annotations** | Validaci贸n autom谩tica |
| **Swagger / Swashbuckle** | Documentaci贸n interactiva |
| **Microsoft.Extensions.DependencyInjection** | Inyecci贸n de dependencias |
| **M茅todos de Extensi贸n** | C贸digo limpio y reutilizable |

---

## 馃Ь Flujo general de ejecuci贸n

```
Controller (WebApi)
   鈫?Service (Application)
   鈫?Mapper + Extension Methods
   鈫?Repository (Infrastructure)
   鈫?Database (SQL Server via ADO.NET / EF Core)
```

---

## 馃П Beneficios de esta arquitectura

- Alta mantenibilidad y escalabilidad.  
- Bajo acoplamiento entre capas.  
- Dominio independiente de frameworks externos.  
- F谩cil reemplazo entre ADO.NET y EF Core.  
- C贸digo m谩s limpio mediante mapeadores y extensiones.  
- Validaci贸n robusta y centralizada.  

---

## 馃И Pruebas

El dise帽o desacoplado permite crear **tests unitarios e integrados** f谩cilmente, inyectando dependencias simuladas (`mock repositories`) mediante **xUnit**, **Moq** o **NSubstitute**.

---

## 馃Ь Licencia

Este proyecto se distribuye bajo licencia **MIT**, permitiendo su uso, modificaci贸n y redistribuci贸n libremente, citando la fuente original.
