﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;
using System.Threading;
using System.Diagnostics;
using Pes7BotCrator.Commands;

namespace Pes7BotCrator.Modules
{
    public class _2chModule : Module
    {
        public _2chModule() : base("_2chModule")
        {
            Modulle = this;
        }

        public static List<dynamic> WebmsSent = new List<dynamic>();

        private List<ThBoard> Get2chBoards(Bot Parent, string address)
        {
            List<ThBoard> Th = new List<ThBoard>();
            dynamic s = ThBoard.GetJson(address);
            foreach (dynamic h in s.threads)
            {
                if (h.num != null && h.files_count != null)
                {
                    string subj = null;
                    try
                    {
                        subj = h.subject;
                    }
                    catch { }
                    ThBoard th = new ThBoard((string)h.num, (string)h.comment, (string)h.date, (string)h.files_count, subj);
                    Th.Add(th);
                }
            }
            return Th;
        }

        public void ParseWebmsFromDvach(Bot Parent)
        {
            DvochSynkAsync(Get2chBoards(Parent, "http://2ch.hk/b/catalog_num.json"),Parent);
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

        private bool FilterThread(ThBoard t)
        {
            string[] Filters = {
                "WEBM",
                "webm",
                "MP4",
                "ЦУЬИ",
                "WebM",
                "WEBM/ЦУЬИ",
                "MP4/WebM-тред"
            };
            bool iser = false;
            foreach (string fl in Filters)
            {
                if (t.Discription.Contains(fl))
                    iser = true;
                if (t.Subject != null)
                    if (t.Subject.Contains(fl))
                        iser = true;
            }
            return iser;
        }

        private List<dynamic> getWebms(Bot Parent, string address)
        {
            List<dynamic> Dy = new List<dynamic>();
            List<ThBoard> Th = Get2chBoards(Parent, address);
            string tag = address.Split('/')[3];
            if (tag != null)
            {
                try
                {
                    foreach (ThBoard t in Th)
                    {
                        if (FilterThread(t))
                        {
                            dynamic s = ThBoard.GetJson($"http://2ch.hk/{tag}/res/{t.Id}.json");
                            foreach (dynamic h in s.threads)
                            {
                                foreach (dynamic c in h.posts)
                                {
                                    foreach (dynamic f in c.files)
                                    {
                                        var file = new { fullname = f.fullname, path = "https://2ch.hk" + f.path, thumbnail = "https://2ch.hk" + f.thumbnail };
                                        string format = ((string)file.path).Split('.')[2];
                                        if (format == "webm" || format == "mp4")
                                        {
                                            Dy.Add(file);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Parent.Exceptions.Add(ex);
                }
            }
            return Dy;
        }

        public static int WebmCountW = 0;
        public static int WebmCountA = 0;
        public List<dynamic> WebmsW = null;
        public List<dynamic> WebmsA = null;
        public void Ragenerated(Message ms, Bot Parent)
        {
            if (ms.From.Username == "nazarpes7")
            {
                if (WebmsA != null)
                    WebmsA.Clear();
                if (WebmsW != null)
                    WebmsW.Clear();
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
            string[] d = ms.Text.Split('-');
            if (d != null && d.Length > 1)
                if (d[1] == "а" || d[1] == "a")
                    Webms = WebmsA;
                else Webms = WebmsW;
            else Webms = WebmsW;

            if (Webms != null && Webms?.Count > 0)
            {
                dynamic webm = Webms[Parent.rand.Next(0, Webms.Count)];
                Webms.Remove(webm);
                SendWebm(Parent, webm, d);
            }
            else
                Parent.Exceptions.Add(new Exception("No Webms There. User regenerate func."));
        }

        /*Be wery careful because we have there unless send if webm is not valid*/
        public void SendWebm(Bot Parent, dynamic webm, string[] d = null)
        {
            if (webm != null)
            {
                Thread th = new Thread(async () =>
                {
                    try
                    {
                        await Parent.Client.SendPhotoAsync(Parent.MessagesLast.Last().Chat.Id, new FileToSend(webm.thumbnail), webm.path, false, 0, LikeDislikeComponent.getKeyBoard(webm.path));
                        WebmsSent.Add(webm);
                    }
                    catch (Exception ex)
                    {
                        await Parent.Client.SendTextMessageAsync(Parent.MessagesLast.Last().Chat.Id,"Sorry, but something went wrong.");
                        return;
                    }
                });
                th.Start();
            }
        }
    }
}
