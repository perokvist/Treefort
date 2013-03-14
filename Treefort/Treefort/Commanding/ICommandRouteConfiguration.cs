namespace Treefort.Commanding
{
    public interface ICommandRouteConfiguration
    {
        void Add<TCommand, TAggregate>()
            where TCommand : ICommand
            where TAggregate : new();
    }
}