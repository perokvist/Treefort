using System.Collections.Generic;
using System.Threading.Tasks;

namespace Treefort.Events
{
    public interface IEventListener
    {
        Task ReceiveAsync(IEnumerable<IEvent> events);
    }
}