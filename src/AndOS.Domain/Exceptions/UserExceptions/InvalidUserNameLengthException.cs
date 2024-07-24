namespace AndOS.Domain.Exceptions.UserExceptions;

public class InvalidUserNameLengthException : DomainLayerException
{
    public InvalidUserNameLengthException() : base()
    {
    }

    public InvalidUserNameLengthException(string message) : base(message)
    {
    }

    public InvalidUserNameLengthException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
