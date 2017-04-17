using System;
using System.Collections.Generic;
using System.Configuration;
using GitDb.Core;
using GitDb.Local;
using GitDb.Server.Auth;

namespace GitDb.Server
{
    class Program
    {
        public static void Main(string[] args)
        {
            var url = ConfigurationManager.AppSettings["server.url"];
            var gitRepoPath = ConfigurationManager.AppSettings["git.repository.path"];
            var remoteUrl = ConfigurationManager.AppSettings["remote.url"];
            var userName = ConfigurationManager.AppSettings["remote.user.name"];
            var userEmail = ConfigurationManager.AppSettings["remote.user.email"];
            var password = ConfigurationManager.AppSettings["remote.user.password"];
            var logger = new Logger();
            var app = App.Create(url, new LocalGitDb(gitRepoPath, logger, remoteUrl, userName, userEmail, password), logger, new List<User>
            {
                new User{ UserName = "GitAdmin", Password = "LCz8ovCZiddM4FGH1T3V", Roles = new [] { "admin","read","write" }},
                new User{ UserName = "GitReader",Password = "IUFYTF2oPuK04OfnVl5H",Roles = new [] { "read" }},
                new User{ UserName = "GitWriter", Password = "4yzvqhPkHPZbSbuGN4aQ6b",Roles = new [] { "write" }}
            });
            using (app.Start())
            {
                logger.Info($"Server started on {url}, with repo at {gitRepoPath}");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            };
        }
    }

    class Logger : ILogger
    {
        public void Trace(string format, params object[] args) => Console.WriteLine("TRACE: " + format, args);
        public void Info(string format, params object[] args) => Console.WriteLine("INFO: " + format, args);
        public void Debug(string format, params object[] args) => Console.WriteLine("DEBUG: " + format, args);
        public void Warn(string format, params object[] args) => Console.WriteLine("WARB: " + format, args);
        public void Error(string format, params object[] args) => Console.WriteLine("ERROR: " + format, args);
    }
}
