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
            if (TryRedirectUrl(context.Response, context.Request.Url.AbsolutePath, out var targetUrl))
            {
                return;
            }

            targetUrl = ResolveUrlRoot(targetUrl);

            if (!ValidateAccess(context.Response, targetUrl))
            {
                return;
            }
            var dir = Directory.GetCurrentDirectory();
            var file = Path.Combine(dir, targetUrl);
            if (File.Exists(file))
            {
                var mimeType = MimeUtility.GetMimeMapping(targetUrl);
                var responseBytes = File.ReadAllBytes(targetUrl);

                HttpServer.SetResponseBytes(context.Response, mimeType, responseBytes);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }

        private bool ValidateAccess(HttpListenerResponse response, string url)
        {
            if (!IsSubPath(RootPath, url))
            {
                var mimeType = MimeUtility.GetMimeMapping(url);

                response.StatusCode = (int)HttpStatusCode.Forbidden;
                HttpServer.SetResponseText(response, mimeType, "Access denied.");
                return false;
            }

            return true;
        }

        private bool TryRedirectUrl(HttpListenerResponse response, string url, out string redirectedUrl)
        {
            if (UrlRedirections.TryGetValue(url, out redirectedUrl))
            {
                response.Redirect(redirectedUrl);
                response.Close();
                return true;
            }
            else
            {
                redirectedUrl = url;
                return false;
            }
        }

        private string ResolveUrlRoot(string url)
        {
            return RootPath + url;
        }

        private static bool IsSubPath(string rootPath, string subPath)
        {
            return Path.GetFullPath(subPath)
                .StartsWith(Path.GetFullPath(rootPath), StringComparison.OrdinalIgnoreCase);
        }
    }
}
