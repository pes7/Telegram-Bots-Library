using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Commands
{
    public class Help : SynkCommand
    {
        public BotBase Parent { get; set; }
        public Help(BotBase bot) : base(Act, new List<string>() { "/help" },descr:"Список команд.") { Parent = bot; }
        public static void Act(Message re, IBot Parent, List<ArgC> args)
        {
            Parent.Client.SendTextMessageAsync(re.Chat.Id,$"This bot[{Parent.Name}] was created with pes7's Bot Creator.");
            string coms = "";
            if (args == null)
            {
                foreach (SynkCommand sn in Parent.SynkCommands.Where(fn => fn.Type == TypeOfCommand.Standart && fn.TypeOfAccess == TypeOfAccess.Public && fn.CommandLine.First() != "Default"))
                {
                    if (sn.Description != null)
                        coms += $"\n{sn.CommandLine.First()} - {sn.Description}";
                    else
                        coms += $"\n{sn.CommandLine.First()}";
                }
                Parent.Client.SendTextMessageAsync(re.Chat.Id, $"Commands: {coms}");
            }
            else
            {
                var arg = args.Find(fn => fn.Name == "admin");
                if(arg != null)
                {
                    Thread th = new Thread(async () =>
                    {
                        bool canwe = false;
                        if (re.Chat.Type != Telegram.Bot.Types.Enums.ChatType.Private)
                            canwe = await WebHook.IsAdminAsync(Parent, re.Chat.Id, re.From.Id);
                        else
                            canwe = true;
                        if (canwe)
                        {
                            foreach (SynkCommand sn in Parent.SynkCommands.Where(fn => fn.Type == TypeOfCommand.Standart && fn.TypeOfAccess == TypeOfAccess.Admin && fn.CommandLine.First() != "Default"))
                            {
                                if (sn.Description != null)
                                    coms += $"\n{sn.CommandLine.First()} - {sn.Description}";
                                else
                                    coms += $"\n{sn.CommandLine.First()}";
                            }
                            await Parent.Client.SendTextMessageAsync(re.Chat.Id, $"Commands: {coms}");
                        }
                    });
                    th.Start();
                }
            }
            
        }
    }
}
