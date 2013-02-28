using System.Threading.Tasks;
using Treefort.Events;
namespace Treefort.Read
{
    public interface IProjection
    {
        Task WhenAsync(IEvent @event);
    }
}