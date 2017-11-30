using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Commands;
using Pes7BotCrator.Modules;
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

        public Bot(string key, string webmdir = null, string gachiimage = null, string preViewDir = null, int[] likeDislikeQuata = null, List<ModuleInterface> modules = null) :
            base (key,likeDislikeQuata,modules)
        {
            LikeDislikeComponent LDModule = GetModule<LikeDislikeComponent>() as LikeDislikeComponent;
            if (LDModule.LikeDislikeQuata == null)
                LDModule.LikeDislikeQuata = new int[] { 3, 3 };
            LDModule.LLikes = new List<Likes>();
            LastWebms = new List<dynamic>();
            WebmDir = webmdir;
            GachiImage = gachiimage;
            PreViewDir = preViewDir;
            GenerePreViewDir();
        }

        private void SynkModules()
        {
            foreach (ModuleInterface nd in Modules)
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
        public static async Task ClearCommandAsync(long id, int msgid, BotInteface Parent)
        {
            try
            {
                await Parent.Client.DeleteMessageAsync(id, msgid);
            }catch(Exception ex)
            {
                Parent.Exceptions.Add(ex);
            }
        }

        private void TimeT()
        {
            while (true)
            {
                RunTime++;
                Thread.Sleep(1000);
                BotSynk();
                ShowInf();
            }
        }

        public string[] getInfForList()
        {
            return $"Messages count: {MessagesLast.Count} msgs.|Available messages: {CountOfAvailableMessages}|RunTime: {TimeToString(RunTime)}|Webms Online: {_2chModule.WebmCountW + _2chModule.WebmCountA}|Likes and dislikes: {(GetModule<LikeDislikeComponent>() as LikeDislikeComponent).LLikes.Count}".Split('|');
        }

        public override void ShowInf()
        {
            Console.Clear();
            Console.WriteLine("Bot Stats: {");
            Console.WriteLine($"    Messages count: {MessagesLast.Count} msgs.\n    Available messages: {CountOfAvailableMessages}\n    RunTime: {TimeToString(RunTime)}\n    Webms Online: {_2chModule.WebmCountW + _2chModule.WebmCountA}\n    Likes and dislikes: {(GetModule<LikeDislikeComponent>() as LikeDislikeComponent).LLikes.Count}");
            Console.WriteLine("}");
            Console.WriteLine("Active Users: {");
            foreach (UserM um in ActiveUsers)
            {
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
                foreach (Message ms in MessagesLast)
                {
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
                foreach (Exception ms in Exceptions)
                {
                    Console.WriteLine($"    {ms}");
                }
            }
            Console.WriteLine("}");
        }
    }
}
