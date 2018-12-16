using System;
using System.Net;
using System.Reflection;

namespace Minx.SharpService
{
    public class SharpService : IDisposable
    {
        private HttpServer http;
        private ScriptEnvironment scriptEnvironment;

        public RequestHandlersRegister RequestHandlers { get; private set; } = new RequestHandlersRegister();

        public event EventHandler<ReportedEventArgs> Reported;

        public SharpService(string netInterface, object globals)
        {
            InitServer(netInterface);

            RequestHandlers.LoadHandlersFromAssembly(Assembly.GetExecutingAssembly());

            scriptEnvironment = new ScriptEnvironment(globals);
        }

        public SharpService(string netInterface)
        {
            InitServer(netInterface);

            RequestHandlers.LoadHandlersFromAssembly(Assembly.GetExecutingAssembly());

            scriptEnvironment = new ScriptEnvironment();
        }

        private void InitServer(string netInterface)
        {
            var prefixes = GetPrefixes(netInterface);
            http = new HttpServer(prefixes);
            http.Start();
        }

        private string GetPrefixes(string netInterface)
        {
            return $"http://{netInterface}/sharpservice/";
        }

        public void ProcessRequests()
        {
            http.ProcessRequests(ProcessRequest);
        }

        private void ProcessRequest(HttpListenerContext context)
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
                HttpServer.SetResponseData(context.Response, msg);
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

        public void Dispose()
        {
            ((IDisposable)http).Dispose();
        }
    }
}
