using System;

namespace NetCoreCleanArchitecture.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public string Entity { get; }

        internal NotFoundException() : base() { }

        internal NotFoundException(string message) : base(message) { }

        internal NotFoundException(string message, Exception innerException) : base(message, innerException) { }

        public NotFoundException(string entity, object key) : base($"{entity} ({key}) was not found")
        {
            Entity = entity;
        }
    }
}
