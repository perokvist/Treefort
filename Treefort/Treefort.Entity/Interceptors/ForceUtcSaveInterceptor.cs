using System;
using Treefort.Common.Extensions;
using Treefort.Entity.Extensions;

namespace Treefort.Entity.Interceptors
{
    public class ForceUtcSaveInterceptor : IContextSaveInterceptor
    {
        public void BeforeSave(System.Data.Entity.Infrastructure.DbChangeTracker tracker)
        {
            tracker.Entries().ForEach(e => e.Entity.ForceUtc());
        }

        public void AfterSave()
        {
            throw new NotImplementedException();
        }
    }
}
