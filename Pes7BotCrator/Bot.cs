using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Pes7BotCrator
{
    public class Bot
    {
        private String Key { get; set; }
        public Telegram.Bot.TelegramBotClient Client { get; set; }
        public Random rand { get; }

        private Thread WebThread { get; set; }
        private Thread TMessageQueueSynk { get; set; }
        private Thread TimeSynk { get; set; }
        public WebHook WebHook { get; }

        public List<Message> MessagesLast { get; set; }
        public List<dynamic> MessagesQueue { get; set; }
        public List<Command> CommandsSynk { get; set; }
        public List<UserM> ActiveUsers { get; set; }
        public List<Exception> Exceptions { get; set; }
        public List<Likes> LLikes { get; set; } 

        public List<SynkCommand> Commands { get; set; }

        public int CountOfAvailableMessages { get; set; } = 20; // Availble messages for 60 secs
        public int RunTime { get; set; } = 0;

        public string WebmDir { get; set; }
        public string GachiImage { get; set; }
        public string PreViewDir { get; set; } //If nun, generated.

        public Bot(string key, string webmdir = null, string gachiimage = null, string preViewDir = null)
        {
            Client = new Telegram.Bot.TelegramBotClient(key);
            rand = new Random();
            CommandsSynk = new List<Command>();
            MessagesLast = new List<Message>();
            MessagesQueue = new List<dynamic>();
            ActiveUsers = new List<UserM>();
            Commands = new List<SynkCommand>();
            Exceptions = new List<Exception>();
            LLikes = new List<Likes>();
            WebmDir = webmdir;
            GachiImage = gachiimage;
            PreViewDir = preViewDir;
            GenerePreViewDir();
            WebHook = new WebHook(this);
            WebThread = new Thread(() =>
            {
                WebHook.Start();
            });
            WebThread.Start();
            TMessageQueueSynk = new Thread(() =>
            {
                MessageQueueSynkAsync();
            });
            TMessageQueueSynk.Start();
            TimeSynk = new Thread(TimeT);
            TimeSynk.Start();
        }
        private void GenerePreViewDir()
        {
            if (PreViewDir == null)
            {
                if (!Directory.Exists(PreViewDir))
                {
                    Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}/previews/");
                    PreViewDir = $"{AppDomain.CurrentDomain.BaseDirectory}/previews/";
                }
            }
        }
        public static async Task ClearCommandAsync(long id, int msgid, Bot Parent)
        {
            await Parent.Client.DeleteMessageAsync(id, msgid);
        }
        public void SendMessage(long ChatId, string text, UserM user = null)
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
                    //ShowInf();
                }
            }
        }

        public void Dispose()
        {
            WebThread.Abort();
            TMessageQueueSynk.Abort();
            TimeSynk.Abort();
        }

        private void TimeT()
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
            if(RunTime % 60 == 0)
            {
                CountOfAvailableMessages = 20;
            }
        }

        public void ShowInf()
        {
            Console.Clear();
            Console.WriteLine("Bot Stats: {");
            Console.WriteLine($"    Messages count: {MessagesLast.Count} msgs.\n    Available messages: {CountOfAvailableMessages}\n    RunTime: {RunTime} sec.");
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
            foreach(Exception ex in Exceptions)
            {
                Console.WriteLine($"{ex.Message}");
            }
            Console.WriteLine("}");
        }

        internal void SendTextMessageAsync(object id, string v1, bool v2, bool v3, int v4, InlineKeyboardMarkup keyboard, ParseMode @default)
        {
            throw new NotImplementedException();
        }
    }
}
