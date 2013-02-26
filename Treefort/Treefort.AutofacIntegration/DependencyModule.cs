using Autofac;
using System.Reflection;
using Treefort.Commanding;

namespace Treefort.AutofacIntegration
{
    public class DependencyModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .PreserveExistingDefaults()
                .AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(CommandHandlerFacade<>));
        }
    }
}
