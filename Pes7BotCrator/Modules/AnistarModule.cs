using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Modules
{
    public class AnistarModule : Module
    {
        /*Нужно сделать модуль который оповещает тебя про выход новых аниме.*/
        public SynkCommand Command { get; set; }
        public AnistarModule () : base("AnistarModule",typeof(AnistarModule))
        {
            Command = new SynkCommand(ParseAsync,new List<string>() { "/animetoday" },descr:"Отображает ангоинги аниме.");
        }

        public void ParseAsync(Message message, IBot Parent, List<ArgC> args)
        {
            var web = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.GetEncoding("windows-1251")
            };
            HtmlDocument html = web.Load("https://anistar.me/raspisanie-vyhoda-seriy-ongoingov.html");
            var dle = html.GetElementbyId("dle-content").ChildNodes.Where(fs => fs.Name == "small").First();
            var news = dle.ChildNodes.Where(fn => fn.HasClass("news-top"));
            var animenow = news.ElementAt(0).ChildNodes.Where(fn => fn.HasClass("top-new")).First().ChildNodes.Where(sn => sn.Name == "div");
            foreach (var an in animenow)
            {
                var anime = an.ChildNodes.Where(gf => gf.Name == "div").First();
                var t = anime.ChildNodes.Where(sf => sf.HasClass("timer_cal")).First();
                string picHref = anime.Attributes["style"].Value.Split('\'').Where(fn => fn.Contains("uploads")).First();
                string time = t.ChildNodes.Where(fs => fs.Name == "smal" || fs.Name == "span").First().InnerText;
                Parent.Client.SendPhotoAsync(Parent.MessagesLast.Last().Chat.Id, new FileToSend(new Uri($"https://anistar.me{picHref}")), time);
            }
        }

        private async Task<bool> getFile(string href, string name, string id)
        {
            if (!Directory.Exists("./Anime"))
                Directory.CreateDirectory("./Anime");
            using (WebClient client = new WebClient())
            {
                
                await client.DownloadFileTaskAsync(new Uri(href), $"./Anime/{id}_{name}");
                return true;
            }
        }
    }
}
