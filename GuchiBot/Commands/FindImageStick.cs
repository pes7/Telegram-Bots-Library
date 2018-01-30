using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace GuchiBot.Commands
{
    public class FindImageStick : SynkCommand
    {
        public FindImageStick() : base(InlineAct, new List<string> { "_noon" }) { }

        public static void InlineAct(InlineQuery query, IBot Parent)
        {
            Random rand = new Random();
            if (Parent.Modules.Exists(fn => fn.Name == "BindedImages") && query.Query.Contains("ib"))
            {
                var image = query.Query.Split('b').Last();
                if (image != null)
                {
                    Telegram.Bot.Types.InlineQueryResults.InlineQueryResult[] results = {
                            new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultPhoto{
                                Id = rand.Next(0,1488).ToString(),
                                Url = $"http://botimages.kl.com.ua/{image}.png",
                                ThumbUrl = $"http://botimages.kl.com.ua/{image}.png"
                            }
                    };
                    Parent.Client.AnswerInlineQueryAsync(query.Id, results);
                }
            }
        }
    }
}
