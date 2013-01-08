namespace Treefort.Commanding
{
    public class CommandHandlerFacade<T> : ICommandHandler
        where T : class
    {
        private readonly ICommandHandler<T> _commandHandler;

        public CommandHandlerFacade(ICommandHandler<T> handler)
        {
            _commandHandler = handler;
        }

        public void Handle(object command)
        {
            _commandHandler.Handle((T)command);
        }
    }
}
