using System;
using System.Reflection;
using System.Threading;

namespace Minx.SharpService.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new SharpService("*:80");

            service.RequestHandlers.LoadHandlersFromAssembly(Assembly.GetExecutingAssembly());

            service.Reported += (s, e) =>
            {
                Console.WriteLine($"{e.Level}: {e.Message}");
            };

            while (true)
            {
                service.ProcessRequests();
                Thread.Sleep(1);
            }
        }
    }
}
