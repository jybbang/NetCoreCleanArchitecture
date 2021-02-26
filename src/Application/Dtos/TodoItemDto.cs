using DaprCleanArchitecture.Domain.ValueObjects;
using System;

namespace DaprCleanArchitecture.Application.Dtos
{
    public record TodoItemDto
    {
        public Guid Id { get; init; }

        public Detail Detail { get; init; }

        public bool Done { get; init; }
    }
}
