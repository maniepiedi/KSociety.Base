﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Autofac;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.EventBus.Handlers
{
    public class IntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>
        : IntegrationGeneralHandler, IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>
        where TIntegrationEvent : IIntegrationEvent
        where TIntegrationEventReply : IIntegrationEventReply
    {
        protected readonly ILogger<IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>> Logger;

        public BufferBlock<TIntegrationEventReply> Queue { get; }
        public bool IsEmpty => Queue.Count == 0;


        public IntegrationRpcHandler(ILoggerFactory loggerFactory, IComponentContext componentContext)
            : base(loggerFactory, componentContext)
        {
            Logger = LoggerFactory.CreateLogger<IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>>();
            Queue = new BufferBlock<TIntegrationEventReply>();
        }

        public virtual TIntegrationEventReply HandleRpc(TIntegrationEvent @event, CancellationToken cancel = default)
        {
            Logger.LogWarning("IntegrationRpcHandler HandleRpc: NotImplemented!");
            throw new NotImplementedException();
        }

        public virtual ValueTask<TIntegrationEventReply> HandleRpcAsync(TIntegrationEvent @event, CancellationToken cancel = default)
        {
            Logger.LogWarning("IntegrationRpcHandler HandleRpcAsync: NotImplemented!");
            throw new NotImplementedException();
        }

        public virtual async ValueTask HandleReply(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default)
        {
            await Queue.SendAsync(@integrationEventReply, cancel).ConfigureAwait(false);
        }

        public virtual async ValueTask<bool> Enqueue(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default)
        {
            return await Queue.SendAsync(@integrationEventReply, cancel).ConfigureAwait(false);
        }

        public virtual async ValueTask<TIntegrationEventReply> Take(CancellationToken cancel = default)
        {
            return await Queue.ReceiveAsync(cancel).ConfigureAwait(false);
        }
    }
}
