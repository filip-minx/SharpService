using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Minx.SharpService
{
    public static class RequestHandlersRegister
    {
        private static List<RequestHandler> handlers = new List<RequestHandler>();

        static RequestHandlersRegister()
        {
            var handlerTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where((type) => { return !type.IsAbstract; })
                .Where((type) => { return typeof(RequestHandler).IsAssignableFrom(type); });

            foreach (var handlerType in handlerTypes)
            {
                handlers.Add((RequestHandler)Activator.CreateInstance(handlerType));
            }

            //handlers.Add(new PostExecution());
            //handlers.Add(new GetSharpServiceCLI());
            //handlers.Add(new GetExecution());
        }

        public static RequestHandler GetHandler(HttpListenerContext context)
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

        public static void AddCustomRequestHandler(RequestHandler handler)
        {
            handlers.Add(handler);
        }
    }
}
