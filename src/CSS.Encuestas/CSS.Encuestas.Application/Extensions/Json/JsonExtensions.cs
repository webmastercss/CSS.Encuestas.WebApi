using System.Text.Json;

namespace CSS.Encuestas.Application.Extensions.Serialize;
public static class JsonExtensions
{

    public static T Deserialize<T>(this string json, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("El contenido JSON no puede ser nulo o vacío.", nameof(json));

        return JsonSerializer.Deserialize<T>(json, options)
               ?? throw new InvalidOperationException("No se pudo deserializar el JSON.");
    }
}
