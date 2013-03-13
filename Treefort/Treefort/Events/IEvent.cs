namespace Treefort.Events
{
    public interface IEvent
    {
        string CorrelationId { get; }
    }
}