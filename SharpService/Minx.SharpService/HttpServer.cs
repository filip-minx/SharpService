using System;
using System.Collections.Concurrent;
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

        public static void SetResponseData(HttpListenerResponse response, string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            response.Headers.Add(HttpResponseHeader.ContentType, "text/html; charset=utf-8");
            response.ContentLength64 = data.Length;

            var output = response.OutputStream;
            output.Write(bytes, 0, bytes.Length);
        }

        void IDisposable.Dispose()
        {
            ((IDisposable)listener).Dispose();
        }
    }
}
