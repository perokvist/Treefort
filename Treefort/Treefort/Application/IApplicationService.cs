using System.Threading.Tasks;
using Treefort.Commanding;

namespace Treefort.Application
{
    public interface IApplicationService
    {
        Task HandleAsync(ICommand command);
    }
}