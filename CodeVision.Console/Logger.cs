using System;
using System.Text;

namespace CodeVision.Console
{
    public class Logger : ILogger
    {
        public void Log(string message) 
        {
            Log(message, null);
        }

        public void Log(string message, Exception ex)
        {
            System.Console.WriteLine(message);
            if (ex != null)
            {
                var sb = new StringBuilder(message);
                AppendException(sb, ex);
                sb.Append(ex.StackTrace);
                System.Console.WriteLine("More details:" + sb.ToString());
            }            
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
