using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GuchiBot
{
    class Bot
    {
        private String Key { get; set; }
        public Telegram.Bot.TelegramBotClient Client { get; set; }
        public Random rand { get; }

        private Thread WebThread { get; set; }
        private WebHook WebHook { get; set; }
        
        public Bot(string key = "466088141:AAHIcb1aG8F6P5YQSgcQlqaKJBD9vlLuMAw")
        {
            Client = new Telegram.Bot.TelegramBotClient(key);
            rand = new Random();
            WebHook = new WebHook(this);
            WebThread = new Thread(() =>
            {
                WebHook.Start();
            });
            WebThread.Start();
        }
    }
}
