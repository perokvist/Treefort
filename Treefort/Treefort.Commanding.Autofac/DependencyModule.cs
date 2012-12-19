using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Module = Autofac.Module;

namespace Treefort.Commanding.Autofac
{
    public class DependencyModule : Module
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
