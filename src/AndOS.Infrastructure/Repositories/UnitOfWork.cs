using Amazon.S3.Model.Internal.MarshallTransformations;
using AndOS.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;

namespace AndOS.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{

    private readonly AppDbContext _context = context;
    private IDbContextTransaction _transaction;

    public DatabaseFacade Database => _context.Database;

    #region synchronous
    void beginTransaction()
    {
        _transaction = _context.Database.BeginTransaction();
    }

    void commit()
    {
        _context.SaveChanges();
        _transaction.Commit();
    }

    public void rollback()
    {
        if (_transaction != null)
            _transaction.Rollback();
    }

    public void Dispose()
    {
        if (_transaction != null)
        {
            _transaction.Dispose();
            _transaction = null;
        }

        _context.Dispose();
    }
    public void Execute(Action operation)
    {
        var executionStrategy = _context.Database.CreateExecutionStrategy();
        executionStrategy.Execute(() =>
        {
            try
            {
                beginTransaction();
                operation();
                commit();
            }
            catch (Exception)
            {
                rollback();
                throw;
            }
            finally
            {
                rollback();
            }
        });
    }


    public T Execute<T>(Func<T> operation)
    {
        var executionStrategy = _context.Database.CreateExecutionStrategy();
        var result = executionStrategy.Execute(() =>
         {
             try
             {
                 beginTransaction();
                 var result = operation();
                 commit();
                 return result;
             }
             catch (Exception)
             {
                 rollback();
                 throw;
             }
             finally
             {
                 rollback();
             }
         });
        return result;
    }
    #endregion

    #region asynchronous
    async Task beginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        //// Verifica se já existe uma transação ativa
        //if (_context.Database.CurrentTransaction == null)
        //{
        //    // Se não houver uma transação ativa, inicia uma nova
        //    _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        //}
        //else
        //{
        //    // Se já houver uma transação ativa, usa-a
        //    _transaction = _context.Database.CurrentTransaction;
        //}
    }
    async Task commitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await rollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    async Task rollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
                await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        await _context.DisposeAsync();
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        var executionStrategy = _context.Database.CreateExecutionStrategy();
        await executionStrategy.ExecuteAsync(async (CancellationToken cancellation) =>
        {
            try
            {
                await beginTransactionAsync(cancellation);
                await operation(cancellation);
                await commitAsync(cancellation);
            }
            catch (Exception)
            {
                await rollbackAsync(cancellation);
                throw;
            }
            finally
            {
                await DisposeAsync();
            }
        }, cancellationToken);
    }

    public Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        var executionStrategy = _context.Database.CreateExecutionStrategy();
        var result = executionStrategy.ExecuteAsync(async (CancellationToken cancellationToken) =>
        {
            try
            {
                await beginTransactionAsync(cancellationToken);
                var result = await operation(cancellationToken);
                await commitAsync(cancellationToken);
                return result;
            }
            catch (Exception)
            {
                await rollbackAsync(cancellationToken);
                throw;
            }
            finally
            {
                await DisposeAsync();
            }
        }, cancellationToken);
        return result;
    }
    #endregion
}