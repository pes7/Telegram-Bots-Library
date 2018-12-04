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
        public delegate void AutoIvoke(IBot serving, Message msg);
        public List<AutoIvoke> AutoInvokes = new List<AutoIvoke>();
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
                usernameofcreator: creatorName)
        {
            this.OnWebHoockUpdated += this.FireListener;
        }

        private void FireListener()
        {
            try
            {
                Thread thread = new Thread(delegate ()
                {
                    Message[] msgslast, prelast;
                    lock (this.MessagesLast)
                    {
                        msgslast = new Message[this.MessagesLast.Count];
                        this.MessagesLast.CopyTo(msgslast);
                    }
                    lock(this.preLastMsgs)
                    {
                        prelast = new Message[this.preLastMsgs.Count];
                        this.preLastMsgs.CopyTo(prelast);
                    }
                    List<Message> listen_msgs = msgslast.ToList().Except(prelast.ToList()).ToList();
                    this.preLastMsgs.Clear();
                    this.preLastMsgs.AddRange(msgslast);
                    foreach (AutoIvoke invoke in this.AutoInvokes)
                    {
                        foreach (Message msg in listen_msgs) invoke(this, msg);
                    }
                });
                thread.Start();
            }
            catch(Exception ex)
            {
                this.Exceptions.Add(ex);
            }
        }
    }
}
