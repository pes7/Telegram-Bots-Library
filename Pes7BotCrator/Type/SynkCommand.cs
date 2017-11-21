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
        private Action<CallbackQuery, Bot, Update> act;
        private List<string> list;

        public enum TypeOfCommand {
            Standart,
            Query,
            InlineQuery
        }
        public TypeOfCommand Type { get; set; }
        public List<string> CommandLine { get; set; }
        public dynamic doFunc { get; set; }
        public int SecondsSilents { get; set; }
        public string EndMessage { get; set; }
        public SynkCommand(Action<Message, Bot> act, List<string> cm = null,int sec = 0, string endMessage = null)
        {
            SecondsSilents = sec;
            EndMessage = endMessage;
            Type = TypeOfCommand.Standart;
            Incialize(act, cm);
        }

        public SynkCommand(Action<Message, Bot, Update> act, List<string> cm = null, int sec = 0, string endMessage = null)
        {
            SecondsSilents = sec;
            EndMessage = endMessage;
            Type = TypeOfCommand.Query;
            Incialize(act, cm);
        }

        public SynkCommand(Action<InlineQuery, Bot, Update> act, List<string> cm = null, int sec = 0, string endMessage = null)
        {
            SecondsSilents = sec;
            EndMessage = endMessage;
            Type = TypeOfCommand.InlineQuery;
            Incialize(act, cm);
        }

        public SynkCommand(Action<CallbackQuery, Bot, Update> act, List<string> cm = null, int sec = 0, string endMessage = null)
        {
            SecondsSilents = sec;
            EndMessage = endMessage;
            Type = TypeOfCommand.Query;
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
