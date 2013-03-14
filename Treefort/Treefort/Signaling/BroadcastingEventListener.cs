using System.Collections.Generic;
using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.Signaling
{
    public class BroadcastingEventListener : IEventListener
    {
        private readonly IEnumerable<IBroadcast> _broadcasts;

        public BroadcastingEventListener(IEnumerable<IBroadcast> broadcasts)
        {
            _broadcasts = broadcasts;
        }

        public async Task ReceiveAsync(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                foreach (var broadcast in _broadcasts)
                {
                    await broadcast.WhenAsync(@event);
                }
            }
        }
    }
}