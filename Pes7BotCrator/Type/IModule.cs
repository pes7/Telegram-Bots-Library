using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    public interface IModule
    {
        /*
         * Сделать IBotBase и setIBotBase(IBotBase bot) для модулей, и вызывать при инициализации бота
         */
        string Name { get; set; }
        IBotBase Parent { get; set; }
        System.Type Type { get; set; }
        Thread MainThread { get; set; }
        void Start();
        void AbortThread();
    }
}
