using Pes7BotCrator.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Commands
{
    public class LogUlog : SynkCommand
    {
        public BotBase Parent { get; set; }
        public string HelloMessage { get; set; }
        public string BueMessage { get; set; }
        public LogUlog(BotBase bot, string helloMassage = null, string bueMessage = null, bool showCountOfPeople = false) : base(Act,descr: "hlbe") {
            Parent = bot;
            if (bueMessage != null) BueMessage = bueMessage;
            else BueMessage = "Bue, my dear friend...";
            if (helloMassage != null) HelloMessage = helloMassage;
            else HelloMessage = "Hello, my friend. Nice to meet u in our chanel.";
        }
        public static void Act(Update re, IBotBase Parent)
        { 
            LogUlog Th = null;
            try
            {
                Th = Parent.Commands.Find(fn => fn.Type == TypeOfCommand.Service && fn.Description == "hlbe") as LogUlog;
            }
            catch { }
            if(re.Message != null && Th != null)
            {
                if(re.Message.NewChatMember != null)
                {
                    Parent.Client.SendTextMessageAsync(re.Message.Chat.Id, Th.HelloMessage);
                }else if(re.Message.LeftChatMember != null)
                {
                    Parent.Client.SendTextMessageAsync(re.Message.Chat.Id, Th.BueMessage);
                }
            }
        }
    }
}
