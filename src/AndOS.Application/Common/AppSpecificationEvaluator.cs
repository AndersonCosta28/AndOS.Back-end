
namespace AndOS.Application.Common;

public class AppSpecificationEvaluator : Ardalis.Specification.EntityFrameworkCore.SpecificationEvaluator
{
    public static AppSpecificationEvaluator Instance { get; } = new AppSpecificationEvaluator();

    public AppSpecificationEvaluator() : base()
    {
        Evaluators.Add(QueryTagEvaluator.Instance);
    }
}