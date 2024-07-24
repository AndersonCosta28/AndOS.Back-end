namespace AndOS.Domain.Exceptions.FolderExceptions;

public class InvalidFolderNameLengthException : DomainLayerException
{
    public InvalidFolderNameLengthException() : base()
    {
    }

    public InvalidFolderNameLengthException(string message) : base(message)
    {
    }

    public InvalidFolderNameLengthException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
