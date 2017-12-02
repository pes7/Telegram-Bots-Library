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
    public abstract class BotBase : BotInteface
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public Random Rand { get; set; } = new Random();
        public Telegram.Bot.TelegramBotClient Client { get; set; }
        public Thread WebThread { get; set; }
        public Thread TMessageQueueSynk { get; set; }
        public Thread TimeSynk { get; set; }
        public WebHook WebHook { get; set; }
        public List<Message> MessagesLast { get; set; }
        public List<dynamic> MessagesQueue { get; set; }
        public List<Command> CommandsSynk { get; set; }
        public List<UserM> ActiveUsers { get; set; }
        public List<Exception> Exceptions { get; set; }

        public Action OnWebHoockUpdated { get; set; } = ()=> { };

        public List<ModuleInterface> Modules { get; set; }
        public T GetModule<T>() where T : ModuleInterface
        {
            return (T)Modules.Find(fn => fn.Type == typeof(T));
        }
        public ModuleInterface GetModule(string name)
        {
            return Modules.Find(fn => fn.Name == name);
        }

        public List<SynkCommand> Commands { get; set; }
        public int CountOfAvailableMessages { get; set; } = 20; // Availble messages for 60 secs
        public int RunTime { get; set; } = 0;

        public BotBase(string key, string name, List<ModuleInterface> modules = null)
        {
            Client = new Telegram.Bot.TelegramBotClient(key);
            Name = name;
            Modules = modules;
            CommandsSynk = new List<Command>();
            MessagesLast = new List<Message>();
            MessagesQueue = new List<dynamic>();
            ActiveUsers = new List<UserM>();
            Commands = new List<SynkCommand>();
            Exceptions = new List<Exception>();
            WebHook = new WebHook(this);
            WebThread = new Thread(() =>
            {
                WebHook.Start();
            });
            WebThread.Start();
            TMessageQueueSynk = new Thread(async () =>
            {
                await MessageQueueSynkAsync();
            });
            TMessageQueueSynk.Start();
            TimeSynk = new Thread(TimeT);
            TimeSynk.Start();
            SynkModules();
        }

        private void SynkModules()
        {
            foreach (ModuleInterface nd in Modules)
            {
                nd.Start();
            }
        }
        public static async Task ClearCommandAsync(long id, int msgid, BotInteface Parent)
        {
            try
            {
                await Parent.Client.DeleteMessageAsync(id, msgid);
            }
            catch (Exception ex)
            {
                Parent.Exceptions.Add(ex);
            }
        }
        public virtual void SendMessage(long ChatId, string text, UserM user = null)
        {
            MessagesQueue.Add(new { id = ChatId, text = text });
        }
        public async Task MessageQueueSynkAsync()
        {
            while (true)
            {
                while (MessagesQueue.Count > 0 && CountOfAvailableMessages > 0)
                {
                    CountOfAvailableMessages--;
                    dynamic ms = MessagesQueue.First();
                    try
                    {
                        await Client.SendTextMessageAsync(ms.id, ms.text);
                    }
                    catch (Exception ex) { Exceptions.Add(ex); }
                    MessagesQueue.RemoveAt(0);
                }
            }
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
            TMessageQueueSynk.Abort();
            TimeSynk.Abort();
            foreach (ModuleInterface md in Modules)
            {
                md.AbortThread();
            }
        }

        private  void TimeT()
        {
            while (true)
            {
                RunTime++;
                Thread.Sleep(1000);
                BotSynk();
                ShowInf();
            }
        }

        public void BotSynk()
        {
            if (RunTime % 60 == 0)
            {
                CountOfAvailableMessages = 20;
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
            foreach (UserM um in ActiveUsers)
            {
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
                foreach (Message ms in MessagesLast)
                {
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
                foreach (Exception ms in Exceptions)
                {
                    Console.WriteLine($"    {ms}");
                }
            }
            Console.WriteLine("}");
        }
    }
}
