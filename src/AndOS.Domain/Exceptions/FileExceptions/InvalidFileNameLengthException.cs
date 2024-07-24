namespace AndOS.Domain.Exceptions.FileExceptions;

public class InvalidFileNameLengthException : DomainLayerException
{
    public InvalidFileNameLengthException() : base()
    {
    }

    public InvalidFileNameLengthException(string message) : base(message)
    {
    }

    public InvalidFileNameLengthException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
