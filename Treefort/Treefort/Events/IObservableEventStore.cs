using System;
namespace Treefort.Events
{
    public interface IObservableEventStore : IEventStore, IObservable<IEvent>
    {
         
    }
}