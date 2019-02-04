using Newtonsoft.Json;
using System.Net;

namespace Minx.SharpService.RequestHandlers
{
    class GetExecution : GetRequestHandler
    {
        public override string Route => "/sharpservice/executions/{id}";

        protected override void ProcessRequest(HttpListenerContext context, ScriptEnvironment script)
        {
            var executionId = GetArgument<int>("id");

            if (executionId > script.Executions.Count - 1)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.StatusDescription = $"Execution with the ID '{executionId}' does not exist.";

                HttpServer.SetResponseText(context.Response, "application/json", string.Empty);
            }
            else
            {
                var responseJson = JsonConvert.SerializeObject(script.Executions[executionId]);

                HttpServer.SetResponseText(context.Response, "application/json", responseJson);
            }
        }
    }
}
