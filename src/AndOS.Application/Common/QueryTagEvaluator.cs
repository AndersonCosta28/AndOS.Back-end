using Ardalis.Specification;

namespace AndOS.Application.Common;

public class QueryTagEvaluator : IEvaluator
{
    private QueryTagEvaluator() { }
    public static QueryTagEvaluator Instance { get; } = new QueryTagEvaluator();

    public bool IsCriteriaEvaluator { get; } = true;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.Items.TryGetValue("TagWith", out object value) && value is string tag)
        {
            query = query.TagWith(tag);
        }

        return query;
    }
}