using System.Threading.Tasks;
using Treefort.Commanding;

namespace Treefort.Application
{
    public interface IApplicationServer
    {
        Task DispatchAsync(ICommand command);
    }
}