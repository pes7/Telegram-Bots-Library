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
    class WebHook
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
        private bool GachiAttakTrigger = false;
        private async void MessageSynk(Message ms){
            if(ms.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
            {
                Parent.Messages.Add(ms);
                LogSystem(ms.From);
                switch (ms.Text)
                {
                    case "/gachiattak@guchimuchibot":
                    case "/gachiattak":
                        await ClearCommandAsync(ms.Chat.Id,ms.MessageId);
                        if (!GachiAttakTrigger)
                        {
                            await Parent.Client.SendTextMessageAsync(ms.Chat.Id, "Prepare your NyanuS.");
                            Thread th = new Thread(() =>
                            {
                                GuchiAttak(ms.Chat.Id);
                            });
                            th.Start();
                            GachiAttakTrigger = true;
                        }
                        break;
                    case "/sendrandimg@guchimuchibot":
                    case "/sendrandimg":
                        await ClearCommandAsync(ms.Chat.Id, ms.MessageId);
                        if (Parent.GachiImage != null)
                        {
                            await ClearCommandAsync(ms.Chat.Id, ms.MessageId);
                            GetLocalGachi(ms.Chat.Id);
                        }
                        else await Parent.Client.SendTextMessageAsync(ms.Chat.Id,"Sorry, but my creator dont have Gachi Photoes.");
                        break;
                    case "/sendrandwebm@guchimuchibot":
                    case "/sendrandwebm":
                        await ClearCommandAsync(ms.Chat.Id, ms.MessageId);
                        if (Parent.WebmDir != null)
                        {
                            if (!WebmTrigger)
                            {
                                WebmTrigger = true;
                                try
                                {
                                    await GetLocalWebmAsync(ms.Chat.Id);
                                } catch (Exception ex) { Console.WriteLine($"{ex.Message}"); }
                            }
                        } else await Parent.Client.SendTextMessageAsync(ms.Chat.Id, "Sorry, but my creator dont have any webm.");
                        break;
                    case "/testmemory":
                        Parent.CommandsSynk.Add(new Command(ms.Text,ms.From.Id,ms.Chat.Id));
                        await Parent.Client.SendTextMessageAsync(ms.Chat.Id,"Say somethink");
                        break;
                    default:
                        CommandAdd(ms);
                        CommandSynk();
                        break;
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
        private void CommandAdd(Message ms)
        {
            Command cm = Parent.CommandsSynk.Find(fn => fn.ChatId == ms.Chat.Id && fn.UserId == ms.From.Id);
            if (cm != null)
            {
                cm.AddArgc(ms.Text);
                ClearCommandAsync(ms.Chat.Id, ms.MessageId);
            }
        }
        private void CommandSynk()
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
        private async Task ClearCommandAsync(long id, int msgid)
        {
            await Parent.Client.DeleteMessageAsync(id, msgid);
        }
        private void GuchiAttak(long chatid)
        {
            for (int i = 0; i < 5; i++)
            {
                Parent.Client.SendTextMessageAsync(chatid,i.ToString());
                Thread.Sleep(1000);
            }
            Parent.Client.SendTextMessageAsync(chatid, "Start!!!");
            for(int i = 0; i < 20; i++)
            {
                Parent.Client.SendTextMessageAsync(chatid, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                Thread.Sleep(700);
            }
            Parent.Client.SendTextMessageAsync(chatid, "Attak ended.");
            GachiAttakTrigger = false;
        }
        private void GetLocalGachi(long chatid)
        {
            String[] files = Directory.GetFiles(Parent.GachiImage, "*");
            int index = Parent.rand.Next(0, files.Length);
            Parent.Client.SendPhotoAsync(chatid, new FileToSend(Path.GetFileName(files[index]), System.IO.File.Open(files[index],FileMode.Open)));
        }

        /* Webm Module */
        private List<string> Webms;
        private bool WebmTrigger = false;
        private async Task GetLocalWebmAsync(long chatid)
        {
            if (Webms == null)
            {
                Webms = new List<string>();
                Webms.AddRange(Directory.EnumerateFiles(Parent.WebmDir, "*.*")
                .Where(s => s.EndsWith(".webm") || s.EndsWith(".mp4")));
            }
            int index = Parent.rand.Next(0, Webms.Count);
            GetPreViewOfFile(Webms[index]);
            await Parent.Client.SendPhotoAsync(chatid, new FileToSend($"{Path.GetFileName(Webms[index])}.jpg",
                System.IO.File.Open($"{Parent.PreViewDir}{Path.GetFileName(Webms[index])}.jpg", FileMode.Open)));
            await Parent.Client.SendDocumentAsync(chatid, new FileToSend(Path.GetFileName(Webms[index]), System.IO.File.Open(Webms[index], FileMode.Open)));
            WebmTrigger = false;
        }

        private void GetPreViewOfFile(string videopath)
        {
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            ffMpeg.GetVideoThumbnail(videopath, $"{Parent.PreViewDir}{Path.GetFileName(videopath)}.jpg", 5);
        }

    }
}
