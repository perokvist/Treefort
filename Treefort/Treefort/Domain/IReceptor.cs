using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Events;

namespace Treefort.Domain
{
    public interface IReceptor 
    {
        Task<ICommand> MapAsync(IEvent @event);
    }
}