using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace GuchiBot
{
    public class WebHook
    {
        private Bot Parent { get; set; }
        public WebHook(Bot parent)
        {
            Parent = parent;
            Parent.Client.SetWebhookAsync("");
        }
        public async void Start()
        {
            int offset = 0;
            //Console.Clear();
            while (true)
            {
                var update = await Parent.Client.GetUpdatesAsync(offset);
                foreach (var up in update)
                {
                    offset = up.Id + 1;
                    new Thread(()=> { MessageSynk(up.Message); }).Start();
                }
            }
        }
        private void ShowInf()
        {
            Console.Clear();
            Console.WriteLine("Active Users: {");
            foreach (UserM um in Parent.ActiveUsers)
            {
                Console.WriteLine($"    {um.Username} {um.MessageCount} messages.");
            }
            Console.WriteLine("}\nMessages: {");
            foreach (Message ms in Parent.Messages)
            {
                Console.WriteLine($"    {ms.From.Username}: {ms.Text}");
            }
            Console.WriteLine("}");
        }
        
        private async void MessageSynk(Message ms){
            if(ms.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
            {
                Parent.Messages.Add(ms);
                LogSystem(ms.From);
                bool iser = false;
                foreach(SynkCommand sy in Parent.Commands)
                {
                    if (sy.CommandLine.Exists(fn=>fn== ms.Text))
                    {
                        await Bot.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
                        Thread th = new Thread(() =>
                        {
                            sy.doFunc(ms, Parent);
                        });
                        th.Start();
                        iser = true;
                        break;
                    }
                }
                if (!iser) {
                    SynkCommand cm = Parent.Commands.Find(fn => fn.CommandLine.Exists(f=>f=="default"));
                    if (cm != null) {
                        cm.doFunc(ms, Parent);
                    }
                }
                ShowInf();
            }
        }
        private void LogSystem(User us)
        {
            UserM mu = Parent.ActiveUsers.Find(f => f.Id == us.Id && f.Username == us.Username);
            if (mu == null)
            {
                Parent.ActiveUsers.Add(new UserM(us,1));
            }else
            {
                mu.MessageCount++;
            }
        }

    }
}
