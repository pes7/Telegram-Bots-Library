using Pes7BotCrator.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Pes7BotCrator.Modules
{
    class StatiscticOfGroup : ModuleInterface
    {
        public string Name { get; set; }
        public dynamic Modulle { get; set; }
        public Thread MainThread { get; set; }

        public void AbortThread()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}

