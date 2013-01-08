using System;

namespace Treefort.Common
{
    public interface ITimestamped
    {
        DateTime UpdatedAt { get; set; }
        DateTime CreatedAt { get; set; }
    }
}