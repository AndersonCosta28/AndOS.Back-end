using AndOS.Application.Common.Behaviours;
using FluentValidation;
using MediatR;

namespace AndOS.Infrastructure.Validator;

public static class ValidatorServiceCollection
{
    public static IServiceCollection AddValidatorService(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssemblyContaining<Application.Startup>()
               .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}
