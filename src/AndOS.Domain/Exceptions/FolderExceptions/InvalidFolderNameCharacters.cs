namespace AndOS.Domain.Exceptions.FolderExceptions;

public class InvalidFolderNameCharacters : DomainLayerException
{
    public InvalidFolderNameCharacters() : base()
    {
    }

    public InvalidFolderNameCharacters(string message) : base(message)
    {
    }

    public InvalidFolderNameCharacters(string message, Exception innerException) : base(message, innerException)
    {
    }
}
