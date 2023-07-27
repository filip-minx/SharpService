using System.Threading;

namespace Minx.SharpService.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new SharpServiceServer("127.0.0.1:8081", new Globals());

            while (true)
            {
                server.ProcessRequests();
                Thread.Sleep(1);
            }
        }
    }
}
