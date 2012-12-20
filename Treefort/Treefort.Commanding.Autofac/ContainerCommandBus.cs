using System;
using System.Collections.Generic;
using Autofac;
using Treefort.Common.Extensions;

namespace Treefort.Commanding.Autofac
{
    public class ContainerCommandBus : ICommandBus
    {
        private readonly IComponentContext _context;

        public ContainerCommandBus(IComponentContext context)
        {
            _context = context;
        }

        public void Send(Envelope<ICommand> command)
        {
            var commandHandlerType = GetCommandHandlerType(command);
            var handler = _context.Resolve(commandHandlerType) as ICommandHandler;
            if (handler == null) throw new InvalidOperationException("Handler not found for " + command.ToString());
            handler.Handle(command.Body);
        }

        public void Send(IEnumerable<Envelope<ICommand>> commands)
        {
            commands.ForEach(Send);
        }

        private static Type GetCommandHandlerType(Envelope<ICommand> command)
        {
            var commandType = command.Body.GetType();
            return typeof(CommandHandlerFacade<>).MakeGenericType(commandType);
        }

    }
}
