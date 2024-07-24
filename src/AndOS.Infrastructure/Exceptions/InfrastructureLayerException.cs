namespace AndOS.Infrastructure.Exceptions;

public class InfrastructureLayerException : Exception
{
    public InfrastructureLayerException()
    {
    }

    public InfrastructureLayerException(string message) : base(message)
    {
    }

    public InfrastructureLayerException(string message, Exception inner) : base(message, inner)
    {
    }
}
