namespace AndOS.Domain.Exceptions.FileExceptions;

public class InvalidFileExtensionCharacters : DomainLayerException
{
    public InvalidFileExtensionCharacters() : base()
    {
    }

    public InvalidFileExtensionCharacters(string message) : base(message)
    {
    }

    public InvalidFileExtensionCharacters(string message, Exception innerException) : base(message, innerException)
    {
    }
}
