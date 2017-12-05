﻿using Pes7BotCrator.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Pes7BotCrator.Modules
{
    public class LikeDislikeModule : Module
    {
        /*
         * Нужно сделать доп поле id для опросов, и передавать в кнопках, что бы потом можно бы ло бы подсосать Текст сообщения.
         * 
         */

        public LikeDislikeModule() : base("LikeDislikeModule", typeof(LikeDislikeModule)) { Command = new LikeSynkCommand(); }
        public List<Likes> LLikes { get; set; } = new List<Likes>();
        public int[] LikeDislikeQuata { get; set; }
        public LikeSynkCommand Command { get; set; }

        public class LikeSynkCommand : SynkCommand
        {
            public LikeSynkCommand() : base(Act, new List<string> { "like", "dislike" }) { }
        }

        public static InlineKeyboardMarkup getKeyBoard(string query = null)
        {
            InlineKeyboardMarkup keyboard = null;
            if (query == null)
            {
                keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("👍 0","t:f:type:like"),
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("👎 0","t:f:type:dislike")
                    }
                });
            }
            else
            {
                keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("👍 0",$"t:f:type:like"),
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("👎 0",$"t:f:type:dislike")
                    },
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardSwitchInlineQueryButton("Share",query)
                    }
                });
            }
            return keyboard;
        }

        public static InlineKeyboardMarkup getKeyBoard(int l, int d, string query = null)
        {
            InlineKeyboardMarkup keyboard = null;
            if (query == null)
            {
                keyboard = new InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"👍 {l}",$"t:f:type:like"),
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"👎 {d}",$"t:f:type:dislike")
                    }
                });
            }
            else
            {
                keyboard = new InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"👍 {l}",$"t:f:type:like"),
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"👎 {d}",$"t:f:type:dislike")
                    },
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardSwitchInlineQueryButton("Share",query)
                    }
                });
            }
            return keyboard;
        }

        public static void Act(CallbackQuery re, IBotBase Parent)
        {
            Message ms = re?.Message;
            long id;
            string query = null;
            if (re.Message != null)
            {
                query = re.Message.Caption;
                id = re.Message.MessageId;
            }
            else
                id = long.Parse(re.Id);
            LikeDislikeModule LDModule = Parent.GetModule<LikeDislikeModule>();
            Likes ll;
            if(ms!=null)
                ll = LDModule.LLikes.Find(ff => ff.MessageId == id.ToString());
            else 
                ll = LDModule.LLikes.Find(ff => ff.MessageId == re.InlineMessageId);
            if (ll == null)
            {
                if (ms != null)
                    LDModule.LLikes.Add(new Likes(ms.MessageId, ms.Chat.Id));
                else
                    LDModule.LLikes.Add(new Likes(re.InlineMessageId,id));
                ll = LDModule.LLikes.Last();
            }
            string[] Dd = re.Data.Split(':');
            bool FullShow = Dd[1].Contains("t") ? true : false; 
            if (Dd[3] == "like")
            {
                long li = ll.LikeId.Find(dd => dd == re.From.Id);
                long ld = ll.DisLikeId.Find(dd => dd == re.From.Id);
                if (li == 0 && ld == 0)
                    ll.LikeId.Add(re.From.Id);
                else if (li == 0 && ld != 0)
                {
                    ll.LikeId.Add(ld);
                    ll.DisLikeId.Remove(ld);
                }
                else
                    ll.LikeId.Remove(li);
            }
            else if (Dd[3] == "dislike")
            {
                long li = ll.LikeId.Find(dd => dd == re.From.Id);
                long ld = ll.DisLikeId.Find(dd => dd == re.From.Id);
                if (li == 0 && ld == 0)
                    ll.DisLikeId.Add(re.From.Id);
                else if (li != 0 && ld == 0)
                {
                    ll.DisLikeId.Add(li);
                    ll.LikeId.Remove(li);
                }
                else
                    ll.DisLikeId.Remove(ld);
            }

            /* Квота лайков дизлайков
            if (ll.DisLikeId.Count >= LDModule.LikeDislikeQuata[1])
            {
                Thread sd = new Thread(async () =>
                {
                    await BotBase.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
                });
                sd.Start();
                return;
            }
            */

            InlineKeyboardMarkup keyboard = null;
            if (query!=null)
                keyboard = getKeyBoard(ll.LikeId.Count, ll.DisLikeId.Count, query);
            else
                keyboard = getKeyBoard(ll.LikeId.Count, ll.DisLikeId.Count);
            Thread th = new Thread(async () =>
            {
                try
                {
                    if (ll.Type == Likes.TypeOfLike.Common)
                        await Parent.Client.EditMessageReplyMarkupAsync(ms.Chat.Id, ms.MessageId, keyboard);
                    else {
                        if (FullShow)
                        {
                            await Parent.Client.EditInlineMessageReplyMarkupAsync(re.InlineMessageId, keyboard);
                            //string text = ll;
                            //foreach(long fid in ll.LikeId)
                            //    text = 
                            //await Parent.Client.EditInlineMessageTextAsync(re.InlineMessageId, , replyMarkup: keyboard);
                        }
                        else
                        {
                            await Parent.Client.EditInlineMessageReplyMarkupAsync(re.InlineMessageId, keyboard);
                        }
                    }     
                }
                catch { };
            });
            th.Start();
        }
    }
}
