using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treefort.Entity
{
    public interface IContextSaveInterceptor
    {
        void BeforeSave(DbChangeTracker tracker);

        void AfterSave();
    }
}
