using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Minx.SharpService
{
    public class HttpServiceServer
    {
        private HttpServer httpServer;

        private List<(Regex Pattern, IHttpService Service)> serviceMappings = new List<(Regex, IHttpService)>();
        private Dictionary<string, IHttpService> serviceMappingsCache = new Dictionary<string, IHttpService>();

        public event EventHandler<ReportedEventArgs> Reported;

        public HttpServiceServer(string netInterface)
        {
            httpServer = new HttpServer($"http://{netInterface}/");
            httpServer.Start();
        }

        public void AddService(string urlPrefix, IHttpService service)
        {
            Regex regex = CreateServiceUrlPrefixRegex(urlPrefix);
            urlPrefix = ResolveServiceUrlPrefix(urlPrefix);

            serviceMappings.Add((regex, service));
        }

        public void ProcessRequests()
        {
            httpServer.ProcessRequests(ProcessRequest);
        }

        private void ReportRequest(HttpListenerRequest request)
        {
            Report($"{request.HttpMethod} - {request.RemoteEndPoint.Address}:{request.RemoteEndPoint.Port} {request.Url.AbsolutePath}");
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            ReportRequest(context.Request);

            IHttpService service = GetServiceForRequest(context.Request);

            service.ProcessRequest(context);
        }

        private IHttpService GetServiceForRequest(HttpListenerRequest request)
        {
            var url = request.Url.AbsolutePath + "/";

            if (serviceMappingsCache.TryGetValue(url, out var longestUrlPrefixService))
            {
                return longestUrlPrefixService;
            }

            int longestServiceUrlPrefixLength = 0;

            foreach (var serviceMapping in serviceMappings)
            {
                var match = serviceMapping.Pattern.Match(url);

                if (match.Success)
                {
                    var patternString = serviceMapping.Pattern.ToString();

                    if (longestServiceUrlPrefixLength < patternString.Length)
                    {
                        longestServiceUrlPrefixLength = patternString.Length;
                        longestUrlPrefixService = serviceMapping.Service;
                    }
                }
            }

            if (longestUrlPrefixService == null)
            {
                throw new InvalidOperationException("No service found for the request: " + url);
            }

            serviceMappingsCache.Add(url, longestUrlPrefixService);

            return longestUrlPrefixService;
        }

        private void Report(string message, ReportLevel level = ReportLevel.Informational)
        {
            Reported?.Invoke(this, new ReportedEventArgs(message, level));
        }

        private string ResolveServiceUrlPrefix(string urlPrefix)
        {
            if (!urlPrefix.StartsWith("/"))
            {
                urlPrefix = "/" + urlPrefix;
            }

            if (!urlPrefix.EndsWith("/"))
            {
                urlPrefix += "/";
            }

            return urlPrefix;
        }

        private Regex CreateServiceUrlPrefixRegex(string urlPattern)
        {
            var regexPattern = Regex.Replace(urlPattern, @"{(\w+)}", @"(?<$1>\w+)");

            return new Regex($"^{regexPattern}.*$", RegexOptions.IgnoreCase);
        }
    }
}
