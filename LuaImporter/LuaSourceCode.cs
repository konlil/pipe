using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaImporter
{
    public class LuaSourceCode
    {
        private string source_code;

        public LuaSourceCode(string code)
        {
            this.source_code = code;
        }

        public string SourceCode
        {
            get { return source_code; }
        }
    }
}
