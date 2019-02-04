using Newtonsoft.Json;
using System.Net;

namespace Minx.SharpService.RequestHandlers
{
    class PostExecution : PostRequestHandler
    {
        public override string UrlPattern => "/sharpservice/executions";

        protected override void ProcessRequest(HttpListenerContext context, ScriptEnvironment script)
        {
            var requestText = HttpServer.ReadRequestText(context.Request);

            var executionRequest = JsonConvert.DeserializeObject<ScriptExecution>(requestText);

            var executionResult = script.Execute(executionRequest.Code);

            var resultJson = JsonConvert.SerializeObject(executionResult);

            HttpServer.SetResponseText(context.Response, "application/json", resultJson);
        }
    }
}
