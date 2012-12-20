using System.Data.Entity.Infrastructure;

namespace Treefort.Entity.Interceptors
{
    public interface IContextSaveInterceptor
    {
        void BeforeSave(DbChangeTracker tracker);

        void AfterSave();
    }
}
