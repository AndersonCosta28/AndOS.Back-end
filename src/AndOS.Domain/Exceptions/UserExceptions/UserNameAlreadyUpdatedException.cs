namespace AndOS.Domain.Exceptions.UserExceptions;

public class UserNameAlreadyUpdatedException : DomainLayerException
{
    public UserNameAlreadyUpdatedException() : base()
    {
    }

    public UserNameAlreadyUpdatedException(string message) : base(message)
    {
    }

    public UserNameAlreadyUpdatedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
