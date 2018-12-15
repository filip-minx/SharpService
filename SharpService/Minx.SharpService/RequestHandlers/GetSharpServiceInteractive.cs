using System.IO;
using System.Net;
using System.Reflection;

namespace Minx.SharpService.RequestHandlers
{
    class GetSharpServiceInteractive : GetRequestHandler
    {
        public override string UrlPattern => "/sharpservice/interactive";

        protected override void ProcessRequest(HttpListenerContext context, ScriptEnvironment script)
        {
            string index = null;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Minx.SharpService.Resources.interactive.html";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                index = reader.ReadToEnd();
            }

            HttpServer.SetResponseData(context.Response, index);
        }
    }
}
