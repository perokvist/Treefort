using Treefort.Commanding;
using Treefort.Events;

namespace Treefort.Domain
{
    public interface IReceptor 
    {
        ICommand When(IEvent @event);
    }
}