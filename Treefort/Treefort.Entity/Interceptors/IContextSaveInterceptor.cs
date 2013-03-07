using System.Data.Entity.Infrastructure;

namespace Treefort.EntityFramework.Interceptors
{
    public interface IContextSaveInterceptor
    {
        void BeforeSave(DbChangeTracker tracker);

        void AfterSave();
    }
}
