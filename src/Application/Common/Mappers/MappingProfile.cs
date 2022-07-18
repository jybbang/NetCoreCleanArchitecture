using System;
using AutoMapper;

namespace NetCoreCleanArchitecture.Application.Common.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DateTimeOffset, DateTime>().ConvertUsing(x => x.UtcDateTime);

            CreateMap<Guid, string>().ConvertUsing(g => g.ToString());

            CreateMap<string, Guid>().ConvertUsing(s => Guid.Parse(s));
        }
    }
}
