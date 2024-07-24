namespace AndOS.Domain.Exceptions.FileExceptions;

public class InvalidFileExtensionLengthException : DomainLayerException
{
    public InvalidFileExtensionLengthException() : base()
    {
    }

    public InvalidFileExtensionLengthException(string message) : base(message)
    {
    }

    public InvalidFileExtensionLengthException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
