namespace CSS.Encuestas.Domain.Exceptions;
public class MappingException : Exception
{
    public MappingException():base("Error when mapping objects")
    {
        
    }

    public MappingException(string? message) : base(message)
    {
    }
}
