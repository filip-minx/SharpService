using System.Net;

namespace Minx.SharpService.RequestHandlers
{
    class PostExecution : PostRequestHandler
    {
        public override string UrlPattern => "/sharpservice/executions";

        protected override void ProcessRequest(HttpListenerContext context, ScriptEnvironment script)
        {

            var code = HttpServer.ReadRequestText(context.Request);

            var result = script.Execute(code);

            HttpServer.SetResponseText(context.Response, "text/html", result);
        }
    }
}
