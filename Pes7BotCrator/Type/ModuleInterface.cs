using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    public interface ModuleInterface
    {
        string Name { get; set; }
        dynamic Modulle { get; set; }
        System.Type Type { get; set; }
        Thread MainThread { get; set; }
        void Start();
        void AbortThread();
    }
}
