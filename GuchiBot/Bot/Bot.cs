using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Commands;
using Pes7BotCrator.Modules;
using Pes7BotCrator.Modules.Types;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Pes7BotCrator
{
    public class Bot : BotBase
    {
        public List<dynamic> LastWebms { get; set; } //Need to done. we need to save names of webms that was posted, and when use regenerate del them. 

        public string WebmDir { get; set; }
        public string GachiImage { get; set; }
        public string PreViewDir { get; set; } //If nun, generated.

        public Bot(string key, string name, string webmdir = null, string gachiimage = null, string preViewDir = null, List<IModule> modules = null) :
            base(key, name, modules)
        {
            LastWebms = new List<dynamic>();
            WebmDir = webmdir;
            GachiImage = gachiimage;
            PreViewDir = preViewDir;
            GenerePreViewDir();
        }

        private void SynkModules()
        {
            foreach (IModule nd in Modules)
            {
                nd.Start();
            }
        }
        private void GenerePreViewDir()
        {
            if (PreViewDir == null)
            {
                if (!Directory.Exists(PreViewDir))
                {
                    Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}/previews/");
                    PreViewDir = $"{AppDomain.CurrentDomain.BaseDirectory}/previews/";
                }
            }
        }
        public static async new Task ClearCommandAsync(long id, int msgid, IBot Parent)
        {
            try
            {
                await Parent.Client.DeleteMessageAsync(id, msgid);
            }
            catch {  /*Parent.Exceptions.Add(ex);*/ }
        }

        private void TimeT()
        {
            while (true)
            {
                RunTime++;
                Thread.Sleep(1000);
                ShowInf();
            }
        }

        public string[] getInfForList()
        {
            return $"Messages count: {MessagesLast.Count} msgs.|RunTime: {TimeToString(RunTime)}|Webms Online: {_2chModule.WebmCountW + _2chModule.WebmCountA}|Likes and dislikes: {GetModule<LikeDislikeModule>().LLikes.Count}".Split('|');
        }

        public override void ShowInf()
        {
            Console.Clear();
            Console.WriteLine("Bot Stats: {");
            Console.WriteLine($"    Messages count: {MessagesLast.Count} msgs.\n    RunTime: {TimeToString(RunTime)}\n    Webms Online: {_2chModule.WebmCountW + _2chModule.WebmCountA}\n    Likes and dislikes: {GetModule<LikeDislikeModule>().LLikes.Count}");
            Console.WriteLine("}");
            Console.WriteLine("Active Users: {");
            for (int i = 0; i < ActiveUsers.Count; i++)
            {
                var um = ActiveUsers[i];
                Console.WriteLine($"    {um.Username} {um.MessageCount} messages.");
            }
            Console.WriteLine("}\nLast 10 Messages: {");
            if (MessagesLast.Count > 10)
            {
                for (int i = MessagesLast.Count - 10; i < MessagesLast.Count; i++)
                {

                    Message ms = MessagesLast[i];
                    Console.WriteLine($"    {UserM.usernameGet(ms.From)}: {ms.Text}");
                }
            }
            else
            {
                for (int i = 0; i < MessagesLast.Count; i++)
                {
                    var ms = MessagesLast[i];
                    Console.WriteLine($"    {ms.From.Username}: {ms.Text}");
                }
            }
            Console.WriteLine("}");
            Console.WriteLine("Exceptions: {");
            if (Exceptions.Count > 10)
            {
                for (int i = Exceptions.Count - 10; i < Exceptions.Count; i++)
                {
                    Console.WriteLine($"    {Exceptions[i]}");
                }
            }
            else
            {
                for (int i = 0; i < Exceptions.Count; i++)
                {
                    var ex = Exceptions[i];
                    Console.WriteLine($"    {ex}");
                }
            }
            Console.WriteLine("}");
        }
    }
}
