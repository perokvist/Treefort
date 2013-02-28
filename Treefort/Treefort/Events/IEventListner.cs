using System.Collections.Generic;
using System.Threading.Tasks;

namespace Treefort.Events
{
    public interface IEventListner
    {
        Task ReceiveAsync(IEnumerable<IEvent> events);
    }
}