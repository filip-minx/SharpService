using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Minx.SharpService
{
    public class HttpServer : IDisposable
    {
        private HttpListener listener;

        private ConcurrentQueue<HttpListenerContext> pendingRequests = new ConcurrentQueue<HttpListenerContext>();

        public HttpServer(string[] prefixes)
        {
            listener = new HttpListener();
            
            foreach (var prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }
        }

        public HttpServer(string prefix)
            : this(new[] { prefix })
        { }

        public void Start()
        {
            listener.Start();

            Task.Run((Action)Receive);
        }

        private void Receive()
        {
            while (listener.IsListening)
            {
                try
                {
                    var context = listener.GetContext();

                    pendingRequests.Enqueue(context);
                }
                catch (HttpListenerException)
                {
                    // The listener was stopped.
                }
            }
        }

        public void ProcessRequests(Action<HttpListenerContext> requestHandler)
        {
            if (listener.IsListening)
            {
                while (pendingRequests.TryDequeue(out var context))
                {
                    requestHandler(context);
                }
            }
        }

        public static void SetResponseText(HttpListenerResponse response, string responseType, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);

            SetResponseBytes(response, responseType, bytes);
        }

        public static void SetResponseBytes(HttpListenerResponse response, string responseType, byte[] bytes)
        {
            response.Headers.Add(HttpResponseHeader.ContentType, responseType + "; charset=utf-8");

            if (bytes != null)
            {
                response.ContentLength64 = bytes.LongLength;
                response.OutputStream.Write(bytes, 0, bytes.Length);
            }
        }

        public static string ReadRequestText(HttpListenerRequest request)
        {
            using (var reader = new StreamReader(request.InputStream))
            {
                return reader.ReadToEnd();
            }
        }

        void IDisposable.Dispose()
        {
            ((IDisposable)listener).Dispose();
        }
    }
}
