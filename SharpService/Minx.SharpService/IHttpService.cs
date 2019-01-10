using System.Net;

namespace Minx.SharpService
{
    public interface IHttpService
    {
        void ProcessRequest(HttpListenerContext context);
    }
}
