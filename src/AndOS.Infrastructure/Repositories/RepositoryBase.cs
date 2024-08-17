using AndOS.Application.Common;
using AndOS.Domain.Interfaces;
using Ardalis.Specification;

namespace AndOS.Infrastructure.Repositories;

public abstract class RepositoryBase<T> : IRepository<T> where T : class, IAggregateRoot
{
    protected DbContext _dbContext;
    protected ISpecificationEvaluator Evaluator;
    protected IMapperService _mapperService;
    private readonly DbSet<T> _dbSet;
    // We have a custom evaluator for QueryTag, therefore we're passing our custom specification evaluator
    protected RepositoryBase() { }

    protected RepositoryBase(DbContext dbContext, IMapperService mapperService) : this(dbContext, AppSpecificationEvaluator.Instance, mapperService) { }

    public RepositoryBase(DbContext dbContext, ISpecificationEvaluator specificationEvaluator, IMapperService mapperService)
    {
        _dbContext = dbContext;
        Evaluator = specificationEvaluator;
        _mapperService = mapperService;
        _dbSet = _dbContext.Set<T>();
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Add(entity);

        await SaveChangesAsync(cancellationToken);

        return entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.AddRange(entities);

        await SaveChangesAsync(cancellationToken);

        return entities;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);

        await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);

        await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities);

        await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteRangeAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = ApplySpecification(specification);
        _dbSet.RemoveRange(query);

        await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<T> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<T> SingleOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult> SingleOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).SingleOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        List<T> queryResult = await ApplySpecification(specification).ToListAsync(cancellationToken);

        return specification.PostProcessingAction == null ? queryResult : specification.PostProcessingAction(queryResult).ToList();
    }

    public virtual async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        List<TResult> queryResult = await ApplySpecification(specification).ToListAsync(cancellationToken);

        return specification.PostProcessingAction == null ? queryResult : specification.PostProcessingAction(queryResult).ToList();
    }

    public virtual async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification, true).CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification, true).AnyAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(cancellationToken);
    }

    public async Task<TResult> ProjectToFirstOrDefaultAsync<TResult>(ISpecification<T> specification, CancellationToken cancellationToken)
    {
        T data = await ApplySpecification(specification)
            .FirstOrDefaultAsync(cancellationToken);

        return await _mapperService.MapAsync<TResult>(data);
    }

    public async Task<List<TResult>> ProjectToListAsync<TResult>(CancellationToken cancellationToken)
    {
        List<T> data = await _dbSet.ToListAsync(cancellationToken);

        return await _mapperService.MapAsync<List<TResult>>(data);
    }

    public async Task<List<TResult>> ProjectToListAsync<TResult>(ISpecification<T> specification, CancellationToken cancellationToken)
    {
        List<T> data = await ApplySpecification(specification)
            .ToListAsync(cancellationToken);

        return await _mapperService.MapAsync<List<TResult>>(data);
    }

    public async Task<PagedResponse<TResult>> ProjectToListAsync<TResult>(ISpecification<T> specification, BaseFilter filter, CancellationToken cancellationToken)
    {
        int count = await ApplySpecification(specification).CountAsync(cancellationToken);
        Pagination pagination = new Pagination(count, filter);

        List<T> data = await ApplySpecification(specification)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToListAsync(cancellationToken);

        List<TResult> dataMapped = await _mapperService.MapAsync<List<TResult>>(data);
        return new PagedResponse<TResult>(dataMapped, pagination);
    }

    protected virtual IQueryable<T> ApplySpecification(ISpecification<T> specification, bool evaluateCriteriaOnly = false)
    {
        return Evaluator.GetQuery(_dbSet, specification, evaluateCriteriaOnly);
    }

    protected virtual IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification)
    {
        return Evaluator.GetQuery(_dbSet, specification);
    }
}


public class RepositoryBase<T, D> : RepositoryBase<T>
    where T : class, IAggregateRoot
    where D : class, T
{
    private readonly DbSet<D> _dbSet;

    protected RepositoryBase(DbContext dbContext, IMapperService mapperService) : this(dbContext, AppSpecificationEvaluator.Instance, mapperService) { }

    public RepositoryBase(DbContext dbContext, ISpecificationEvaluator specificationEvaluator, IMapperService mapperService) : base()
    {
        _dbContext = dbContext;
        Evaluator = specificationEvaluator;
        _dbSet = _dbContext.Set<D>();
        _mapperService = mapperService;
    }

    public override async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var appUser = (D)entity;
        _dbSet.Add(appUser);

        await SaveChangesAsync(cancellationToken);

        return appUser;
    }

    public override async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.AddRange(entities.Cast<D>());

        await SaveChangesAsync(cancellationToken);

        return entities;
    }

    public override async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update((D)entity);

        await SaveChangesAsync(cancellationToken);
    }

    public override async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove((D)entity);

        await SaveChangesAsync(cancellationToken);
    }

    public override async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities.Cast<D>());

        await SaveChangesAsync(cancellationToken);
    }

    public override async Task DeleteRangeAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = ApplySpecification(specification);
        _dbSet.RemoveRange(query.Cast<D>());

        await SaveChangesAsync(cancellationToken);
    }

    public override async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        var result = await _dbSet.ToListAsync(cancellationToken);
        return result.Cast<T>().ToList();
    }

    public override async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public override async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(cancellationToken);
    }

    protected override IQueryable<T> ApplySpecification(ISpecification<T> specification, bool evaluateCriteriaOnly = false)
    {
        return Evaluator.GetQuery(_dbSet, specification, evaluateCriteriaOnly);
    }

    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification)
    {
        return Evaluator.GetQuery(_dbSet, specification);
    }
}