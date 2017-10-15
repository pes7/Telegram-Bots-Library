using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator
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

        private async void MessageSynk(Message ms){
            if(ms.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
            {
                Parent.MessagesLast.Add(ms);
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
                //Parent.ShowInf();
            }
        }
        private void LogSystem(User us)
        {
            UserM mu = Parent.ActiveUsers.Find(f => f.Id == us.Id && UserM.usernameGet(f) == UserM.usernameGet(us));
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
