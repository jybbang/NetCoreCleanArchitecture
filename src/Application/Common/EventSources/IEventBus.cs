﻿using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public interface IEventBus
    {
        Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default);
    }
}
