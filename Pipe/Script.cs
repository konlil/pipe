using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;

namespace Pipe
{
    public class Script
    {
        public static Lua lua_vm = null;

        public Script()
        {
            lua_vm = new Lua();
        }

        public void DoFile(string file_name)
        {
            lua_vm.DoFile(file_name);
        }
    }
}
