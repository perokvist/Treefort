using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Common;
using Treefort.Infrastructure;
using Treefort.Messaging;

namespace Treefort.Application
{
    public class ApplicationServer : ICommandDispatcher, ICommandBus
    {
        private readonly Func<ICommand, Task> _dispatcher;
        private readonly ILogger _logger;
        
        public ApplicationServer(
            Func<ICommand, Task> dispatcher,
            ILogger logger)
        {
            _dispatcher = dispatcher;
            _logger = logger;
        }

        public Task DispatchAsync(ICommand command)
        {
            _logger.Info(string.Format("Server Dispatches command {0} ({1})", command, command.CorrelationId));
            //TODO this need to be awaited to run ?
            return _dispatcher(command);
        }

        Task ICommandBus.SendAsync(Envelope<ICommand> command)
        {
            return DispatchAsync(command.Body);
        }
        
       void ICommandBus.Send(IEnumerable<Envelope<ICommand>> commands)
        {
            commands.ForEach(cmd => DispatchAsync(cmd.Body));
        }
    }

}