namespace AndOS.Domain.Exceptions.UserExceptions;

public class InvalidUserNameCharacters : DomainLayerException
{
    public InvalidUserNameCharacters() : base()
    {
    }

    public InvalidUserNameCharacters(string message) : base(message)
    {
    }

    public InvalidUserNameCharacters(string message, Exception innerException) : base(message, innerException)
    {
    }
}
