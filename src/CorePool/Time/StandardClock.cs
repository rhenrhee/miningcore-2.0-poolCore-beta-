using System;
using System.Collections.Generic;
using System.Text;

namespace CorePool.Time
{
    public class StandardClock : IMasterClock
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
