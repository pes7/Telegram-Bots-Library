using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;
using System.IO;

namespace Pes7BotCrator
{
    public class BotBase : IBot
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string NameString { get; set; }
        public string UserNameOfCreator { get; set; }
        public Random Rand { get; set; } = new Random();
        public Telegram.Bot.TelegramBotClient Client { get; set; }
        public Thread WebThread { get; set; }
        public Thread TimeSynk { get; set; }
        public WebHook WebHook { get; set; }
        public List<Message> MessagesLast { get; set; }
        public List<dynamic> MessagesQueue { get; set; }
        public List<Command> ActionCommands { get; set; }
        public List<UserM> ActiveUsers { get; set; }
        public List<Exception> Exceptions { get; set; }

        public Action OnWebHoockUpdated { get; set; } = null;

        public List<IModule> Modules { get; set; }
        public T GetModule<T>() where T : IModule
        {
            try
            {
                return (T)Modules.Find(fn => fn.Type == typeof(T));
            }
            catch { throw new Exception("There is no this Module."); }
        }
        public IModule GetModule(string name)
        {
            try
            {
                return Modules.Find(fn => fn.Name == name);
            }
            catch { throw new Exception("There is no this Module."); }
        }

        public GList<SynkCommand> SynkCommands { get; set; }

        public int RunTime { get; set; } = 0;

        public CrushReloader Reload { get; set; }
        public BotBase(string key, string name, string nameString, string usernameofcreator = null, List<IModule> modules = null)
        {
            Client = new Telegram.Bot.TelegramBotClient(key);
            Name = name;
            NameString = nameString;
            Modules = modules;
            UserNameOfCreator = usernameofcreator;
            ActionCommands = new List<Command>();
            MessagesLast = new List<Message>();
            MessagesQueue = new List<dynamic>();
            ActiveUsers = new List<UserM>();
            SynkCommands = new GList<SynkCommand>(this);
            Exceptions = new List<Exception>();
            WebHook = new WebHook(this);
            setModulesParent();
            WebThread = new Thread(() =>
            {
                WebHook.Start();
            });
            WebThread.Start();
            TimeSynk = new Thread(TimeT);
            TimeSynk.Start();
            SynkModules();
            Reload = new CrushReloader();
        }

        private void SynkModules()
        {
            foreach (IModule nd in Modules)
            {
                nd.Start();
            }
        }
        public static async Task ClearCommandAsync(long id, int msgid, IBot Parent)
        {
            try
            {
                await Parent.Client.DeleteMessageAsync(id, msgid);
            }
            catch { /*No admin acces in group*/ }
        }
        public virtual void SendMessage(long ChatId, string text, UserM user = null)
        {
            MessagesQueue.Add(new { id = ChatId, text = text });
        }
        /*Нужна Сильная доработка*/
        public async Task<FileStream> getFileFrom(string id, string name = null)
        {
            if (name != null)
            {
                if (System.IO.File.Exists($"./DownloadFiles/{name}"))
                {
                    string filePath = $"./DownloadFiles/{name}";
                    return new FileStream(filePath,FileMode.Open);
                }
            }
            var photo = Client.GetFileAsync(id).Result;
            string filename = photo.FilePath.Split('/')[1];
            if (!Directory.Exists("./DownloadFiles"))
                Directory.CreateDirectory("./DownloadFiles");
            if (System.IO.File.Exists($"./DownloadFiles{filename}"))
                return new FileStream($"./DownloadFiles/{filename}",FileMode.Open);
            var file = new FileStream($"./DownloadFiles/{filename}",FileMode.Create);
            var down = Client.GetFileAsync(id);
            await down.Result.FileStream.CopyToAsync(file);
            return file;
        }
        public virtual void Dispose()
        {
            WebThread.Abort();
            TimeSynk.Abort();
            foreach (IModule md in Modules)
            {
                md.AbortThread();
            }
            Reload.MainTh.Abort();
        }

        private  void TimeT()
        {
            while (true)
            {
                RunTime++;
                Thread.Sleep(1000);
                ShowInf();
            }
        }

        public string TimeToString(int i)
        {
            string str;
            if (i / 60 > 1)
            {
                if (i / 60 / 60 > 1)
                {
                    if (i / 60 / 60 > 24)
                    {
                        str = $"{i / 60 / 60 / 24} days.";
                    }
                    else str = $"{i / 60 / 60} hrs.";
                }
                else str = $"{i / 60} min.";
            }
            else str = $"{i} sec.";
            return str;
        }

        public virtual void ShowInf()
        {
            Console.Clear();
            Console.WriteLine("Bot Stats: {");
            Console.WriteLine($"    Messages count: {MessagesLast.Count} msgs.\n    RunTime: {TimeToString(RunTime)}\n");
            Console.WriteLine("}");
            Console.WriteLine("Active Users: {");
            for(int i = 0; i < ActiveUsers.Count; i++)
            {
                var um = ActiveUsers[i];
                Console.WriteLine($"    {um.Username} {um.MessageCount} messages.");
            }
            Console.WriteLine("}\nLast 10 Messages: {");
            if (MessagesLast.Count > 10)
            {
                for (int i = MessagesLast.Count - 10; i < MessagesLast.Count; i++)
                {

                    Message ms = MessagesLast[i];
                    Console.WriteLine($"    {UserM.usernameGet(ms.From)}: {ms.Text}");
                }
            }
            else
            {
                for (int i =0; i< MessagesLast.Count;i++)
                {
                    var ms = MessagesLast[i];
                    Console.WriteLine($"    {ms.From.Username}: {ms.Text}");
                }
            }
            Console.WriteLine("}");
            Console.WriteLine("Exceptions: {");
            if (Exceptions.Count > 10)
            {
                for (int i = Exceptions.Count - 10; i < Exceptions.Count; i++)
                {
                    Console.WriteLine($"    {Exceptions[i]}");
                }
            }
            else
            {
                for (int i = 0; i < Exceptions.Count; i++)
                {
                    var ex = Exceptions[i];
                    Console.WriteLine($"    {ex}");
                }
            }
            Console.WriteLine("}");
        }

        public void setModulesParent() {
            foreach (var md in Modules) {
                md.Parent = this;
            }
        }
    }
}
