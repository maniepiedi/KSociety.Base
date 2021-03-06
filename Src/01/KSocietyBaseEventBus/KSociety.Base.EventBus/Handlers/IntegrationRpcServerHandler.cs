﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.EventBus.Handlers
{
    public class IntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>
        : IntegrationGeneralHandler, IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>
        where TIntegrationEvent : IIntegrationEventRpc
        where TIntegrationEventReply : IIntegrationEventReply
    {
        protected readonly ILogger<IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>> Logger;

        public IntegrationRpcServerHandler(ILoggerFactory loggerFactory, IComponentContext componentContext)
            : base(loggerFactory, componentContext)
        {
            Logger = LoggerFactory.CreateLogger<IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>>();
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
    }
}
