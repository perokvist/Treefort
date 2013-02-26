using Treefort.Commanding;
namespace Treefort
{
    public interface IApplicationService
    {
        void Handle(ICommand command);
    }
}