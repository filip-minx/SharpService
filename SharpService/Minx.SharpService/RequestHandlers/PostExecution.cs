using System.IO;
using System.Net;

namespace Minx.SharpService.RequestHandlers
{
    class PostExecution : PostRequestHandler
    {
        public override string UrlPattern => "/sharpservice/executions";

        protected override void ProcessRequest(HttpListenerContext context, ScriptEnvironment script)
        {
            var code = new StreamReader(context.Request.InputStream).ReadToEnd();

            var result = script.Execute(code);

            HttpServer.SetResponseData(context.Response, result);
        }
    }
}
