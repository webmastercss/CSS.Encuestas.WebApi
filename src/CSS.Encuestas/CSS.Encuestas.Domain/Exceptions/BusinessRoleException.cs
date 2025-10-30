namespace CSS.Encuestas.Domain.Exceptions;
public class BusinessRoleException: Exception
{
    public BusinessRoleException():base("Business rule violation: Operation cannot be completed due to business constraints.")
    {
      
    }

    public BusinessRoleException(string? message) : base(message)
    {
    }
}
