using System;
using System.IO;
using System.Text;

namespace Minx.SharpService
{
    class ConsoleInterceptor : IDisposable
    {
        private static volatile bool created = false;
        private static object lockObject = new object();

        private TextWriter defaultOutput;
        private StringBuilder outputText;
        private StringWriter writer;

        public string OutputText => outputText.ToString();

        public static ConsoleInterceptor Get()
        {
            lock (lockObject)
            {
                if (created)
                {
                    throw new InvalidOperationException("Tried to create another interceptor before an existing interceptor was disposed.");
                }
            }

            return new ConsoleInterceptor();
        }

        private ConsoleInterceptor()
        {
            defaultOutput = Console.Out;

            outputText = new StringBuilder();
            writer = new StringWriter(outputText);

            Console.SetOut(writer);
        }

        public void Dispose()
        {
            lock (lockObject)
            {
                created = false;

                Console.SetOut(defaultOutput);
            }

            writer.Dispose();
        }
    }
}
