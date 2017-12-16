using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Commands
{
    public class Statistic : Module
    {
        public Dictionary<string, int> CommandStat { get; set; }
        public SynkCommand CommandHelp { get; set; }
        public StatisticRuntimeCommand CommandRuntime { get; set; }
        public Statistic() : base("Statistic",typeof(Statistic)){
            CommandStat = new Dictionary<string, int>();
            CommandHelp = new StatisticCommand();
            CommandRuntime = new StatisticRuntimeCommand();
        }
        public class StatisticCommand : SynkCommand
        {
            public StatisticCommand() : base(Act, new List<string>() { "/stat" }, descr: "Статистика чата.") { }
        }
        public class StatisticRuntimeCommand : SynkCommand
        {
            public StatisticRuntimeCommand() : base(ActAll) { }
        }
        public static void Act(Message re, IBotBase Parent, List<ArgC> args)
        {
            string add = "";
            if (Parent.ActiveUsers.Count > 0)
                add = "\nUser Stats: {";
            foreach (UserM um in Parent.ActiveUsers)
            {
                add += $"\n    {UserM.usernameGet(um)} {um.MessageCount} msgs.";
            }
            add += "\n}\nCommand Statistic: {";
            foreach(var j in Parent.GetModule<Statistic>().CommandStat)
            {
                add += $"\n{j.Key} - {j.Value}";
            }
            add += "\n}";
            Parent.Client.SendTextMessageAsync(re.Chat.Id, $"Messages count: {Parent.MessagesLast.Count} msgs.\nRunTime: {Parent.TimeToString(Parent.RunTime)}{add}");
        }
        public static void ActAll(Update up, IBotBase Parent, List<ArgC> args)
        {
            int i = 0;
            if (up.Message != null && up?.Message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage && args != null)
            {
                var CommandStat = Parent.GetModule<Statistic>().CommandStat;
                string command = null;
                if (args != null)
                {
                    command = args[0].Name;
                }
                else if (up != null)
                {
                    command = up.Message.Text;
                }
                ISynkCommand sn = Parent.Commands.Find(fn => fn.CommandLine.Contains(command));
                if (sn != null)
                {
                    try
                    {
                        CommandStat[sn.CommandLine.First()]++;
                    }
                    catch
                    {
                        CommandStat.Add(sn.CommandLine.First(), 0);
                    }
                }
            }
        }
    }
}
