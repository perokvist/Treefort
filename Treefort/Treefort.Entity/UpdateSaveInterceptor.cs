using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treefort.Entity
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
