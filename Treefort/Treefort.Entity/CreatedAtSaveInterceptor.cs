using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treefort.Entity
{
    public class CreatedAtSaveInterceptor : IContextSaveInterceptor
    {
        public void BeforeSave(DbChangeTracker tracker)
        {
            foreach (var entry in tracker.Entries<ITimestamped>()
                .Where(x => x.State == EntityState.Added && x.Entity.CreatedAt == default(DateTime)))
            {
                var now = DateTime.UtcNow;
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
        }

        public void AfterSave()
        {
        }
    }
}
