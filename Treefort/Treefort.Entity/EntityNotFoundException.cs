using System;

namespace Treefort.EntityFramework
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }
    }

    public class EntityNotFoundException<T> : EntityNotFoundException
    {
        public EntityNotFoundException(int id)
            : base(string.Format("Could not find {0} with id {1}", typeof(T), id))
        {
        }
    }

    public class EntityNotFoundException<TEntity, TIdentifier> : EntityNotFoundException
    {
        public EntityNotFoundException(TIdentifier id)
            : base(string.Format("Could not find {0} with id {1}", typeof(TEntity), id))
        {
        }
    }
}
