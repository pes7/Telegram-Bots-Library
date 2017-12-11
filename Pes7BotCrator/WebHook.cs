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
using System.Diagnostics;
using System.Collections;

namespace Pes7BotCrator
{
    public class WebHook
    {
        private IBotBase Parent { get; set; }
        public WebHook(IBotBase parent)
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
                    new Thread(() => { MessageSynk(up); }).Start();
                }
            }
        }

        private async void MessageSynk(Update Up) {
            Message ms;
            switch (Up.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.MessageUpdate:
                    ms = Up.Message;
                    switch (Up.Message.Type) {
                        case Telegram.Bot.Types.Enums.MessageType.PhotoMessage:
                            Parent.MessagesLast.Add(ms);
                            break;
                        case Telegram.Bot.Types.Enums.MessageType.TextMessage:
                            Parent.MessagesLast.Add(ms);
                            LogSystem(ms.From);
                            foreach (SynkCommand sy in Parent.Commands.Where(
                                fn => fn.Type == TypeOfCommand.Standart &&
                                fn.CommandLine.Exists(sn => sn == ((getArgs(ms.Text) == null) ? ms.Text : getArgs(ms.Text).First().Name.Trim()) ||
                                                           (sn == tryToParseNameBotCommand(ms.Text) && tryToParseNameBotCommand(ms.Text) != null))
                                ))
                            {
                                await BotBase.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
                                Thread the = new Thread(() =>
                                {
                                    sy.doFunc(ms, Parent, getArgs(ms.Text));
                                });
                                the.Start();
                                break;
                            }
                            Parent.Commands.Find(fn => fn.CommandLine.Exists(nf => nf == "Default"))?.doFunc(ms, Parent, getArgs(ms.Text));
                            break;
                        case Telegram.Bot.Types.Enums.MessageType.StickerMessage:

                            break;
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQueryUpdate:
                    /*
                     Нужно сохранять лайки людей с инлайна в других группах
                     */
                    CallbackQuery qq = Up.CallbackQuery;
                    foreach (SynkCommand sy in Parent.Commands.Where(
                        fn => fn.Type == TypeOfCommand.Query
                        && fn.CommandLine.Exists(nf => Up.CallbackQuery.Data.Contains(nf))))
                    {
                        Thread the = new Thread(() =>
                        {
                            sy.doFunc(qq, Parent);
                        });
                        the.Start();
                        break;
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.InlineQueryUpdate:
                    var query = Up.InlineQuery;
                    foreach (SynkCommand sy in Parent.Commands.Where(fn => fn.Type == TypeOfCommand.InlineQuery))
                    {
                        sy.doFunc(query, Parent);
                    }
                    break;
            }
            Parent.OnWebHoockUpdated();
        }
        private void LogSystem(User us)
        {
            UserM mu = Parent.ActiveUsers.Find(f => f.Id == us.Id && UserM.usernameGet(f) == UserM.usernameGet(us));
            if (mu == null)
            {
                Parent.ActiveUsers.Add(new UserM(us, 1));
            } else
            {
                mu.MessageCount++;
            }
        }
        private List<ArgC> getArgs(string message)
        {
            List<ArgC> Args = new List<ArgC>();
            string[] args_parse = null;
            try
            {
                args_parse = message.Split('-');
            }
            catch { return null; }
            if (args_parse.Length > 1)
            {
                for (int i = 0; i < args_parse.Length; i++)
                {
                    var sf = new ArgC();
                    try
                    {
                        string[] ssf = args_parse[i].Split(':');
                        if(ssf.Length > 1)
                        {
                            sf.Name = ssf[0];
                            sf.Arg = ssf[1];
                        }else
                        {
                            sf.Name = ssf[0];
                        }
                    }
                    catch { sf.Name = args_parse[i]; }
                    Args.Add(sf);
                }
                return Args;
            }
            else return null;
        }
        private string tryToParseNameBotCommand(string s)
        {
            try
            {
                return s.Split('@').First();
            }
            catch { return null; }
        }
    }
}
