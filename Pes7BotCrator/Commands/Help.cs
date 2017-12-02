using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Commands
{
    public class Help : SynkCommand
    {
        public BotBase Parent { get; set; }
        public Help(BotBase bot) : base(Act, new List<string>() { "/help" },descr:"Список команд.") { Parent = bot; }
        public static void Act(Message re, BotInteface Parent, List<ArgC> args)
        {
            Parent.Client.SendTextMessageAsync(re.Chat.Id,$"This bot[{Parent.Name}] was created with pes7's Bot Creator.");
            string coms = "";
            foreach(SynkCommand sn in Parent.Commands.Where(fn=>fn.Type==TypeOfCommand.Standart && fn.CommandLine.First() != "Default"))
            {
                if(sn.Description != null)
                    coms += $"\n{sn.CommandLine.First()} - {sn.Description}";
                else
                    coms += $"\n{sn.CommandLine.First()}";
            }
            Parent.Client.SendTextMessageAsync(re.Chat.Id, $"Commands: {coms}");
        }
    }
}
