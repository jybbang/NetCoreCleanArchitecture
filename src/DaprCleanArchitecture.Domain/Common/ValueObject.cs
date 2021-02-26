namespace DaprCleanArchitecture.Domain.Common
{
    public abstract record ValueObject
    {
        public object WithClone() => this with { };
    }
}
