using System.Collections.Generic;
using System.Threading.Tasks;
using Treefort.Messaging;

namespace Treefort.Commanding
{
    public interface ICommandBus
    {
        Task SendAsync(Envelope<ICommand> command);
        void Send(IEnumerable<Envelope<ICommand>> commands);
    }
}
