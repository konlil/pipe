using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Pipe
{
    public class Helper
    {
        public static void Log(string format, params Object [] objlist)
        {
            String str = String.Format(format, objlist);
            Debug.WriteLine(str);
        }
    }
}
