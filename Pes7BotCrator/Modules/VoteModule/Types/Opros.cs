using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Pes7BotCrator.Modules.Types
{
    [Serializable]
    public class Opros
    {
        public string About { get; set; }
        public string First { get; set; }
        public string Second { get; set; }
        public int Id { get; set; }
        public string Query { get; set; }
        public VoteMessage oprosThis { get; set; }
        public Opros(string question, int id, Message it = null)
        {
            
            About = question;
            Id = id;
            Query = null;
            First = "👍";
            Second = "👎";
            if(it!=null)
                loadMessage(it);
        }
        public Opros(string question, int id, string fristAnswer, string secondAnswer,  Message it = null)
        {
            About = question;
            Id = id;
            Query = null;
            First = fristAnswer;
            Second = secondAnswer;
            if(it!=null)
                loadMessage(it);
        }

        private void loadMessage(Message it)
        {
            oprosThis = new VoteMessage
            {
                Chat = it.Chat,
                Date = it.Date,
                From = it.From,
                MessageId = it.MessageId,
                Text = it.Text,
                Type = it.Type
            };
        }
    }
}
