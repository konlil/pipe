using System;
using System.Collections.Generic;
using System.Text;

namespace DGui
{
    static class GoldenRatio
    {
        static GoldenRatio()
        {
        }

        public static float LongFromShort(float value)
        {
            value *= (float)((1 + Math.Sqrt(5)) / 2);
            return value;
        }

        public static float ShortFromLong(float value)
        {
            value /= (float)((1 + Math.Sqrt(5)) / 2);
            return value;
        }
    }
}
