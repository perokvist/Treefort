using System.Collections.Generic;

namespace Treefort.Events
{
    public interface IEventListner
    {
        void Receive(IEnumerable<IEvent> events);
    }
}