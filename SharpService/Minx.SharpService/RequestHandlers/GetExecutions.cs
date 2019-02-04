using Newtonsoft.Json;
using System.Net;

namespace Minx.SharpService.RequestHandlers
{
    class GetExecutions : GetRequestHandler
    {
        public override string Route => "/sharpservice/executions";

        protected override void ProcessRequest(HttpListenerContext context, ScriptEnvironment script)
        {
            var responseJson = JsonConvert.SerializeObject(script.Executions);

            HttpServer.SetResponseText(context.Response, "application/json", responseJson);
        }
    }
}
