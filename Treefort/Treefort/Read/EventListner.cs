using System.Collections.Generic;
using Treefort.Common.Extensions;
using Treefort.Events;

namespace Treefort.Read
{
    public class EventListner : IEventListner
    {
        private readonly IEnumerable<IProjection> _listerners;

        public EventListner(IEnumerable<IProjection> listerners)
        {
            _listerners = listerners;
        }

        public void Receive(IEnumerable<IEvent> events)
        {
            _listerners.ForEach(l => events.ForEach(l.When));
        }
    }
}