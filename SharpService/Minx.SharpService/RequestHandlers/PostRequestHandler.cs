using Minx.SharpService.RequestHandlers;
using System.Net.Http;

namespace Minx.SharpService
{
    public abstract class PostRequestHandler : RequestHandler
    {
        public override HttpMethod HttpMethod => HttpMethod.Post;
    }
}
