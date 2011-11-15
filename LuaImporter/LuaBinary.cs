using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaImporter
{
    public class LuaBinary
    {
        private byte[] compiled_script;
        public byte[] CompiledScript
        {
            get { return (byte[])compiled_script.Clone(); }
        }

        public LuaBinary(byte[] compiled_script)
        {
            this.compiled_script = compiled_script;
        }
    }
}
