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
        //private Action<CallbackQuery, BotBase, Update> act;
        //private List<string> list;

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
        public string Description { get; set; }
        public SynkCommand(Action<Message, IBotBase, List<ArgC>> act, List<string> cm = null,int sec = 0, string endMessage = null, string descr = null)
        {
            SecondsSilents = sec;
            EndMessage = endMessage;
            Description = descr;
            Type = TypeOfCommand.Standart;
            Incialize(act, cm);
        }

        /*
        public SynkCommand(Action<Message, BotInteface, Update> act, List<string> cm = null, int sec = 0, string endMessage = null)
        {
            SecondsSilents = sec;
            EndMessage = endMessage;
            Type = TypeOfCommand.Query;
            Incialize(act, cm);
        }
        */

        public SynkCommand(Action<InlineQuery, IBotBase> act, List<string> cm = null, int sec = 0, string endMessage = null, string descr = null)
        {
            SecondsSilents = sec;
            EndMessage = endMessage;
            Description = descr;
            Type = TypeOfCommand.InlineQuery;
            Incialize(act, cm);
        }

        public SynkCommand(Action<CallbackQuery, IBotBase> act, List<string> cm = null, int sec = 0, string endMessage = null, string descr = null)
        {
            SecondsSilents = sec;
            EndMessage = endMessage;
            Description = descr;
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
