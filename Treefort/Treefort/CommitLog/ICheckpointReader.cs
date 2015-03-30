using System;

namespace Treefort.CommitLog
{
    public interface ICheckpointReader : IDisposable
    {
        long Read();
    }
}