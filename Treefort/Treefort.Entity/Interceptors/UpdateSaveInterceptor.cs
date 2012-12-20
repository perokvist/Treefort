using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Treefort.Common;

namespace Treefort.Entity.Interceptors
{
    public class UpdatedAtSaveInterceptor : IContextSaveInterceptor
    {
        public void BeforeSave(DbChangeTracker tracker)
        {
            foreach (var entry in tracker.Entries<ITimestamped>()
                .Where(x => x.State == EntityState.Modified))
                entry.Entity.UpdatedAt = DateTime.Now.ToUniversalTime();
        }

        public void AfterSave()
        {
        }
    }
}
