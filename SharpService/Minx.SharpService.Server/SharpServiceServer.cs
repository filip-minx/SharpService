using System;
using System.Threading.Tasks;

namespace Minx.SharpService.Server
{
    public class SharpServiceServer : HttpServiceServer
    {
        public SharpServiceServer(string netInterface, Globals globals) : base(netInterface)
        {
            Reported += (s, e) =>
            {
                if (e.Level != ReportLevel.Informational)
                {
                    Console.Write($"{e.Level}: ");
                }

                Console.WriteLine($"{e.Message}");
            };

            var resourceService = new FileService()
            {
                RootPath = @"Resources"
            };

            resourceService.AddUrlRedirection("/", "/interactive.html");

            var sharpService = new SharpService(globals);
            globals.Script.ScriptEnvironment = sharpService.ScriptEnvironment;

            AddService("/sharpservice", sharpService);
            AddService("/", resourceService);
        }

        public async Task StartProcessingRequests()
        {
            await Task.Run(() =>
            {
                ProcessRequests();
                Task.Delay(1).ContinueWith(t => StartProcessingRequests());
            });
        }
    }
}
