using Mapster;
using MapsterMapper;

namespace AndOS.API.Mapper;

public class MapperService(IMapper mapper) : IMapperService
{
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return MapAsync<TSource, TDestination>(source).Result;
    }

    public TDestination Map<TDestination>(object source)
    {
        return MapAsync<TDestination>(source).Result;
    }

    public async Task<TDestination> MapAsync<TSource, TDestination>(TSource source)
    {
        return await mapper.From(source).AdaptToTypeAsync<TDestination>();
    }
    public async Task<TDestination> MapAsync<TDestination>(object source)
    {
        return await mapper.From(source).AdaptToTypeAsync<TDestination>();
    }
}