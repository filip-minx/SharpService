using System;
using System.Threading;

namespace Minx.SharpService.Server
{
    public class Globals
    {
        public ScriptEnvironmentGlobals Script { get; set; } = new ScriptEnvironmentGlobals();
    }

    class Program
    {
        static void Main(string[] args)
        {
            var serviceServer = new HttpServiceServer("*:80");

            serviceServer.Reported += (s, e) =>
            {
                if (e.Level != ReportLevel.Informational)
                {
                    Console.Write($"{e.Level}: ");
                }
                
                Console.WriteLine($"{e.Message}");
            };

            var resourceService = new FileService()
            {
                RootPath = "Resources"
            };

            resourceService.AddUrlRedirection("/", "/interactive.html");

            var globals = new Globals();
            var sharpService = new SharpService(globals);
            globals.Script.ScriptEnvironment = sharpService.ScriptEnvironment;

            serviceServer.AddService("/sharpservice", sharpService);
            serviceServer.AddService("/", resourceService);

            while (true)
            {
                serviceServer.ProcessRequests();
                Thread.Sleep(1);
            }
        }
    }
}
