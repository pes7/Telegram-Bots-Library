using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Modules.Types;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Modules
{
    public class TRM : Module
    {
        public List<TimeRelayMessage> TimeRelayMessages { get; set; }
        public SayAfterMe _SayAfterMe { get; set; }
        // TimeRelayMessages
        public TRM() : base("TRM", typeof(TRM)) {
            TimeRelayMessages = new List<TimeRelayMessage>();
            _SayAfterMe = new SayAfterMe();
            var Th = new Thread(() =>
            {
                while (true)
                {
                    foreach(var ms in TimeRelayMessages.FindAll(fn => !fn.IsAlife))
                    {
                        TimeRelayMessages.Remove(ms);
                    }
                    Thread.Sleep(30000);
                }
            });
            Th.Start();
            MainThreads.Add(Th);
        }

        public async Task<Message> SendTimeRelayMessageAsynkAsync(long id, string text, int time, int relayToMessageId = 0)
        {
            var ms = await Parent.Client.SendTextMessageAsync(id, text, replyToMessageId: relayToMessageId);
            TimeRelayMessages.Add(new TimeRelayMessage(Parent, ms, TypeOfTimeRealyMessage.AutoDel, time));
            return ms;
        }

        public async Task<Message> SendTimeRelayMessageAsynkAsync(string id, string text, int time, int relayToMessageId = 0)
        {
            var ms = await Parent.Client.SendTextMessageAsync(id, text, replyToMessageId: relayToMessageId);
            TimeRelayMessages.Add(new TimeRelayMessage(Parent, ms, TypeOfTimeRealyMessage.AutoDel, time));
            return ms;
        }

        public async Task<Message> SendTimeRelayMessageAsynkAsync(ChatId id, string text, int time, int relayToMessageId = 0)
        {
            var ms = await Parent.Client.SendTextMessageAsync(id, text, replyToMessageId: relayToMessageId);
            TimeRelayMessages.Add(new TimeRelayMessage(Parent, ms, TypeOfTimeRealyMessage.AutoDel, time));
            return ms;
        }

        public async Task<Message> SendTimeRelayMessageAsynkAsync(ChatId id, FileToSend photo, int time, string caption, int relayToMessageId = 0)
        {
            var ms = await Parent.Client.SendPhotoAsync(id, photo, caption, replyToMessageId: relayToMessageId);
            TimeRelayMessages.Add(new TimeRelayMessage(Parent, ms, TypeOfTimeRealyMessage.AutoDel, time));
            return ms;
        }

        public async Task<Message> SendTimeRelayPhotoAsynkAsync(long id, FileToSend photo, int time, string caption, int relayToMessageId = 0)
        {
            var ms = await Parent.Client.SendPhotoAsync(id, photo, caption, replyToMessageId: relayToMessageId);
            TimeRelayMessages.Add(new TimeRelayMessage(Parent, ms, TypeOfTimeRealyMessage.AutoDel, time));
            return ms;
        }

        public async Task<Message> SendTimeRelayPhotoAsynkAsync(string id, FileToSend photo, int time, string caption, int relayToMessageId = 0)
        {
            var ms = await Parent.Client.SendPhotoAsync(id, photo, caption, replyToMessageId: relayToMessageId);
            TimeRelayMessages.Add(new TimeRelayMessage(Parent, ms, TypeOfTimeRealyMessage.AutoDel, time));
            return ms;
        }

        public class SayAfterMe : SynkCommand
        {
            public SayAfterMe() : base(ActSay, new List<string>() { "/say" }, TypeOfAccess.Named, commandName: "скажи", descr: "Бот повторит за вами `-w` текст `-t` тайм удаления в секундах") { }
        }

        public static void ActSay(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null)
            {
                var d = args.Find(fn => fn.Name == "0");
                var arg1 = d == null ? args.Find(fn => fn.Name == "w") : d;
                var c = args.Find(fn => fn.Name == "1");
                var arg2 = c == null ? args.Find(fn => fn.Name == "t") : c;
                if (arg1 != null && arg2 != null)
                {
                    try
                    {
                        Parent.GetModule<TRM>().SendTimeRelayMessageAsynkAsync(re.Chat.Id, arg1.Arg, int.Parse(arg2.Arg));
                    }
                    catch { Parent.Client.SendTextMessageAsync(re.Chat.Id, $"@{re.From.Username} научись пользоваться командами ****"); }
                }
                else if (arg1 != null)
                {
                    Parent.Client.SendTextMessageAsync(re.Chat.Id, arg1.Arg);
                }
                else Parent.Client.SendTextMessageAsync(re.Chat.Id, $"@{re.From.Username} что розговаривать розучился?");
            }
        }
    }
}
