using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Treefort.Application;
using Treefort.Commanding;
using Treefort.Common;
using Treefort.Domain;

namespace Treefort.Infrastructure
{
    public class InMemoryCommandRouter : ICommandRouter
    {
        private readonly ILogger _logger;
        private readonly IDictionary<Type, Func<ICommand, Task>> _routes = new Dictionary<Type, Func<ICommand, Task>>();

        public InMemoryCommandRouter(ILogger logger)
        {
            _logger = logger;
        }

        public void RegisterHandler(IApplicationService applicationService)
        {
                var methodName = "When"; //TODO configure conventions
                var infos = applicationService.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == methodName)
                .Where(m => m.GetParameters().Length == 1);

            infos.ForEach(methodInfo => 
                _routes.Add(methodInfo.GetParameters().First().ParameterType, applicationService.HandleAsync));
        }
        
        public Func<ICommand, Task> GetHandler<T>(T command) where T : ICommand
        {
            //TODO handle ex, fix marker interface support
            var appService = _routes[command.GetType()];
            _logger.Info(string.Format("Routing {0} to {1}", command, appService));
            return appService;
        }
    }
}