using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using LuaImporter;

namespace Pipe
{
    class LuaReader : ContentTypeReader<LuaBinary>
    {
        protected override LuaBinary Read(ContentReader input, LuaBinary existingInstance)
        {
            int code_size = input.ReadInt32();
            byte[] binary_code = input.ReadBytes(code_size);

            return new LuaBinary(binary_code);
        }
    }
}
