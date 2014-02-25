using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Common;
using Treefort.Domain;
using Treefort.Events;
using Treefort.Messaging;

namespace Treefort.Infrastructure
{
    public class ReceptorListener : IEventListener
    {
        private readonly IEnumerable<IReceptor> _receptors;
        private readonly ICommandBus _commandBus;

        public ReceptorListener(IEnumerable<IReceptor> receptors, ICommandBus commandBus)
        {
            _receptors = receptors;
            _commandBus = commandBus;
        }

        public async Task ReceiveAsync(IEnumerable<IEvent> events)
        {
            var receptorTasks = events.SelectMany(evt => _receptors.Select(r => r.HandleAsync(evt)));
            var commands = await Task.WhenAll(receptorTasks);
            foreach (var command in commands)
            {
                await _commandBus.SendAsync(new Envelope<ICommand>(command));
            }
        }
    }
}