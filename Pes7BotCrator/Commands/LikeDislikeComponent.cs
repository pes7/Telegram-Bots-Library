using Pes7BotCrator.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Pes7BotCrator.Commands
{
    public class LikeDislikeComponent : SynkCommand
    {
        public static InlineKeyboardMarkup getKeyBoard()
        {
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                new [] {
                    new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("👍 0","like"),
                    new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("👎 0","dislike")
                }
            });
            return keyboard;
        }

        public static void Act(Message ms, Bot Parent, Update Up)
        {
            Likes ll = Parent.LLikes.Find(ff => ff.MessageId == ms.MessageId);
            if (ll == null)
            {
                Parent.LLikes.Add(new Likes(ms.MessageId, ms.Chat.Id));
                ll = Parent.LLikes.Last();
            }
            if (Up.CallbackQuery.Data == "like")
            {
                long li = ll.LikeId.Find(dd => dd == Up.CallbackQuery.From.Id);
                long ld = ll.DisLikeId.Find(dd => dd == Up.CallbackQuery.From.Id);
                if (li == 0 && ld == 0)
                    ll.LikeId.Add(Up.CallbackQuery.From.Id);
                else if (li == 0 && ld != 0)
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
            if (ll.DisLikeId.Count >= Parent.LikeDislikeQuata[1])
            {
                Thread sd = new Thread(async () =>
                {
                    await Bot.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
                });
                sd.Start();
                return;
            }

            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                        new [] {
                            new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"👍 {ll.LikeId.Count}","like"),
                            new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"👎 {ll.DisLikeId.Count}","dislike")
                        }
                    });
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

        public LikeDislikeComponent() : base(Act, new List<string> {"like", "dislike" }) { }
    }
}
