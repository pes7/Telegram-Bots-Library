using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;
using System.Threading;
using System.Diagnostics;

namespace Pes7BotCrator.Modules
{
    public class _2chModule
    {
        private List<ThBoard> Get2chBoards(Bot Parent)
        {
            List<ThBoard> Th = new List<ThBoard>();
            string[] boards = {
                "http://2ch.hk/b/catalog_num.json"
            };
            foreach (string adress in boards) {
                dynamic s = ThBoard.GetJson(adress);
                foreach (dynamic h in s.threads)
                {
                    if (h.num != null && h.files_count != null)
                    {
                        ThBoard th = new ThBoard((string)h.num, (string)h.comment, (string)h.date, (string)h.files_count);
                        Th.Add(th);
                    }
                }
            }
            return Th;
        }

        public void ParseWebmsFromDvach(Bot Parent)
        {
            DvochSynkAsync(Get2chBoards(Parent), Parent);
        }

        public async Task DvochSynkAsync(List<ThBoard> th, Bot Parent)
        {
            foreach (ThBoard t in th)
            {
                if (t.Discription.Contains("WEBM"))
                {
                    dynamic s = ThBoard.GetJson($"http://2ch.hk/b/res/{t.Id}.json");
                    foreach (dynamic h in s.threads)
                    {
                        foreach (dynamic c in h.posts)
                        {
                            foreach (dynamic f in c.files)
                            {
                                var file = new { fullname = f.fullname, path = "https://2ch.hk" + f.path, thumbnail = "https://2ch.hk" + f.thumbnail };
                                string format = ((string)file.path).Split('.')[2];
                                if (format == "webm")
                                {
                                    Console.WriteLine($"{format}|{file.fullname}|{file.path}|{file.thumbnail}");
                                    await Parent.Client.SendPhotoAsync(Parent.MessagesLast.Last().Chat.Id, new FileToSend(file.thumbnail), file.path);
                                }
                            }
                        }
                    }
                }
            }
        }

        private List<dynamic> getWebms(Bot Parent, string adress)
        {
            List<dynamic> Dy = new List<dynamic>();
            List<ThBoard> Th = Get2chBoards(Parent);
            try
            {
                foreach (ThBoard t in Th)
                {
                    if (t.Discription.Contains("WEBM") || t.Discription.Contains("webm"))
                    {
                        dynamic s = ThBoard.GetJson($"{adress}");
                        foreach (dynamic h in s.threads)
                        {
                            foreach (dynamic c in h.posts)
                            {
                                foreach (dynamic f in c.files)
                                {
                                    var file = new { fullname = f.fullname, path = "https://2ch.hk" + f.path, thumbnail = "https://2ch.hk" + f.thumbnail };
                                    string format = ((string)file.path).Split('.')[2];
                                    if (format == "webm")
                                    {
                                        Dy.Add(file);
                                    }
                                }
                            }
                        }
                    }
                }
            }catch(Exception ex)
            {
                Parent.Exceptions.Add(ex);
            }
            return Dy;
        }

        public static int WebmCountW = 0;
        public static int WebmCountA = 0;
        private List<dynamic> WebmsW;
        private List<dynamic> WebmsA;
        public void Ragenerated(Message ms, Bot Parent)
        {
            if (ms.From.Username == "nazarpes7")
            {
                WebmsW = getWebms(Parent, "http://2ch.hk/b/catalog_num.json");
                WebmsA = getWebms(Parent, "http://2ch.hk/a/catalog_num.json");
                WebmCountW = WebmsW.Count;
                WebmCountA = WebmsA.Count;
                Parent.Client.SendTextMessageAsync(ms.Chat.Id, $"Webms loaded: {WebmsW.Count} normal webms.\nWebms loaded: {WebmsA.Count} anime webms.");
            }
            else Parent.Client.SendTextMessageAsync(ms.Chat.Id, $"You'r not owner of this chat.");
        }

        public void get2chSmartRandWebm(Message ms,Bot Parent)
        {
            List<dynamic> Webms;
            if (ms.Text.Split('-')?[1] != "a")
                Webms = WebmsW;
            else
                Webms = WebmsA;
            if (Webms != null && Webms?.Count > 0)
            {
                dynamic webm = Webms[Parent.rand.Next(0, Webms.Count)];
                Webms.Remove(webm);
                SendWebms(Parent, webm);
            } else Parent.Exceptions.Add(new Exception("No Webms There. User regenerate func."));
        }

        public static void SendWebms(Bot Parent, dynamic webm)
        {
            try
            {
                Thread th = new Thread(async () =>
                {
                    var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                        new [] {
                            new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("👍 0","like"),
                            new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("👎 0","dislike")
                        }
                    });
                    try
                    {
                        await Parent.Client.SendPhotoAsync(Parent.MessagesLast.Last().Chat.Id, new FileToSend(webm.thumbnail), webm.path, false, 0, keyboard);
                    }
                    catch (Exception ex)
                    {
                        Parent.Exceptions.Add(ex);
                        return;
                    }
                });
                th.Start();
            }
            catch
            {

            }
        } 
    }
}
