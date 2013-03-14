using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Treefort.Commanding;

namespace Treefort.Infrastructure
{
    public class CommandRouteConfiguration : ICommandRoutes, ICommandRouteConfiguration
    {
        private readonly IDictionary<Type, Func<dynamic>> _commandMap;

        public CommandRouteConfiguration()
        {
            _commandMap = new ConcurrentDictionary<Type, Func<dynamic>>();
        }
        
        void ICommandRouteConfiguration.Add<TCommand, TAggregate>()
        {
            _commandMap.Add(typeof(TCommand), () => new TAggregate());            
        }

        bool ICommandRoutes.HasRoute<TCommand>(TCommand command)
        {
            return GetCommandTypes(command).Any(t => _commandMap.ContainsKey(t));
        }

        Func<dynamic> ICommandRoutes.AggregateFactory<TCommand>(TCommand command)
        {
            return GetCommandTypes(command)
                .Where(t => _commandMap.ContainsKey(t))
                .Select(t => _commandMap[t]).FirstOrDefault();
        }

        private static IEnumerable<Type> GetCommandTypes(ICommand command)
        {
            var commandType = command.GetType();
            var types = new List<Type> { commandType };
            types.AddRange(commandType.GetInterfaces());
            types.Add(commandType.BaseType);
            return types;
        } 
    }
}