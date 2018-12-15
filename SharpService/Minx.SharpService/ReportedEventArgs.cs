using System;

namespace Minx.SharpService
{
    public class ReportedEventArgs : EventArgs
    {
        public string Message { get; }

        public ReportLevel Level { get; }

        public ReportedEventArgs(string message, ReportLevel level)
        {
            Message = message;
            Level = level;
        }
    }
}
