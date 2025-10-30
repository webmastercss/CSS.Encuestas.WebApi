using System.Reflection;
using CSS.Encuestas.Domain.Exceptions;

namespace CSS.Encuestas.Application.Extensions.Mappings;


public static class ObjectMapper
{
    /// <summary>
    /// Convierte un objeto de tipo origen TSource a un nuevo objeto del tipo destino TDestination,
    /// mapeando las propiedades con el mismo nombre.
    /// </summary>

    public static TDestination MapTo<TDestination>(this object source)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(nameof(source));

            var sourceType = source.GetType();
            var destType = typeof(TDestination);

            var ctor = destType.GetConstructors().FirstOrDefault();

            if (ctor != null && ctor.GetParameters().Length > 0)
            {
                var args = ctor.GetParameters()
                    .Select(p =>
                    {
                        var prop = sourceType.GetProperty(p.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        return prop != null ? prop.GetValue(source) : GetDefault(p.ParameterType);
                    })
                    .ToArray();

                return (TDestination)Activator.CreateInstance(destType, args)!;
            }


            var destination = Activator.CreateInstance<TDestination>();
            foreach (var sProp in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var dProp = destType.GetProperty(sProp.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (dProp != null && dProp.CanWrite)
                {
                    var value = sProp.GetValue(source);
                    if (value != null)
                        dProp.SetValue(destination, value);
                }
            }
            return destination;

        }
        catch
        {

            throw new MappingException();

        }
    }

    private static object GetDefault(Type type) =>
        type.IsValueType ? Activator.CreateInstance(type)! : null!;



    /// <summary>
    /// Convierte una lista de objetos de tipo TSource a una lista de TDestination.
    /// </summary>
    public static IEnumerable<TDestination> MapToEnumerable<TDestination>(this IEnumerable<object> sourceList)
        where TDestination : new()
    {

        try
        {

            ArgumentNullException.ThrowIfNull(nameof(sourceList));

            return [.. sourceList.Select(item => item.MapTo<TDestination>())];

        }
        catch
        {

            throw new MappingException();

        }
    }
}
