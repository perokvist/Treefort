using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Infrastructure;

namespace Treefort.Application
{
    public class ApplicationServer : IApplicationServer
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
                                    .HandleAsync(command);
                                });
        }
    }
}