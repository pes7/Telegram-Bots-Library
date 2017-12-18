using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Type
{
    class TimedSynkCommand : ISynkCommand
    {
        /*
         * Не оправданый функционал, нужно сделать TimeRelay на похідних от Message 
         */
        public TypeOfCommand Type { get; set; }
        public List<string> CommandLine { get; set; }
        public TimeReleyParams Params { get; set; }
        public Thread MainTimeThread { get; set; }
        public Delegate doFunc { get; set; }
        public string Description { get; set; }
        public IBotBase Parent { get; set; }
        
        private int Time { get; set; }
        private bool NeedToLive { get; set; }

        public TimedSynkCommand(IBotBase parent, Action<IBotBase> act, TimeReleyParams type, List<string> cm = null, string endMessage = null, string descr = null)
        {
            Parent = parent;
            Description = descr;
            Type = TypeOfCommand.TimeReley;
            Params = type;
            Incialize(act, cm);
        }

        public void StartTimeReley()
        {
            NeedToLive = true;
            MainTimeThread = new Thread(() =>
            {
                while (NeedToLive)
                {
                    TimeSynk();
                    Time += 1;
                }
                
            });
            MainTimeThread.Start();
        }

        private void TimeSynk()
        {
            if (Time >= Params.Time)
            {
                switch (Params.Type)
                {
                    case TypeOfTimeReley.Delayed:
                        doFunc.DynamicInvoke(Parent);
                        NeedToLive = false;
                        break;
                    case TypeOfTimeReley.Repeat:
                        doFunc.DynamicInvoke(Parent);
                        Time = 0;
                        break;
                }
            }
        }

        private void Incialize(dynamic ds, List<string> cm)
        {
            if (cm == null)
                CommandLine = new List<string>();
            else CommandLine = cm;
            doFunc = ds;
            CommandLine = cm;
        }

    }

    public struct TimeReleyParams
    {
        public int Time { get; set; }
        public TypeOfTimeReley Type { get; set; }
    }

    public enum TypeOfTimeReley {
        Delayed,
        Repeat
    }
}
