using System;
using System.Net;
using System.Reflection;

namespace Minx.SharpService
{
    public class SharpService : IHttpService
    {
        private ScriptEnvironment scriptEnvironment;

        public RequestHandlersRegister RequestHandlers { get; private set; } = new RequestHandlersRegister();

        public event EventHandler<ReportedEventArgs> Reported;

        public SharpService(object globals)
        {
            RequestHandlers.LoadHandlersFromAssembly(Assembly.GetExecutingAssembly());

            scriptEnvironment = new ScriptEnvironment(globals);
        }

        public SharpService()
        {
            RequestHandlers.LoadHandlersFromAssembly(Assembly.GetExecutingAssembly());

            scriptEnvironment = new ScriptEnvironment();
        }
        
        public void ProcessRequest(HttpListenerContext context)
        {
            ReportRequest(context.Request);

            try
            {
                var handler = RequestHandlers.GetHandler(context);
                handler.InternalProcessRequest(context, scriptEnvironment);
            }
            catch (InvalidOperationException e)
            {
                var msg = "Invalid API call. " + e.Message;

                Report(msg, ReportLevel.Error);

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                HttpServer.SetResponseText(context.Response, "text/html", msg);
            }
        }

        private void ReportRequest(HttpListenerRequest request)
        {
            Report($"{request.HttpMethod} - {request.RemoteEndPoint.Address}:{request.RemoteEndPoint.Port} {request.Url.AbsolutePath}");
        }

        private void Report(string message, ReportLevel level = ReportLevel.Informational)
        {
            Reported?.Invoke(this, new ReportedEventArgs(message, level));
        }
    }
}
