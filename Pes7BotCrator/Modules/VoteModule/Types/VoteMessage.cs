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
    public class VoteMessage
    {
        public int MessageId { get; set; }
        public User From { get; set; }
        public DateTime Date { get; set; }
        public Chat Chat { get; set; }
        public string Text { get; set; }
        public MessageType Type { get; set; }
        public VoteMessage(){}
    }
}
