using System;
using Treefort.Common.Extensions;
using Treefort.Entity.Extensions;

namespace Treefort.Entity.Interceptors
{
    public class ForceUtcSaveInterceptor : IContextSaveInterceptor
    {
        public void BeforeSave(System.Data.Entity.Infrastructure.DbChangeTracker tracker)
        {
            tracker.Entries().ForEach(e => ObjectExtensions.ForceUtc(e.Entity));
        }

        public void AfterSave()
        {
            throw new NotImplementedException();
        }
    }
}
