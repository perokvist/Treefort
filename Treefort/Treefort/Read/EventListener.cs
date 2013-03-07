using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.Read
{
    public class EventListener : IEventListener
    {
        private readonly IEnumerable<IProjection> _listerners;

        public EventListener(IEnumerable<IProjection> listerners)
        {
            _listerners = listerners;
        }

        public Task ReceiveAsync(IEnumerable<IEvent> events)
        {
            return Task.WhenAll(events.SelectMany(e => _listerners.Select(l => l.WhenAsync(e))));
        }
    }
}