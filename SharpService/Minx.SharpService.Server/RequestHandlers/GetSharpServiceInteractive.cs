using Minx.SharpService.RequestHandlers;
using System.IO;
using System.Net;
using System.Reflection;

namespace Minx.SharpService.Server.RequestHandlers
{
    class GetSharpServiceInteractive : GetRequestHandler
    {
        public override string UrlPattern => "/sharpservice/interactive";

        protected override void ProcessRequest(HttpListenerContext context, ScriptEnvironment script)
        {
            var response = File.ReadAllText("Resources/interactive.html");

            HttpServer.SetResponseData(context.Response, response);
        }
    }
}
