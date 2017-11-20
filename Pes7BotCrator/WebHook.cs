using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator
{
    public class WebHook
    {
        private Bot Parent { get; set; }
        public WebHook(Bot parent)
        {
            Parent = parent;
            Parent.Client.SetWebhookAsync("");
        }
        public async void Start()
        {
            int offset = 0;
            while (true)
            {
                var update = await Parent.Client.GetUpdatesAsync(offset);
                foreach (var up in update)
                {
                    offset = up.Id + 1;
                    new Thread(()=> { MessageSynk(up); }).Start();
                }
            }
        }

        private async void MessageSynk(Update Up){
            Message ms;
            switch (Up.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.MessageUpdate:
                    ms = Up.Message;
                    if (ms?.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
                    {
                        Parent.MessagesLast.Add(ms);
                        LogSystem(ms.From);
                        foreach (SynkCommand sy in Parent.Commands.Where(
                            fn=>fn.Type == SynkCommand.TypeOfCommand.Standart && 
                            fn.CommandLine.Exists(nf => nf == ms.Text)))
                        {
                            await Bot.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
                            Thread the = new Thread(() =>
                            {
                                sy.doFunc(ms, Parent);
                            });
                            the.Start();
                            break;
                        }
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQueryUpdate:
                    ms = Up.CallbackQuery.Message;
                    foreach (SynkCommand sy in Parent.Commands.Where(
                        fn=>fn.Type == SynkCommand.TypeOfCommand.Query 
                        && fn.CommandLine.Exists(nf => nf == Up.CallbackQuery.Data)))
                    {
                        Thread the = new Thread(() =>
                        {
                            sy.doFunc(ms, Parent, Up);
                        });
                        the.Start();
                        break;
                    }
                    break;
            }
        }
        private void LogSystem(User us)
        {
            UserM mu = Parent.ActiveUsers.Find(f => f.Id == us.Id && UserM.usernameGet(f) == UserM.usernameGet(us));
            if (mu == null)
            {
                Parent.ActiveUsers.Add(new UserM(us,1));
            }else
            {
                mu.MessageCount++;
            }
        }

    }
}
