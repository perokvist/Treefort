using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Treefort.Commanding;

namespace Treefort.Events
{
    public interface IEventPublisher : ISubject<IEvent>
    {
    }

}