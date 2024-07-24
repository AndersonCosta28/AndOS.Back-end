namespace AndOS.Application.Interfaces;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default);
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default);
    void Execute(Action operation);
    T Execute<T>(Func<T> operation);
}