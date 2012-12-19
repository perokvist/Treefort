using System;

namespace Treefort.Entity
{
    public interface ITimestamped
    {
        DateTime UpdatedAt { get; set; }
        DateTime CreatedAt { get; set; }
    }
}