using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Treefort.Application;
using Treefort.Commanding;

namespace Treefort.Common
{
    public static class DispatcherExtensions
    {

        public static void Register(this Dispatcher<ICommand, Task> dispatcher, IApplicationService applicationService, string convention = "When")
        {
            var methodName = convention;
            var infos = applicationService.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m => m.Name == methodName)
            .Where(m => m.GetParameters().Length == 1)
            .Where(m => m.ReturnType != typeof(Task<ICommand>));


            infos.ForEach(methodInfo =>
                Register(dispatcher,methodInfo.GetParameters().First().ParameterType, applicationService));
        }

        public static void Register(Dispatcher<ICommand, Task> dispatcher, Type type, IApplicationService applicationSerivce)
        {
             typeof(DispatcherExtensions).GetMethod("RegisterApplicationService").MakeGenericMethod(type).Invoke(null, new object[] { dispatcher, applicationSerivce});
        }

        public static void RegisterApplicationService<T>(this Dispatcher<ICommand, Task> dispatcher, IApplicationService applicationService)
            where T :ICommand
        {
            dispatcher.Register<T>(arg => applicationService.HandleAsync(arg));
        }

    }
}