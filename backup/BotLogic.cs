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
    class BotLogic
    {
        public async void GetGachiImageLogic(Message ms, Bot Parent)
        {
            if (Parent.GachiImage != null)
            {
                //await ClearCommandAsync(ms.Chat.Id, ms.MessageId);
                GetLocalGachi(ms.Chat.Id, Parent);
            }
            else await Parent.Client.SendTextMessageAsync(ms.Chat.Id, "Sorry, but my creator dont have Gachi Photoes.");
        }

        private void GetLocalGachi(long chatid, Bot Parent)
        {
            String[] files = Directory.GetFiles(Parent.GachiImage, "*");
            int index = Parent.rand.Next(0, files.Length);
            Parent.Client.SendPhotoAsync(chatid, new FileToSend(Path.GetFileName(files[index]), System.IO.File.Open(files[index], FileMode.Open)));
        }

        private static bool GachiAttakTrigger = false;
        private void GuchiAttak(long chatid, Bot Parent)
        {
            for (int i = 0; i < 5; i++)
            {
                Parent.Client.SendTextMessageAsync(chatid, i.ToString());
                Thread.Sleep(1000);
            }
            Parent.Client.SendTextMessageAsync(chatid, "Start!!!");
            for (int i = 0; i < 20; i++)
            {
                Parent.Client.SendTextMessageAsync(chatid, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                Thread.Sleep(700);
            }
            Parent.Client.SendTextMessageAsync(chatid, "Attak ended.");
            GachiAttakTrigger = false;
        }

        public async void GachiAttakSynk(Message ms, Bot Parent)
        {
            if (!GachiAttakTrigger)
            {
                await Parent.Client.SendTextMessageAsync(ms.Chat.Id, "Prepare your NyanuS.");
                Thread th = new Thread(() =>
                {
                    GuchiAttak(ms.Chat.Id, Parent);
                });
                th.Start();
                GachiAttakTrigger = true;
            }
        }

        public async void GetArgkSynk(Message ms, Bot Parent)
        {
            Parent.CommandsSynk.Add(new Command(ms.Text, ms.From.Id, ms.Chat.Id));
            await Parent.Client.SendTextMessageAsync(ms.Chat.Id, "Say somethink");
        }

        public void DefaultSynk(Message ms, Bot Parent)
        {
            CommandAdd(ms, Parent);
            CommandSynk(Parent);
        }

        private void CommandAdd(Message ms, Bot Parent)
        {
            Command cm = Parent.CommandsSynk.Find(fn => fn.ChatId == ms.Chat.Id && fn.UserId == ms.From.Id);
            if (cm != null)
            {
                cm.AddArgc(ms.Text);
                Bot.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
            }
        }
        private void CommandSynk(Bot Parent)
        {
            Command[] arg = Parent.CommandsSynk.Where(fn => fn.cArgs.Count > 0).ToArray<Command>();
            for (int i = 0; i < arg.Length; i++)
            {
                switch (arg[i].cCommand)
                {
                    case "/testmemory":
                        Parent.Client.SendTextMessageAsync(arg[i].ChatId, $"You said: {arg[i].cArgs[0]}");
                        Parent.CommandsSynk.Remove(arg[i]);
                        break;
                }
            }
        }
    }
}
