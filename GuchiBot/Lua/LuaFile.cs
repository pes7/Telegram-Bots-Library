using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAble
{
    public class LuaFile
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public LuaFile(string path, string name, string version = "v0.0.0.0")
        {
            Path = path;
            Name = name;
            Version = version;
        }
    }
}
