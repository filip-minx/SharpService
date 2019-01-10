using MimeMapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Minx.SharpService.Server
{
    public class FileService : IHttpService
    {
        private string defaultRootPath;

        public Dictionary<string, string> UrlRedirections { get; private set; } = new Dictionary<string, string>();

        public string RootPath { get; set; }

        public FileService()
        {
            defaultRootPath = Directory.GetCurrentDirectory();
            RootPath = defaultRootPath;
        }

        public void ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;

            if (request.HttpMethod == HttpMethod.Get.Method)
            {
                ProcessGetRequest(context);
            }
        }

        public void AddUrlRedirection(string url, string urlRedirection)
        {
            UrlRedirections.Add(url,
                urlRedirection.StartsWith("/")
                    ? urlRedirection
                    : "/" + urlRedirection);
        }

        private void ProcessGetRequest(HttpListenerContext context)
        {
            var url = ResolveUrl(context.Request.Url.AbsolutePath);
            var mimeType = MimeUtility.GetMimeMapping(url);
            
            if (!IsSubPath(RootPath, url))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                HttpServer.SetResponseText(context.Response, mimeType, "Access denied.");
                return;
            }

            byte[] responseBytes = null;

            if (File.Exists(url))
            {
                responseBytes = File.ReadAllBytes(url);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            HttpServer.SetResponseBytes(context.Response, mimeType, responseBytes);
        }

        private string ResolveUrl(string url)
        {
            if (UrlRedirections.TryGetValue(url, out var redirectedUrl))
            {
                url = redirectedUrl;
            }

            url = RootPath + url;

            return url;
        }

        private static bool IsSubPath(string rootPath, string subPath)
        {
            return Path.GetFullPath(subPath)
                .StartsWith(Path.GetFullPath(rootPath), StringComparison.OrdinalIgnoreCase);
        }
    }
}
