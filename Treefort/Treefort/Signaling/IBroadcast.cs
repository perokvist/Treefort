using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.Signaling
{
    public interface IBroadcast
    {
        Task WhenAsync(IEvent @event);
    }
}
