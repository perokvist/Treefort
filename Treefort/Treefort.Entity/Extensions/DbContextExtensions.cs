using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treefort.Entity.Interceptors;

namespace Treefort.Entity.Extensions
{
    public static class DbContextExtensions
    {
        public static void MaterializeDateTimeAsUtc<T>(this DbContext context) 
        {
            ((IObjectContextAdapter)context).ObjectContext.ObjectMaterialized += 
                (sender,e) =>
                    {
                        if(e.Entity.GetType().IsInstanceOfType(typeof (T)))
                        {
                            ((T)e.Entity).ForceUtc();
                        }
                    };
        }

        public static void MaterializeDateTimesAsUtc(this DbContext context)
        {
            ((IObjectContextAdapter)context).ObjectContext.ObjectMaterialized +=
                (sender, e) => e.Entity.ForceUtc();
        }

        public static int Intercept(this DbContext context, IEnumerable<IContextSaveInterceptor> interceptors, Func<DbContext, int> interceptAction)
        {
            var interceptorsList = interceptors.ToList();
            interceptorsList.ForEach(i => i.BeforeSave(context.ChangeTracker));
            var result = interceptAction(context);
            interceptorsList.ForEach(i => i.AfterSave());
            return result;
        }
    }
}
