using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Linq;

namespace DaprCleanArchitecture.Infrastructure.Mappers
{
    public class Mapper : DaprCleanArchitecture.Application.Common.Interfaces.IMapper
    {
        private readonly AutoMapper.IMapper _mapper;

        public Mapper(AutoMapper.IMapper mapper)
        {
            _mapper = mapper;
        }

        public TDestination Map<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class
            => _mapper.Map<TSource, TDestination>(source);

        public TDestination Map<TDestination>(object source) where TDestination : class
            => _mapper.Map<TDestination>(source);

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
            => _mapper.Map<TSource, TDestination>(source, destination);
    }

    public static class MapperExetension
    {
        public static IQueryable<TDestination> Project<TSource, TDestination>(this IQueryable<TSource> queryable, IConfigurationProvider configuration)
            where TSource : class
            where TDestination : class
             => queryable.ProjectTo<TDestination>(configuration);

    }
}
