using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Modules.Types
{
    public enum TypeOfTimeRealyMessage { /*Delayed,*/ Repeat, AutoDel }
    public class TimeRelayMessage : Message
    {
        public TypeOfTimeRealyMessage TypeM { get; set; }
        public int Time { get; set; }
        public Nullable<int> Count { get; set; }
        public Thread MessageThread { get; set; }
        public bool IsAlife { get; set; } = true;
        public IBotBase Parent { get; set; }
        public TimeRelayMessage(IBotBase parent, Message ms, TypeOfTimeRealyMessage tp, int time, Nullable<int> count = null) : base() {
            TypeM = tp;
            Time = time;
            Count = count;
            Parent = parent;

            base.MessageId = ms.MessageId;
            base.Chat = ms.Chat;
            base.Text = ms.Text;
            base.ReplyToMessage = ms.ReplyToMessage;

            MessageThread = new Thread(async () =>
            {
                while (true)
                {
                    await MessageSynkAsync();
                    Thread.Sleep(1000);
                }
            });
            MessageThread.Start();
        }

        private int curTime = 0;
        private async Task MessageSynkAsync()
        {
            if (curTime >= Time)
            {
                if (Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
                {
                    await ForTextMessageAsync();
                }
            }
            curTime++;
        }

        private void GoToDie()
        {
            MessageThread.Abort();
            IsAlife = false;
        }

        // Проверить реально ли умер поток. НУЖНО!!!
        private async Task ForTextMessageAsync()
        {
            switch (TypeM)
            {
                case TypeOfTimeRealyMessage.Repeat:
                    if (Count > 0)
                    {
                        await SendTextMessage();
                        Count--;
                    } else GoToDie();
                    break;
                case TypeOfTimeRealyMessage.AutoDel:
                    try
                    {
                        await Parent.Client.DeleteMessageAsync(Chat.Id, MessageId);
                        GoToDie();
                    }
                    catch { }
                    break;
            }
        }

        private async Task<Message> SendTextMessage()
        {
            return await Parent.Client.SendTextMessageAsync(Chat.Id, Text, replyToMessageId: MessageId);
        }
    }
}
