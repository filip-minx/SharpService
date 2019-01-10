using System;
using System.Threading;

namespace Minx.SharpService.Server
{
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

            serviceServer.AddService("/sharpservice", new SharpService());
            serviceServer.AddService("/", resourceService);

            while (true)
            {
                serviceServer.ProcessRequests();
                Thread.Sleep(1);
            }
        }
    }
}
