
/* 'Infrastructure.Zmq (net5.0)' 프로젝트에서 병합되지 않은 변경 내용
이전:
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
이후:
using System;
using Microsoft.Collections.Concurrent;
*/


/* 'Infrastructure.Zmq (net5.0)' 프로젝트에서 병합되지 않은 변경 내용
이전:
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

/* 'Infrastructure.Zmq (net5.0)' 프로젝트에서 병합되지 않은 변경 내용
이후:
using System.Collections.Generic;
using System.Linq;
/* 'Infrastructure.Zmq (net5.0)' 프로젝트에서 병합되지 않은 변경 내용
*/
using System.Threading;
using System.Threading.Tasks;

/* 'Infrastructure.Zmq (net5.0)' 프로젝트에서 병합되지 않은 변경 내용
이전:
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Options;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Zmqs;
using NetMQ;
using NetMQ.Sockets;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
이후:
using NetCoreCleanArchitecture.Infrastructure.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Options;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Zmqs;
using NetMQ;
using NetMQ.Sockets;
*/
/* 'Infrastructure.Zmq (net5.0)' 프로젝트에서 병합되지 않은 변경 내용
이전:
using System.Runtime.Zmq.Common.Zmqs;
이후:
using System.Runtime.Zmq.Common.Zmqs;
using System.Threading;
using System.Threading.Tasks;
*/


namespace NetCoreCleanArchitecture.Infrastructure.Zmq.EventBus
{
    public class ZmqEventBus : IEventBus
    {
        private readonly ZmqPublisher _pubsub;

        public ZmqEventBus(ZmqPublisher pubsub)
        {
            _pubsub = pubsub;
        }

        public Task PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            return _pubsub.PublishAsync(topic, message, cancellationToken);
        }
    }
}
