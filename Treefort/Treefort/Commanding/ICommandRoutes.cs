using System;

namespace Treefort.Commanding
{
    public interface ICommandRoutes
    {
        bool HasRoute<TCommand>(TCommand command) 
            where TCommand : ICommand;

        Func<dynamic> AggregateFactory<TCommand>(TCommand command)
            where TCommand : ICommand;
    }
}