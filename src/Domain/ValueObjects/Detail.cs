using DaprCleanArchitecture.Domain.Common;

namespace DaprCleanArchitecture.Domain.ValueObjects
{
    public record Detail(string Title, string Description, string Colour) : ValueObject;
}
