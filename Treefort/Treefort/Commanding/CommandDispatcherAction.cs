using System;
using System.Threading.Tasks;

namespace Treefort.Commanding
{
    public class CommandDispatcherAction : ICommandDispatcher
    {
        private readonly Func<ICommand, Task> _action;

        public CommandDispatcherAction(Func<ICommand, Task> action)
        {
            _action = action;
        }

        public Task DispatchAsync(ICommand command)
        {
            return _action(command);
        }
    }
}