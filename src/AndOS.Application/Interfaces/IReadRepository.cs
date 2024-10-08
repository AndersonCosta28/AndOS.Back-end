﻿using AndOS.Application.Common;
using AndOS.Domain.Interfaces;
using Ardalis.Specification;

namespace AndOS.Application.Interfaces;

public interface IReadRepository<T> where T : class, IAggregateRoot
{
    Task<T> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task<TResult> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);
    Task<T> SingleOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task<TResult> SingleOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);
    Task<List<T>> ListAsync(CancellationToken cancellationToken = default);
    Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    Task<TResult> ProjectToFirstOrDefaultAsync<TResult>(ISpecification<T> specification, CancellationToken cancellationToken);
    Task<List<TResult>> ProjectToListAsync<TResult>(CancellationToken cancellationToken);
    Task<List<TResult>> ProjectToListAsync<TResult>(ISpecification<T> specification, CancellationToken cancellationToken);
    Task<PagedResponse<TResult>> ProjectToListAsync<TResult>(ISpecification<T> specification, BaseFilter filter, CancellationToken cancellationToken);
}