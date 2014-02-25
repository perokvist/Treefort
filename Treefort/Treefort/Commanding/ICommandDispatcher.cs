using System.Threading.Tasks;

namespace Treefort.Commanding
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync(ICommand command);
    }
}