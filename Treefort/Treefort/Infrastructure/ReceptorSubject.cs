using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Treefort.Commanding;
using Treefort.Common;
using Treefort.Domain;
using Treefort.Events;

namespace Treefort.Infrastructure
{
    public class ReceptorSubject : ISubject<IEvent, ICommand>
    {
        private readonly IEnumerable<IReceptor> _receptors;
        private readonly ILogger _logger;
        private readonly Subject<ICommand> _sub;

        public ReceptorSubject(IEnumerable<IReceptor> receptors, ILogger logger)
        {
            _sub = new Subject<ICommand>();
            _receptors = receptors;
            _logger = logger;
        }

        public void OnCompleted()
        {
            _sub.OnCompleted();
        }

        public void OnError(System.Exception error)
        {
            _sub.OnCompleted();
        }

        public void OnNext(IEvent value)
        {
            _receptors
                .Select(r => new { Command = r.When(value), Type = r.GetType()})
                .Where(info => info.Command != null)
                .ForEach(info =>
                             {
                                _logger.Info(string.Format("Receptor {0} Pushes {1} ({2})", info.Type, info.Command, info.Command.CorrelationId));
                                 _sub.OnNext(info.Command);
                             });
        }

        public System.IDisposable Subscribe(System.IObserver<ICommand> observer)
        {
            return _sub.Subscribe(observer);
        }
    }
}