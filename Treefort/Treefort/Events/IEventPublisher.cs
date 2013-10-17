using System;
using System.Reactive.Subjects;
using Treefort.Commanding;

namespace Treefort.Events
{
    public interface IEventPublisher : ISubject<IEvent, ICommand>
    {
    }
}