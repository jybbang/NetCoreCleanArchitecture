using System;

namespace NetCoreCleanArchitecture.Application.Common.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        public string Entity { get; }

        internal ForbiddenAccessException() : base() { }

        internal ForbiddenAccessException(string message) : base(message) { }

        internal ForbiddenAccessException(string message, Exception innerException) : base(message, innerException) { }

        public ForbiddenAccessException(string entity, object key) : base($"{entity} ({key}) was fobidden")
        {
            Entity = entity;
        }
    }
}
