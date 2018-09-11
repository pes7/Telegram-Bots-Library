using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace GuchiBot.Modules
{
    class GuchiVoice : Module
    {
        public GVoice _GVoice { set; get; }
        private static List<string> Files;
        public GuchiVoice() : base("GuchiSound", typeof(GuchiVoice))
        {
            _GVoice = new GVoice();
            if (!Directory.Exists("FunFunc/GuchiVoice"))
                Directory.CreateDirectory("FunFunc/GuchiVoice");
            Files = new List<string>();
            Files.AddRange(Directory.GetFiles("FunFunc/GuchiVoice","*.mp3"));
        }
        public class GVoice : SynkCommand
        {
            public GVoice() : base(Act, new List<string>() { "/buss" }, commandName: "бас", descr: "Гачи войсик)", clearcommand: false) { }
        }
        public static void Act(Message re, IBot Parent, List<ArgC> args)
        {
            if (Files.Count > 0)
            {
                var th = new Thread(async () =>
                {
                    var file = System.IO.File.Open(Files[Parent.Rand.Next(0, Files.Count)], FileMode.Open);
                    await Parent.Client.SendVoiceAsync(re.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(file));
                    file.Close();
                });
                th.Start();
            }
        }
    }
}
