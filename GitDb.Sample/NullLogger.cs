using GitDb.Core;

namespace GitDb.Sample
{
    public class NullLogger : ILogger
    {
        public void Debug(string format, params object[] args)
        {
            
        }

        public void Error(string format, params object[] args)
        {
        }

        public void Info(string format, params object[] args)
        {
        }

        public void Trace(string format, params object[] args)
        {
        }

        public void Warn(string format, params object[] args)
        {
        }
    }
}