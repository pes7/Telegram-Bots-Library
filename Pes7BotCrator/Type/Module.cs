using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    public class Module : IModule
    {
        public string Name { get; set; }
        public List<Thread> MainThreads { get; set; }
        public System.Type Type { get; set; }
        public IBot Parent { get; set; }
        public Module(string name, System.Type type, dynamic modulle = null)
        {
            Name = name;
            Type = type;
            MainThreads = new List<Thread>();
        }
        public void Start() { }
        public void AbortThread()
        {
            foreach (var th in MainThreads)
                th.Abort();
        }
    }
}
