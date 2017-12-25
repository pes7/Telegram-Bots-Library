using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Type
{
    public interface IBot
    {
        string Key { get; set; }
        string Name { get; set; }
        Random Rand { get; set; }
        Telegram.Bot.TelegramBotClient Client { get; set; }
        Thread TimeSynk { get; set; }
        WebHook WebHook { get; }
        List<Message> MessagesLast { get; set; }
        List<dynamic> MessagesQueue { get; set; }
        List<Command> ActionCommands { get; set; }
        List<UserM> ActiveUsers { get; set; }
        List<Exception> Exceptions { get; set; }
        List<IModule> Modules { get; set; }
        List<SynkCommand> SynkCommands { get; set; }
        T GetModule<T>() where T : IModule;
        Action OnWebHoockUpdated { get; set; }
        int RunTime { get; set; }
        string TimeToString(int i);
        void setModulesParent();
    }
}
