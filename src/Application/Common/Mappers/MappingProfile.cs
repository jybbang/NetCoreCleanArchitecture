using AutoMapper;
using System;

namespace NetCoreCleanArchitecture.Application.Common.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DateTimeOffset, DateTime>().ConvertUsing(x => x.UtcDateTime);
        }
    }
}
