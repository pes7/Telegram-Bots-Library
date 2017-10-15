using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pes7BotCrator.Type;

namespace Pes7BotCrator.Modules
{
    public class _2chModule
    {
        public void ParseWebmsFromDvach(Bot Parent)
        {
            List<ThBoard> Th = new List<ThBoard>();
            dynamic s = ThBoard.GetJson("http://2ch.hk/b/catalog_num.json");
            foreach (dynamic h in s.threads)
            {
                if (h.num != null && h.files_count != null)
                {
                    ThBoard th = new ThBoard((string)h.num, (string)h.comment, (string)h.date, (string)h.files_count);
                    Th.Add(th);
                    //Console.WriteLine(Th[Th.Count-1]);
                }
            }
            DvochSynkAsync(Th, Parent);
        }

        public async Task DvochSynkAsync(List<ThBoard> th, Bot Parent)
        {
            /* StreamWriter writetext = new StreamWriter("simple.txt");
            writetext.WriteLine(file.path);*/
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
                                    await Parent.Client.SendTextMessageAsync(Parent.MessagesLast.Last().Chat.Id, file.thumbnail);
                                    await Parent.Client.SendTextMessageAsync(Parent.MessagesLast.Last().Chat.Id, file.path);
                                }
                            }
                        }
                    }
                }
                /*
                if (t.Discription.Contains("WEBM") || t.Discription.Contains("webm"))
                {
                    dynamic s = ThBoard.GetJson($"http://2ch.hk/b/res/{t.Id}.json");
                    Console.WriteLine($"Loaded http://2ch.hk/b/res/{t.Id}.json");
                    foreach (dynamic h in s.threads)
                    {
                        foreach (dynamic c in h)
                        {
                            foreach (dynamic f in c)
                            {
                                foreach (JObject d in (JArray)f)
                                {
                                    Console.WriteLine($"{d}");
                                    var file = new { fullname = f.fullname, path = "https://2ch.hk" + f.path, thumbnail = "https://2ch.hk" + f.thumbnail };
                                    string format = ((string)file.path).Split('.')[2];
                                    if (format == "webm")
                                    {
                                        Console.WriteLine($"{format}|{file.fullname}|{file.path}|{file.thumbnail}");
                                        await Bot.Client.SendTextMessageAsync(Bot.Messages.Last().Chat.Id, file.thumbnail);
                                        await Bot.Client.SendTextMessageAsync(Bot.Messages.Last().Chat.Id, file.path);
                                    }
                                }
                            }
                        }
                    }
                }
                */
            }
        }
    }
}
