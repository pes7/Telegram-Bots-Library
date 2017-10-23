using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    public class Module
    {
        public string Name { get; set; }
        public dynamic Modulle { get; set; }
        public Module(string name, dynamic mod)
        {
            Name = name;
            Modulle = mod;
        }
    }
}
