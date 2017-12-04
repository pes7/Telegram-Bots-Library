using Pes7BotCrator.Type;
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
         * НУЖНО ЗАНОСИТЬ В JSON всех кто лайкнул и дизлайкнул
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
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("👍 0","type:like:l:0:d:0"),
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("👎 0","type:dislike:l:0:d:0")
                    }
                });
            }
            else
            {
                keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("👍 0","type:like:l:0:d:0"),
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("👎 0","type:dislike:l:0:d:0")
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
                keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"👍 {l}",$"type:like:l:{l}:d:{d}"),
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"👎 {d}",$"type:dislike:l:{l}:d:{d}")
                    }
                });
            }
            else
            {
                keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"👍 {l}",$"type:like:l:{l}:d:{d}:query:{query}"),
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"👎 {d}",$"type:dislike:l:{l}:d:{d}:query:{query}")
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
            Message ms = re.Message;
            if (ms != null)
            {
                LikeDislikeModule LDModule = Parent.GetModule<LikeDislikeModule>();
                Likes ll = LDModule.LLikes.Find(ff => ff.MessageId == ms.MessageId);
                if (ll == null)
                {
                    LDModule.LLikes.Add(new Likes(ms.MessageId, ms.Chat.Id));
                    ll = LDModule.LLikes.Last();
                }
                string[] Dd = re.Data.Split(':');
                if (Dd[1] == "like")
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
                else if (Dd[1] == "dislike")
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
                if (ll.DisLikeId.Count >= LDModule.LikeDislikeQuata[1])
                {
                    Thread sd = new Thread(async () =>
                    {
                        await BotBase.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
                    });
                    sd.Start();
                    return;
                }

                InlineKeyboardMarkup keyboard = null;
                if (Dd.Length > 6)
                    keyboard = getKeyBoard(ll.LikeId.Count, ll.DisLikeId.Count, Dd[7]);
                else
                    keyboard = getKeyBoard(ll.LikeId.Count, ll.DisLikeId.Count);
                Thread th = new Thread(async () =>
                {
                    try
                    {
                        await Parent.Client.EditMessageReplyMarkupAsync(ms.Chat.Id, ms.MessageId, keyboard);
                    }
                    catch { };
                });
                th.Start();
            }
            else
            {
                /*
                 * 
                 * Сделать привязку к ЮЗВЕРЮ который лайкал!!!
                 * Я добавил там кое что в Likes по этому можно начать реализовывать
                 * 
                 * */


                string[] query = re.Data.Split(':');
                int l = int.Parse(query[3]);
                int d = int.Parse(query[5]);
                if (query[1] == "like") l++; else d++;

                InlineKeyboardMarkup keyboard = null;
                if (query.Length > 6)
                    keyboard = getKeyBoard(l, d, query[7]);
                else
                    keyboard = getKeyBoard(l, d);
                Thread th = new Thread(async () =>
                {
                    try
                    {
                        await Parent.Client.EditInlineMessageReplyMarkupAsync(re.InlineMessageId, keyboard);
                    }
                    catch { };
                });
                th.Start();
            }
        }
    }
}
