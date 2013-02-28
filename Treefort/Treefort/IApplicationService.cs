using System.Threading.Tasks;
using Treefort.Commanding;
namespace Treefort
{
    public interface IApplicationService
    {
        Task HandleAsync(ICommand command);
    }
}