using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Commands
{
    public class Statistic : SynkCommand
    {
        public BotBase Parent { get; set; }
        public Statistic(BotBase bot) : base(Act, new List<string>() { "/stat" },descr:"Статистика чата.") { Parent = bot; }
        public static void Act(Message re, BotInteface Parent, List<ArgC> args)
        {
            string add = "";
            if (Parent.ActiveUsers.Count > 0)
                add = "\nUser Stats: {";
            foreach (UserM um in Parent.ActiveUsers)
            {
                add += $"\n    {UserM.usernameGet(um)} {um.MessageCount} msgs.";
            }
            add += "\n}";
            Parent.Client.SendTextMessageAsync(re.Chat.Id, $"Messages count: {Parent.MessagesLast.Count} msgs.\nRunTime: {Parent.TimeToString(Parent.RunTime)}{add}");
        }
    }
}
