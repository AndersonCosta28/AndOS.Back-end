namespace AndOS.Application.Interfaces;

public interface IMapperService
{
    TDestination Map<TSource, TDestination>(TSource source);
    TDestination Map<TDestination>(object source);

    Task<TDestination> MapAsync<TSource, TDestination>(TSource source);
    Task<TDestination> MapAsync<TDestination>(object source);
}