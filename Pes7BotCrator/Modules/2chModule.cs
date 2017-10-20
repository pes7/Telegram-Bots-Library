using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;
using System.Threading;

namespace Pes7BotCrator.Modules
{
    public class _2chModule
    {
        private List<ThBoard> Get2chBoards(Bot Parent)
        {
            List<ThBoard> Th = new List<ThBoard>();
            dynamic s = ThBoard.GetJson("http://2ch.hk/b/catalog_num.json");
            foreach (dynamic h in s.threads)
            {
                if (h.num != null && h.files_count != null)
                {
                    ThBoard th = new ThBoard((string)h.num, (string)h.comment, (string)h.date, (string)h.files_count);
                    Th.Add(th);
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

        private List<dynamic> getWebms(Bot Parent)
        {
            List<dynamic> Dy = new List<dynamic>();
            List<ThBoard> Th = Get2chBoards(Parent);
            foreach (ThBoard t in Th)
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
                                    Dy.Add(file);
                                }
                            }
                        }
                    }
                }
            }
            return Dy;
        }

        private List<dynamic> Webms;
        public void Ragenerated(Message ms, Bot Parent)
        {
            Webms = getWebms(Parent);
        }

        public void get2chSmartRandWebm(Message ms,Bot Parent)
        {
            if (Webms != null && Webms.Count > 0)
            {
                dynamic webm = Webms[Parent.rand.Next(0, Webms.Count)];
                Webms.Remove(webm);
                Thread th = new Thread(async () =>
                {
                    await Parent.Client.SendPhotoAsync(Parent.MessagesLast.Last().Chat.Id, new FileToSend(webm.thumbnail), webm.path);
                });
                th.Start();
            }
            else
                Parent.Exceptions.Add(new Exception("No Webms There. User regenerate func."));
        } 
    }
}
