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
        public string AdditionEndText { get; set;}
        public enum TypeOf { OnlyMessage, MessageWithChanellName, MessageWithNameAndChannelName}
        public TypeOf TypeOfMessage { get; set; }
        public LogUlog(BotBase bot, TypeOf type = TypeOf.OnlyMessage, string helloMassage = null, string bueMessage = null, bool showCountOfPeople = false) : base(Act,descr: "hlbe") {
            Parent = bot;
            TypeOfMessage = type;
            if (bueMessage != null) BueMessage = bueMessage;
            else BueMessage = "Bue, my dear friend...";
            if (helloMassage != null) HelloMessage = helloMassage;
            else HelloMessage = "Hello, my friend. Nice to meet u in our chanel.";
        }
        public static void Act(Update re, IBot Parent)
        {
            LogUlog Th = null;
            try
            {
                Th = Parent.SynkCommands.Find(fn => fn.Type == TypeOfCommand.Service && fn.Description == "hlbe") as LogUlog;
            }
            catch { }
            if (re.Message != null && Th != null)
            {
                switch (Th.TypeOfMessage)
                {
                    case TypeOf.OnlyMessage:
                        if (re.Message.NewChatMember != null)
                        {
                            Parent.Client.SendTextMessageAsync(re.Message.Chat.Id, Th.HelloMessage);
                        }
                        else if (re.Message.LeftChatMember != null)
                        {
                            Parent.Client.SendTextMessageAsync(re.Message.Chat.Id, Th.BueMessage);
                        }
                        break;
                    case TypeOf.MessageWithChanellName:
                        if (re.Message.NewChatMember != null)
                        {
                            var msg = String.Format($"{Th.HelloMessage}",re.Message.Chat.FirstName);
                            Parent.Client.SendTextMessageAsync(re.Message.Chat.Id, msg);
                        }
                        else if (re.Message.LeftChatMember != null)
                        {
                            Parent.Client.SendTextMessageAsync(re.Message.Chat.Id, Th.BueMessage);
                        }
                        break;
                    case TypeOf.MessageWithNameAndChannelName:
                        if (re.Message.NewChatMember != null)
                        {
                            var msg = String.Format($"{Th.HelloMessage}", re.Message.NewChatMember.Username, re.Message.Chat.Title);
                            Parent.Client.SendTextMessageAsync(re.Message.Chat.Id, msg);
                        }
                        else if (re.Message.LeftChatMember != null)
                        {
                            Parent.Client.SendTextMessageAsync(re.Message.Chat.Id, Th.BueMessage);
                        }
                        break;
                }
            }
        }
    }
}
