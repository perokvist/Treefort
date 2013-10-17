using System.Collections.Generic;
using System.Threading.Tasks;
using Treefort.Messaging;

namespace Treefort.Events
{
    public interface IEventBus
    {
        Task PublishAsync(Envelope<IEvent> @event);
        void Publish(IEnumerable<Envelope<IEvent>> events); 
    }
}