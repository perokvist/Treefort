using System;
using Treefort.Common;
using Treefort.EntityFramework.Extensions;

namespace Treefort.EntityFramework.Interceptors
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
