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
        // TimeRelayMessages
        public TRM() : base("TRM", typeof(TRM)) {
            TimeRelayMessages = new List<TimeRelayMessage>();
            MainThread = new Thread(() =>
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
            MainThread.Start();
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
    }
}
