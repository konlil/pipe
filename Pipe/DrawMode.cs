using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipe
{
    public enum DrawMode
    {
        /// <summary>
        /// 使用DrawIndexedPrimitives绘制
        /// </summary>
        Indexed,

        /// <summary>
        /// 使用DrawPrimitives绘制
        /// </summary>
        Primitive,
    }
}
