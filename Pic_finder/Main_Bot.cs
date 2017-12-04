using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Pes7BotCrator;
using Pes7BotCrator.Modules;
using Pes7BotCrator.Type;
using System.Threading;
using Pes7BotCrator.Commands;
using Telegram.Bot.Types.ReplyMarkups;
using Pes7BotCrator.Systems;
using System.Diagnostics;
using System.Collections;
using Telegram.Bot.Types;

namespace Pic_finder
{

    public class Main_Bot:BotBase
    {
        private System.String SN_Acc_key;


        public Main_Bot(System.String api_key, List<ModuleInterface> mods=null):base(api_key, modules:mods){}
    }
}
