﻿using System;
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
            //Console.Clear();
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
                        bool iser = false;
                        foreach (SynkCommand sy in Parent.Commands)
                        {
                            if (sy.CommandLine.Exists(fn => fn == ms.Text))
                            {
                                await Bot.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
                                Thread th = new Thread(() =>
                                {
                                    sy.doFunc(ms, Parent);
                                });
                                th.Start();
                                iser = true;
                                break;
                            }
                        }
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQueryUpdate:
                    ms = Up.CallbackQuery.Message;
                    Likes ll = Parent.LLikes.Find(ff => ff.MessageId == ms.MessageId);
                    if (ll == null)
                    {
                        Parent.LLikes.Add(new Likes(ms.MessageId,ms.Chat.Id));
                        ll = Parent.LLikes.Last();
                    }
                    if (Up.CallbackQuery.Data == "like")
                    {
                        long li = ll.LikeId.Find(dd => dd == Up.CallbackQuery.From.Id);
                        long ld = ll.DisLikeId.Find(dd => dd == Up.CallbackQuery.From.Id);
                        if (li == 0 && ld == 0)
                            ll.LikeId.Add(Up.CallbackQuery.From.Id);
                        else if(li == 0 && ld != 0)
                        {
                            ll.LikeId.Add(ld);
                            ll.DisLikeId.Remove(ld);
                        }
                        else
                            ll.LikeId.Remove(li);
                    }
                    else if (Up.CallbackQuery.Data == "dislike")
                    {
                        long li = ll.LikeId.Find(dd => dd == Up.CallbackQuery.From.Id);
                        long ld = ll.DisLikeId.Find(dd => dd == Up.CallbackQuery.From.Id);
                        if (li == 0 && ld == 0)
                            ll.DisLikeId.Add(Up.CallbackQuery.From.Id);
                        else if (li != 0 && ld == 0)
                        {
                            ll.DisLikeId.Add(li);
                            ll.LikeId.Remove(li);
                        }
                        else
                            ll.DisLikeId.Remove(ld);
                    }
                    var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                        new [] {
                            new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"👍 {ll.LikeId.Count}","like"),
                            new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"👎 {ll.DisLikeId.Count}","dislike")
                        }
                    });
                    try
                    {
                        await Parent.Client.EditMessageReplyMarkupAsync(ms.Chat.Id, ms.MessageId, keyboard);
                    }
                    catch{ };
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