using DotNnetcoreCleanArchitecture.Domain.Abstracts;
using System.Collections.Generic;

namespace DotNnetcoreCleanArchitecture.Domain.ValueObjects
{
    public record Detail(string Title, string Description, string Colour) : ValueObject;
}
