using System.Data.Entity;
using System.Threading.Tasks;

namespace Treefort.EntityFramework.Eventing
{
    public interface IEventContext
    {
        IDbSet<EventStream> Streams { get; set; }
        Task<int> SaveChangesAsync();
    }
}