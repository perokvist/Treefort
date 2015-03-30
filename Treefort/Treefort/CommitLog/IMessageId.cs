using System;

namespace Treefort.CommitLog
{
    public interface IMessageId : IComparable<IMessageId>, IComparable
    {
        DateTime GetTimeUtc();
        long GetOffset();
        int GetRand();
        bool IsEmpty();
        string ToString();
        byte[] GetBytes();
        int GetHashCode();
    }
}
