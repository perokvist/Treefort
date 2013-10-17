using Treefort.Commanding;
using Treefort.Domain;

namespace Treefort.Application
{
    public interface ICommandRouter
    {
        IApplicationService GetServiceFor<T>(T command) where T : ICommand;
    }
}