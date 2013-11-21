using System;
using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Domain;

namespace Treefort.Application
{
    public interface ICommandRouter
    {
        Func<ICommand, Task> GetHandler<T>(T command) where T : ICommand;
    }
}