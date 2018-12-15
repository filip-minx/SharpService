using System.Net.Http;

namespace Minx.SharpService.RequestHandlers
{
    public abstract class GetRequestHandler : RequestHandler
    {
        public override HttpMethod HttpMethod => HttpMethod.Get;
    }
}
