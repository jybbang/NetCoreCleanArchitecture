using AutoMapper;
using DaprCleanArchitecture.Application.Dtos;
using DaprCleanArchitecture.Domain.Entities;
using System;

namespace DaprCleanArchitecture.Infrastructure.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DateTimeOffset, DateTime>().ConvertUsing(x => x.UtcDateTime);

            CreateMap<TodoItem, TodoItemDto>();
        }
    }
}
