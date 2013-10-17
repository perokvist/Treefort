using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Common;
using Treefort.Infrastructure;
using Treefort.Messaging;

namespace Treefort.Application
{
    public class ApplicationServer : IApplicationServer, ICommandBus
    {
        private readonly ICommandRouter _commandRouter;
        private readonly ILogger _logger;

        public ApplicationServer(
            ICommandRouter commandRouter,
            ILogger logger)
        {
            _commandRouter = commandRouter;
            _logger = logger;
        }

        public Task DispatchAsync(ICommand command)
        {
            _logger.Info(string.Format("Server Dispatches command {0} ({1})", command, command.CorrelationId));
            //TODO this need to be awaited to run ?
            return Task.Run(() =>
                                {
                                    _commandRouter
                                    .GetServiceFor(command)
                                    .HandleAsync(command); //Warning concurrency problems ahead
                                });
        }

        public Task SendAsync(Envelope<ICommand> command)
        {
            return DispatchAsync(command.Body);
        }

        public void Send(IEnumerable<Envelope<ICommand>> commands)
        {
            commands.ForEach(cmd => DispatchAsync(cmd.Body));
        }
    }
    
}