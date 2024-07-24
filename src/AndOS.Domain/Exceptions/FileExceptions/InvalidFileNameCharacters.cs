namespace AndOS.Domain.Exceptions.FileExceptions;

public class InvalidFileNameCharacters : DomainLayerException
{
    public InvalidFileNameCharacters() : base()
    {
    }

    public InvalidFileNameCharacters(string message) : base(message)
    {
    }

    public InvalidFileNameCharacters(string message, Exception innerException) : base(message, innerException)
    {
    }
}
