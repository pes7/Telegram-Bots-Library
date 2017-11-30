using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Commands;
using Pes7BotCrator.Modules;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator
{
    public class WebHook
    {
        private BotInteface Parent { get; set; }
        public WebHook(BotInteface parent)
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
                    switch (Up.Message.Type) {
                        case Telegram.Bot.Types.Enums.MessageType.PhotoMessage:

                            break;
                        case Telegram.Bot.Types.Enums.MessageType.TextMessage:
                            Parent.MessagesLast.Add(ms);
                            LogSystem(ms.From);
                            foreach (SynkCommand sy in Parent.Commands.Where(
                                fn => fn.Type == SynkCommand.TypeOfCommand.Standart &&
                                fn.CommandLine.Exists(nf => nf == ms.Text)))
                            {
                                await BotBase.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
                                Thread the = new Thread(() =>
                                {
                                    sy.doFunc(ms, Parent);
                                });
                                the.Start();
                                break;
                            }
                            Parent.Commands.Find(fn => fn.CommandLine.Exists(nf => nf == "Default"))?.doFunc(ms,Parent);
                            break;
                        case Telegram.Bot.Types.Enums.MessageType.StickerMessage:

                            break;
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQueryUpdate:
                    CallbackQuery qq = Up.CallbackQuery;
                    foreach (SynkCommand sy in Parent.Commands.Where(
                        fn=>fn.Type == SynkCommand.TypeOfCommand.Query 
                        && fn.CommandLine.Exists(nf => Up.CallbackQuery.Data.Contains(nf))))
                    {
                        Thread the = new Thread(() =>
                        {
                            sy.doFunc(qq, Parent, Up);
                        });
                        the.Start();
                        break;
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.InlineQueryUpdate:
                    var query = Up.InlineQuery;
                    foreach(SynkCommand sy in Parent.Commands.Where(fn=>fn.Type == SynkCommand.TypeOfCommand.InlineQuery))
                    {
                        sy.doFunc(query,Parent,Up);
                    }
                    /* ------------------
                     * 
                     * Нужно сделать обработчик кастомных инлайнов!!!
                     * 
                     * ------------------ */

                    
                    break;
            }
            Parent.OnWebHoockUpdated();
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
