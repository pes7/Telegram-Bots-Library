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
                    Console.WriteLine($"[{up.Id}]{up.Message.Text}");
                    offset = up.Id + 1;
                    new Thread(()=> { MessageSynk(up.Message); }).Start();
                }
            }
        }
        private bool GachiAttakTrigger = false;
        private async void MessageSynk(Message ms){
            if(ms.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
            {
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
                        GetLocalGachi(ms.Chat.Id);
                        break;
                    case "/sendrandwebm@guchimuchibot":
                    case "/sendrandwebm":
                        await ClearCommandAsync(ms.Chat.Id, ms.MessageId);
                        if (!WebmTrigger)
                        {
                            WebmTrigger = true;
                            await GetLocalWebmAsync(ms.Chat.Id);
                        }
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
            String[] files = Directory.GetFiles("C:/Users/user/Desktop/GachiArch","*");
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
                Webms.AddRange(Directory.EnumerateFiles("G:/WebServers/home/apirrrsseer.ru/www/List_down/video", "*.*")
                .Where(s => s.EndsWith(".webm") || s.EndsWith(".mp4")));
            }
            int index = Parent.rand.Next(0, Webms.Count);
            GetPreViewOfFile(Webms[index]);
            await Parent.Client.SendPhotoAsync(chatid, new FileToSend($"{AppDomain.CurrentDomain.BaseDirectory}/previews/{Path.GetFileName(Webms[index])}.jpg",
                System.IO.File.Open($"{AppDomain.CurrentDomain.BaseDirectory}/previews/{Path.GetFileName(Webms[index])}.jpg", FileMode.Open)));
            await Parent.Client.SendDocumentAsync(chatid, new FileToSend(Path.GetFileName(Webms[index]), System.IO.File.Open(Webms[index], FileMode.Open)));
            WebmTrigger = false;
        }

        private void GetPreViewOfFile(string videopath)
        {
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            ffMpeg.GetVideoThumbnail(videopath, $"{AppDomain.CurrentDomain.BaseDirectory}/previews/{Path.GetFileName(videopath)}.jpg", 5);
        }

    }
}
