using Pes7BotCrator.Type;
using Telegram.Bot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pic_finder
{
    class ArgsNeo:Module
    {
        public ArgsNeo() : base("Neo Args Parser", type: typeof(ArgsNeo))
        { }

        public async void OnWebhookUpdate(IBot bot, Update update, ArgC args)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message) return;
            System.String to_parse = System.String.Empty;
            switch (update.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Text:
                    to_parse = update.Message.Text ?? System.String.Empty;
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Photo:
                case Telegram.Bot.Types.Enums.MessageType.Video:
                case Telegram.Bot.Types.Enums.MessageType.Audio:
                case Telegram.Bot.Types.Enums.MessageType.Document:
                    to_parse = update.Message.Caption ?? System.String.Empty;
                    break;
            }
            List<ArgC> args1;
        }

        public static List<ArgC> Parse(System.String to_parse)
        {
            List<ArgC> args = new List<ArgC>();
            if ((to_parse ?? System.String.Empty) != System.String.Empty && to_parse != null)
                foreach (System.String key_value in to_parse.Split(' '))
                {
                    if (key_value == System.String.Empty) continue;
                    ArgC arg = new ArgC();
                    if (key_value.Contains("="))
                    {
                        System.String[] kv_div = key_value.Split('=');
                        if (kv_div.Count() == 0) continue;
                        if (kv_div.Count() == 1) arg.Name = kv_div[0].Trim();
                        if (kv_div.Count()>1)
                        {
                            arg.Name = kv_div[0].Trim();
                            arg.Arg = kv_div[1].Trim();
                        }
                    }
                    else arg.Name = key_value.Trim();
                    args.Add(arg);
                }
            return args;
        }
    }
}
