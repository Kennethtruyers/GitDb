using System.Configuration;
using GitDb.Core.Interfaces;
using GitDb.Local;

namespace GitDb.Sample
{
    public class Git
    {
        static IGitDb _gitDb;
        static readonly object _lock = new object();
        public static IGitDb Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_gitDb == null)
                    {
                        var remote = ConfigurationManager.AppSettings["remote"];
                        var user = ConfigurationManager.AppSettings["user"];
                        var password = ConfigurationManager.AppSettings["password"];

                        lock (_lock)
                        {
                            if (_gitDb == null)
                                _gitDb = new LocalGitDb(ConfigurationManager.AppSettings["repoPath"], null, remote, user, user, password, false);
                        }
                    }
                     
                }
                return _gitDb;
            }
        }
    }
}