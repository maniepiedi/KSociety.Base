﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using KSociety.Base.App.Shared;
using KSociety.Base.Srv.Shared.Interface;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Srv.Shared.Class
{
    public class CommandHandlerAsync : ICommandHandlerAsync
    {
        #region [ExecuteListWithResponseAsync<TRequest, TRequestList, TResponse>]

        public async ValueTask<TResponse> ExecuteListWithResponseAsync<TRequest, TRequestList, TResponse>(ILoggerFactory loggerFactory, IComponentContext componentContext, TRequestList request, CancellationToken cancellationToken = default)
            where TRequest : IRequest, new()
            where TRequestList : IAppList<TRequest>, new()
            where TResponse : IResponse, new()
        {
            var logger = loggerFactory.CreateLogger<CommandHandlerAsync>();
            return await ExecutingListWithResponseAsync<TRequest, TRequestList, TResponse>(logger, componentContext, request, cancellationToken).ConfigureAwait(false);
        }

        private static async ValueTask<TResponse> ExecutingListWithResponseAsync<TRequest, TRequestList, TResponse>(
            ILogger logger,
            IComponentContext componentContext,
            TRequestList request,
            CancellationToken cancellationToken = default
        )
            where TRequest : IRequest, new()
            where TRequestList : IAppList<TRequest>, new()
            where TResponse : IResponse, new()
        {
            var openType = typeof(IRequestListHandlerWithResponseAsync<,,>); // Generic open type.
            var type = openType.MakeGenericType(typeof(TRequest), typeof(TRequestList), typeof(TResponse)); // Type is your runtime type.

            if (!componentContext.IsRegistered(type))
            {
                logger.LogError("ExecuteAsync: " + type.FullName + " - " + " Request: " + typeof(TRequest).FullName + " Response: " + typeof(TResponse).FullName + " not registered!");
                return new TResponse();
            }

            try
            {
                var requestHandler = componentContext.Resolve(type);
                var methodInfo = type.GetMethod("ExecuteAsync");
                //logger.LogTrace("ExecuteAsync: " + openType.Name + " - " + " Request: " + typeof(TRequest).FullName + " Response: " + typeof(TResponse).FullName);
                return await ((ValueTask<TResponse>)methodInfo?.Invoke(requestHandler, new[] { (object)request, cancellationToken })).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw LogExceptionManager.LogException(logger, ex);
            }
        }

        #endregion

        #region [Execute<TRequest, TResponse>]

        public async ValueTask<TResponse> ExecuteWithResponseAsync<TRequest, TResponse>(ILoggerFactory loggerFactory, IComponentContext componentContext, TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest, new()
            where TResponse : IResponse, new()
        {
            var logger = loggerFactory.CreateLogger<CommandHandlerAsync>();
            return await ExecutingWithResponseAsync<TRequest, TResponse>(logger, componentContext, request, cancellationToken).ConfigureAwait(false);
        }

        private static async ValueTask<TResponse> ExecutingWithResponseAsync<TRequest, TResponse>(
            ILogger logger,
            IComponentContext componentContext,
            TRequest request,
            CancellationToken cancellationToken = default
        )
            where TRequest : IRequest, new()
            where TResponse : IResponse, new()
        {
            var openType = typeof(IRequestHandlerWithResponseAsync<,>); // Generic open type.
            var type = openType.MakeGenericType(typeof(TRequest), typeof(TResponse)); // Type is your runtime type.

            if (!componentContext.IsRegistered(type))
            {
                logger.LogError("ExecuteAsync: " + type.FullName + " - " + " Request: " + typeof(TRequest).FullName + " Response: " + typeof(TResponse).FullName + " not registered!");
                return new TResponse();
            }

            try
            {
                var requestHandler = componentContext.Resolve(type);
                var methodInfo = type.GetMethod("ExecuteAsync");
                //logger.LogTrace("ExecuteAsync: " + openType.Name + " - " + " Request: " + typeof(TRequest).FullName + " Response: " + typeof(TResponse).FullName);
                return await ((ValueTask<TResponse>)methodInfo?.Invoke(requestHandler, new[] { (object)request, cancellationToken })).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw LogExceptionManager.LogException(logger, ex);
            }
        }

        #endregion

        #region [Execute]

        public async ValueTask ExecuteAsync(ILoggerFactory loggerFactory, IComponentContext componentContext, string serviceName, CancellationToken cancellationToken = default)
        {
            var logger = loggerFactory.CreateLogger<CommandHandlerAsync>();
            await ExecutingAsync(logger, componentContext, serviceName, cancellationToken).ConfigureAwait(false);
        }
        
        private static async ValueTask ExecutingAsync(ILogger logger, IComponentContext componentContext, string serviceName, CancellationToken cancellationToken = default)
        {
            try
            {
                var type = typeof(IRequestHandlerAsync); // Type is your runtime type.
                if (!componentContext.IsRegisteredWithName<IRequestHandlerAsync>(serviceName))
                {
                    logger.LogError("ExecuteAsync: " + type.FullName + " - " + serviceName + " not registered!");
                    return;
                }

                try
                {
                    var requestHandler = componentContext.ResolveNamed<IRequestHandlerAsync>(serviceName);
                    var methodInfo = type.GetMethod("ExecuteAsync");
                    //logger.LogTrace("ExecuteAsync: " + type.Name + " - " + serviceName + " Response: ValueTask");
                    if (methodInfo != null) await ((ValueTask)methodInfo.Invoke(requestHandler, new[] { (object)cancellationToken })).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw LogExceptionManager.LogException(logger, ex);
                }
            }
            catch (Exception eex)
            {
                if (eex.InnerException != null) { throw eex.InnerException; }

                throw;
            }
        }

        #endregion

        #region [Execute<TRequest>]

        public async ValueTask ExecuteAsync<TRequest>(ILoggerFactory loggerFactory, IComponentContext componentContext, TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest, new()
        {
            var logger = loggerFactory.CreateLogger<CommandHandlerAsync>();
            await ExecutingAsync(logger, componentContext, request, cancellationToken).ConfigureAwait(false);
        }
        
        private static async ValueTask ExecutingAsync<TRequest>(ILogger logger, IComponentContext componentContext, TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest, new()
        {
            try
            {
                var openType = typeof(IRequestHandlerAsync<>); // Generic open type.
                var type = openType.MakeGenericType(typeof(TRequest)); // Type is your runtime type.
                if (!componentContext.IsRegistered(type))
                {
                    logger.LogError("ExecuteAsync: " + type.FullName + " - " + " Request: " + typeof(TRequest).FullName + " not registered!");
                    return;
                }

                try
                {
                    var requestHandler = componentContext.Resolve(type);
                    var methodInfo = type.GetMethod("ExecuteAsync");
                    //logger.LogTrace("ExecuteAsync: " + openType.Name + " - " + " Request: " + typeof(TRequest).FullName + " Response: void");
                    if (methodInfo != null) await ((ValueTask)methodInfo.Invoke(requestHandler, new[] { (object)request, cancellationToken })).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw LogExceptionManager.LogException(logger, ex);
                }
            }
            catch (Exception eex)
            {
                if (eex.InnerException != null) { throw eex.InnerException; }

                throw;
            }
        }

        #endregion

        #region [ExecuteWithResponse<TResponse>]

        public async ValueTask<TResponse> ExecuteWithResponseAsync<TResponse>(ILoggerFactory loggerFactory, IComponentContext componentContext, CancellationToken cancellationToken = default)
            where TResponse : IResponse, new()
        {
            var logger = loggerFactory.CreateLogger<CommandHandlerAsync>();
            return await ExecutingWithResponseAsync<TResponse>(logger, componentContext, cancellationToken).ConfigureAwait(false);
        }
        
        private static async ValueTask<TResponse> ExecutingWithResponseAsync<TResponse>(ILogger logger, IComponentContext componentContext, CancellationToken cancellationToken = default)
            where TResponse : IResponse, new()
        {
            try
            {
                var openType = typeof(IRequestHandlerWithResponseAsync<>); // Generic open type.
                var type = openType.MakeGenericType(typeof(TResponse)); // Type is your runtime type.
                if (!componentContext.IsRegistered(type))
                {
                    logger.LogError("ExecuteAsync: " + type.FullName + " - " + " Response: " + typeof(TResponse).FullName + " not registered!");
                    return new TResponse();
                }

                try
                {
                    var requestHandler = componentContext.Resolve(type);
                    var methodInfo = type.GetMethod("ExecuteAsync");
                    //logger.LogTrace("ExecuteAsync: " + openType.Name + " - " + " Response: " + typeof(TResponse).FullName);
                    return await ((ValueTask<TResponse>)methodInfo?.Invoke(requestHandler, new[] { (object)cancellationToken })).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw LogExceptionManager.LogException(logger, ex);
                }
            }
            catch (Exception eex)
            {
                if (eex.InnerException != null) { throw eex.InnerException; }

                throw;
            }
        }

        #endregion
    }
}
