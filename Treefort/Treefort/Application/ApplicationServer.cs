using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
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
        private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _aggregateLocks;
        
        public ApplicationServer(
            Func<ICommand, Task> dispatcher,
            ILogger logger)
        {
            _dispatcher = dispatcher;
            _logger = logger;
            _aggregateLocks = new ConcurrentDictionary<Guid, SemaphoreSlim>();
        }

        public async Task DispatchAsync(ICommand command)
        {
            var @lock = _aggregateLocks.GetOrAdd(command.AggregateId, guid =>
            {
                _logger.Info(string.Format("Creating lock for {0}", guid));
                return new SemaphoreSlim(1);
            });
            
            await @lock.WaitAsync();
            try
            {
                _logger.Info(string.Format("Server Dispatches command {0} ({1})", command, command.CorrelationId));
                await _dispatcher(command);
            }
            catch(Exception ex)
            {
                _logger.Info(string.Format("Dispatch fail for {0} ({1})", command, command.CorrelationId));
                throw;
            }
            finally
            {
                _logger.Info(string.Format("Releasing lock for {0}", command.AggregateId));
                @lock.Release();
            }
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