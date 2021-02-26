using AutoMapper;
using System;

namespace DaprCleanArchitecture.Infrastructure.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DateTimeOffset, DateTime>().ConvertUsing(x => x.UtcDateTime);
        }
    }
}
