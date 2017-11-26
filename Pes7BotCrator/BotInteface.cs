using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator
{
    public interface BotInteface
    {
        string Key { get; set; }
        Random Rand { get; set; }
        Telegram.Bot.TelegramBotClient Client { get; set; }
        Thread TMessageQueueSynk { get; set; }
        Thread TimeSynk { get; set; }
        WebHook WebHook { get; }
        List<Message> MessagesLast { get; set; }
        List<dynamic> MessagesQueue { get; set; }
        List<Command> CommandsSynk { get; set; }
        List<UserM> ActiveUsers { get; set; }
        List<Exception> Exceptions { get; set; }
        List<ModuleInterface> Modules { get; set; }
        ModuleInterface GetModule(string name);
        List<SynkCommand> Commands { get; set; }
        int CountOfAvailableMessages { get; set; }
        int RunTime { get; set; }
    }
}
