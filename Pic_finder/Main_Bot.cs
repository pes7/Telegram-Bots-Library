using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
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
using Newtonsoft.Json;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Concurrent;

namespace Pic_finder
{
    public class BotGetsWrongException : Exception //Виключення для ьота, коли він входить в незадовільну ситуацію.
    {
        public BotGetsWrongException() : base() { }

        public BotGetsWrongException(string message) : base(message) { }

        public BotGetsWrongException(string message, Exception inner) : base(message, inner) { }
    }

    public class Main_Bot : BotBase //Основний 
    {
        private List<Message> preLastMsgs = new List<Message>();
        public Main_Bot(
            System.String api_key,
            System.String name,
            System.String shortName,
            System.String creatorName,
            List<IModule> mods = null
            ) : base(
                api_key,
                name,
                shortName,
                modules: mods,
                usernameofcreator: creatorName){ }
    }
}
