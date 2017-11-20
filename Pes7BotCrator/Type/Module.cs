using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    public class Module : ModuleInterface
    {
        public string Name { get; set; }
        public dynamic Modulle { get; set; }
        public Thread MainThread { get; set; }
        public Module(string name)
        {
            Name = name;
        }
        public void Start() { }
        public void AbortThread()
        {
            if (MainThread != null)
                MainThread.Abort();
        }
    }
}
