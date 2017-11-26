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

                    /* ------------------
                     * 
                     * Нужно сделать обработчик кастомных инлайнов!!!
                     * 
                     * ------------------ */

                    if (Parent.Modules.Exists(fn => fn.Name == "_2chModule")) {
                        dynamic webm = _2chModule.WebmsSent.Find(fn => fn.path == query.Query);
                        if (webm != null) {
                            var msg = new Telegram.Bot.Types.InputMessageContents.InputTextMessageContent
                            {
                                MessageText = $"{webm.thumbnail}\n{webm.path}",
                                ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html,
                            };

                            Telegram.Bot.Types.InlineQueryResults.InlineQueryResult[] results = {
                                new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle{
                                    Id = "0",
                                    InputMessageContent = msg,
                                    ReplyMarkup = LikeDislikeComponent.getKeyBoard(),
                                    Title = "WEBM",
                                    Description = "POST"
                                }
                        };
                            /*Id = "0",
                            ReplyMarkup = LikeDislikeComponent.getKeyBoard(),
                            InputMessageContent = msg,
                            Title = "POST WEBM",
                            Url = webm.thumbnail,
                            ThumbUrl = webm.thumbnail,
                            Description = webm.path*/
                            await Parent.Client.AnswerInlineQueryAsync(query.Id, results);
                        }
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
