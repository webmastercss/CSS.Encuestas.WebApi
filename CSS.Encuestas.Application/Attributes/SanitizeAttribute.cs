using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace CSS.Encuestas.Application.Attributes;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class SanitizeAttribute : ValidationAttribute
{

    private static readonly Regex Dangerous = new(@"[<>""'%;()&+]", RegexOptions.Compiled);
    private static readonly Regex MultiSpace = new(@"\s{2,}", RegexOptions.Compiled);

    public SanitizeAttribute() : base("El valor contiene caracteres no permitidos.")
    {
    }

    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        if (value is null) return ValidationResult.Success;

        var input = value.ToString();

        // Normaliza Unicode (quita combinaciones raras)
        input = input.Normalize(NormalizationForm.FormKC);

        // Recorta y colapsa espacios
        input = input.Trim();
        input = MultiSpace.Replace(input, " ");

        // Elimina caracteres potencialmente peligrosos
        input = Dangerous.Replace(input, string.Empty);

        // Reinyecta el valor saneado en la propiedad (requiere setter)
        var prop = context.ObjectType.GetProperty(context.MemberName!);
        if (prop is not null && prop.CanWrite)
        {
            prop.SetValue(context.ObjectInstance, input);
        }

        return ValidationResult.Success;
    }


}