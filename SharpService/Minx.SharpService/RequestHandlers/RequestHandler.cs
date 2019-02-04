using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Minx.SharpService.RequestHandlers
{
    public abstract class RequestHandler
    {
        public abstract string Route { get; }
        public abstract HttpMethod HttpMethod { get; }

        public Regex UrlRegex { get; }

        private Dictionary<string, string> arguments;

        internal void InternalProcessRequest(HttpListenerContext context, ScriptEnvironment script)
        {
            arguments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var match = UrlRegex.Match(context.Request.Url.AbsolutePath);
            var groupNames = UrlRegex.GetGroupNames();

            foreach (var groupName in groupNames)
            {
                var group = match.Groups[groupName];

                arguments[groupName] = group.Value;
            }

            ProcessRequest(context, script);
        }

        protected abstract void ProcessRequest(HttpListenerContext context, ScriptEnvironment script);

        protected T GetArgument<T>(string name)
        {
            return (T)Convert.ChangeType(arguments[name], typeof(T));
        }

        protected object GetArgument(string name)
        {
            return Convert.ChangeType(arguments[name], typeof(string));
        }

        public RequestHandler()
        {
            UrlRegex = CreateRegex(Route);
        }

        private Regex CreateRegex(string urlPattern)
        {
            var regexPattern = Regex.Replace(urlPattern, @"{(\w+)}", @"(?<$1>\w+)");

            return new Regex($"^{regexPattern}$", RegexOptions.IgnoreCase);
        }
    }
}
