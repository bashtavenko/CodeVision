using System;
using System.Text;

namespace CodeVision.Console
{
    public class Logger : ILogger
    {
        public void Log(string message)
        {
            System.Console.WriteLine(message);
        }

        public void Log(string message, Exception ex)
        {
            var sb = new StringBuilder(message);
            AppendException(sb, ex);
            sb.Append(ex.StackTrace);
        }

        private static void AppendException(StringBuilder sb, Exception ex)
        {
            if (ex == null)
            {
                return;
            }
            sb.Append(ex.Message);
            AppendException(sb, ex.InnerException);
        }
    }
}
