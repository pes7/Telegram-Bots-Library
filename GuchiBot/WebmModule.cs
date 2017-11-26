using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuchiBot;
using Telegram.Bot.Types;

namespace Pes7BotCrator
{
    public class WebmModule
    {
        /* Webm Module */
        private List<string> Webms;
        private static bool WebmTrigger = false;
        delegate void Error(Bot parent);
        private async Task GetLocalWebmAsync(long chatid, Bot Parent)
        {
            if (Webms == null)
            {
                Webms = new List<string>();
                Webms.AddRange(Directory.EnumerateFiles(Parent.WebmDir, "*.*")
                .Where(s => s.EndsWith(".webm") || s.EndsWith(".mp4")));
            }
            int index = Parent.Rand.Next(0, Webms.Count);
            GetPreViewOfFile(Webms[index], Parent);
            await Parent.Client.SendPhotoAsync(chatid, new FileToSend($"{Path.GetFileName(Webms[index])}.jpg",
                System.IO.File.Open($"{Parent.PreViewDir}{Path.GetFileName(Webms[index])}.jpg", FileMode.Open)));
            await Parent.Client.SendDocumentAsync(chatid, new FileToSend(Path.GetFileName(Webms[index]), System.IO.File.Open(Webms[index], FileMode.Open)));
            WebmTrigger = false;
        }

        private void GetPreViewOfFile(string videopath, Bot Parent)
        {
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            ffMpeg.GetVideoThumbnail(videopath, $"{Parent.PreViewDir}{Path.GetFileName(videopath)}.jpg", 5);
        }

        public async void WebmFuncForBot(Message ms, BotInteface Parent)
        {
            Bot PBot = Parent as Bot;
            Error error = async delegate (Bot parent) { await Parent.Client.SendTextMessageAsync(ms.Chat.Id, "Sorry, but my creator dont have any webm."); };
            if (PBot.WebmDir != null)
            {
                if (!WebmTrigger)
                {
                    WebmTrigger = true;
                    try
                    {
                        await GetLocalWebmAsync(ms.Chat.Id, PBot);
                    }
                    catch (Exception ex) {
                        WebmTrigger = false;
                        error(PBot);
                        Parent.Exceptions.Add(ex);
                    }
                }
            }
            else error(PBot);
        }
    }
}
