using Minx.SharpService.RequestHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Minx.SharpService
{
    public class RequestHandlersRegister
    {
        private List<RequestHandler> handlers = new List<RequestHandler>();

        public RequestHandler GetHandler(HttpListenerContext context)
        {
            foreach (var handler in handlers)
            {
                if (handler.UrlRegex.IsMatch(context.Request.Url.AbsolutePath)
                    && handler.HttpMethod.Method == context.Request.HttpMethod)
                {
                    return handler;
                }
            }

            throw new InvalidOperationException($"No request handler found for \"{context.Request.Url.AbsolutePath}\"");
        }

        public void AddHandler(RequestHandler handler)
        {
            handlers.Add(handler);
        }

        public void LoadHandlersFromAssembly(Assembly assembly)
        {
            var handlerTypes = assembly.GetTypes()
                .Where((type) => { return !type.IsAbstract && typeof(RequestHandler).IsAssignableFrom(type); });

            foreach (var handlerType in handlerTypes)
            {
                handlers.Add((RequestHandler)Activator.CreateInstance(handlerType));
            }
        }
    }
}
