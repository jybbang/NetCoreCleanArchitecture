using AutoMapper;
using AutoMapper.QueryableExtensions;
using NetCoreCleanArchitecture.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
