using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    public class ArgC
    {
        public string Name { get; set; }
        public string Arg { get; set; }
        public ArgC(string name = null, string arg = null)
        {
            Name = name;
            Arg = arg;
        }
    }
}
