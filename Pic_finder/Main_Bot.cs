using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator;
using Pes7BotCrator.Commands;
using Pes7BotCrator.Modules;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Pic_finder
{
    public class BotGetsWrongException:Exception
    {
        public BotGetsWrongException():base(){ }

        public BotGetsWrongException(string message) : base(message) { }

        public BotGetsWrongException(string message, Exception inner):base(message, inner) { }
    }

    public class Main_Bot:BotBase
    {
        public Main_Bot(System.String api_key, List<IModule> mods=null):base(api_key, "", modules:mods){}
    }
}
