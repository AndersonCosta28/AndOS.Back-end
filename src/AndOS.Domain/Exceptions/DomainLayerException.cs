namespace AndOS.Domain.Exceptions;

public class DomainLayerException : Exception
{
    public DomainLayerException() : base()
    {
    }

    public DomainLayerException(string message) : base(message)
    {
    }

    public DomainLayerException(string message, Exception innerException) : base(message, innerException)
    {
    }
}