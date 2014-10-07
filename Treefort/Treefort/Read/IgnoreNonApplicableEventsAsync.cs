using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.Read
{
    public class IgnoreNonApplicableEventsAsync
    {
        public Task HandleAsync(IEvent @event)
        {
            return Task.FromResult(0);
        } 
    }
}