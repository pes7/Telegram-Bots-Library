using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace GuchiBot
{
    public class Bot
    {
        private String Key { get; set; }
        public Telegram.Bot.TelegramBotClient Client { get; set; }
        public Random rand { get; }

        private Thread WebThread { get; set; }
        public WebHook WebHook { get; }

        public List<Message> Messages { get; set; }
        public List<Command> CommandsSynk { get; set; }
        public List<UserM> ActiveUsers { get; set; }

        public List<SynkCommand> Commands { get; set; }

        private long LastChatId { get; set; }

        public string WebmDir { get; set; }
        public string GachiImage { get; set; }
        public string PreViewDir { get; set; } //If nun, generated.
        
        public Bot(string key, string webmdir = null, string gachiimage = null, string preViewDir = null)
        {
            Client = new Telegram.Bot.TelegramBotClient(key);
            rand = new Random();
            CommandsSynk = new List<Command>();
            Messages = new List<Message>();
            ActiveUsers = new List<UserM>();
            Commands = new List<SynkCommand>();
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
        }
        private void GenerePreViewDir()
        {
            if(PreViewDir == null)
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
    }
}
