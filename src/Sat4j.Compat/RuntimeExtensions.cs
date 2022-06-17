using System;
using System.Collections.Generic;
using System.Text;

namespace Sharpen
{
    public static class RuntimeExtensions
    {

        public static int FreeMemory(this Runtime self)
        {
            return int.MaxValue;
        }

        public static int MaxMemory(this Runtime self)
        {
            return int.MaxValue;
        }
        public static int TotalMemory(this Runtime self)
        {
            return int.MaxValue;
        }
        public static int AvailableProcessors(this Runtime self)
        {
            return Environment.ProcessorCount;
        }
    }
}
