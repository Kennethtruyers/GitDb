using System;
using System.Collections.Generic;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using GitDb.Core;
using GitDb.Core.Interfaces;
using GitDb.Server.Auth;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Hosting;
using Owin;

namespace GitDb.Server
{
    public class App
    {
        App(){}
        IContainer _container;
        string _url;
        IEnumerable<User> _users;
        ILogger _serverLog;

        public static App Create(string url, IGitDb repo, ILogger serverLog, IEnumerable<User> users)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(repo).As<IGitDb>().ExternallyOwned();
            builder.RegisterApiControllers(typeof(App).Assembly);

            var app = new App
            {
                _container = builder.Build(),
                _url = url,
                _users = users,
                _serverLog = serverLog
            };
            return app;
        }

        public IDisposable Start()
        {
            try
            {
                _serverLog.Info("Starting up git server");
                return WebApp.Start(new StartOptions(_url), Configuration);
            }
            catch (Exception ex)
            {
                _serverLog.Error(ex.Message);
                throw;
            }
        }
            

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            app.UseAutofacMiddleware(_container);
            var auth = new Authentication(_users);
            app.UseBasicAuthentication("gitdb", auth.ValidateUsernameAndPassword);
            config.MapHttpAttributeRoutes();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(_container);
            
            app.UseStageMarker(PipelineStage.MapHandler);
            app.UseWebApi(config);
        }
    }
}