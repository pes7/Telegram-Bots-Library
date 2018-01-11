using System.Collections.Generic;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pic_finder
{
    public class micro_logic:Module
    {
        public micro_logic():base("Robot micro logic", typeof(micro_logic)) { }

        public async void SayHello(Message msg, IBot serving, List<ArgC> args)
        {
            await serving.Client.SendTextMessageAsync(msg.Chat.Id, @"Hi, I'm a Telegram bot which get's an image's from service's ""yande.re"", ""Danbooru"" and ""Gelbooru"".
To adjust a criteria of a search, you can use a parametric system, which consist's on ""Argument:Value"" combination.
For example you can type: ""/getyandere -limit:100 -tag:loli -tag:feet -page:2""(without qoutation marks) to enjoy with an another 100 pairs of lolie's feet's. ;-)
If you want to get picture as file, just type with other parameter's ""-file"".
If you a bad guy or a girl, type a ""-show_any"", to see as photo's an explicit pic's");
        }

        public async void EmExit(Message msg, IBot serving, List<ArgC> args)
        {
            await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Bot will got shut down.");
            System.Environment.Exit(0);
        }
    }
}
