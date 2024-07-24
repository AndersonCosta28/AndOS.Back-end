namespace AndOS.Application.Exceptions;

public class ApplicationLayerException : Exception
{
    public ApplicationLayerException() : base()
    {
    }

    public ApplicationLayerException(string message) : base(message)
    {
    }

    public ApplicationLayerException(string message, Exception inner) : base(message, inner)
    {
    }
}
