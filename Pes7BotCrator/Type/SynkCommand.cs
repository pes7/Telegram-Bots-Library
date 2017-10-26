using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Type
{
    public class SynkCommand
    {
        public List<string> CommandLine { get; set; }
        public dynamic doFunc { get; set; }
        public int SecondsSilents { get; set; }
        public SynkCommand(Action<Message, Bot> act, List<string> cm = null, int sec = 0)
        {
            SecondsSilents = sec;
            Incialize(act, cm);
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
}
