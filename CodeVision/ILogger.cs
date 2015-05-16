using System;
using System.Security.Cryptography.X509Certificates;

namespace CodeVision
{
    public interface ILogger
    {
        void Log(string message);
        void Log(string message, Exception ex);
    }
}
