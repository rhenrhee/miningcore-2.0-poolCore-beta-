using System;

namespace CorePool.Time
{
    public interface IMasterClock
    {
        DateTime Now { get; }
    }
}
