using Treefort.Events;
namespace Treefort.Read
{
    public interface IProjection
    {
        void When(IEvent @event);
    }
}