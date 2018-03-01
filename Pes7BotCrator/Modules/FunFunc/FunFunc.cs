using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Modules.FunFunc
{
    public class FunFunc : Module
    {
        public InfTrue _CommandInf { set; get; }
        public ElseElse _CommandElse { get; set; }
        public Random Rand { get; set; }
        public FunFunc() : base("FunFunc", typeof(FunFunc))
        {
            _CommandInf = new InfTrue();
            _CommandElse = new ElseElse();
            Rand = new Random();
        }
        public class InfTrue : SynkCommand
        {
            public InfTrue() : base(ActInf, new List<string>() { "/inf" }, descr: "Проверить правдивость инфы. `-w` инфа") { }

        }
        public class ElseElse : SynkCommand
        {
            public ElseElse() : base(ActElse, new List<string>() { "/else" }, descr: "То или другое `-t1` первое `t2` второе") { }
        }
        public static void ActInf(Message re, IBot Parent, List<ArgC> args)
        {
            if(re != null)
            {
                var arg = args.Find(fn => fn.Name == "w");
                if (arg?.Arg != null)
                {
                    Parent.Client.SendTextMessageAsync(re.Chat.Id,$"{arg.Arg}: {Parent.GetModule<FunFunc>().Rand.Next(0,100)}%");
                }
            }
        }
        public static void ActElse(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null)
            {
                var arg1 = args.Find(fn => fn.Name == "t1");
                var arg2 = args.Find(fn => fn.Name == "t2");
                if (arg1?.Arg != null && arg1?.Arg != null)
                {
                    var bmp = new Bitmap(FunRes.bomg);
                    using (Graphics graphics = Graphics.FromImage(bmp))
                    {
                        using (Font arialFont = new Font("Arial", 10))
                        {
                            string text1, text2;
                            if(Parent.GetModule<FunFunc>().Rand.Next(0, 10) <= 0)
                            {
                                text1 = arg1.Arg;
                                text2 = arg2.Arg;
                            }
                            else
                            {
                                text1 = arg2.Arg;
                                text2 = arg1.Arg;
                            }
                            var d = new Font(FontFamily.Families[0],20f,FontStyle.Regular);
                            graphics.DrawString(text1, d, Brushes.Blue, 30,20);
                            graphics.DrawString(text2, d, Brushes.Red, 350,20);
                            graphics.Save();
                        }
                    }
                    MemoryStream memoryStream = new MemoryStream();
                    bmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    bmp.Save("test.jpg");
                    Parent.Client.SendPhotoAsync(re.Chat.Id, new FileToSend("ochincin", System.IO.File.Open("test.jpg",FileMode.Open)),"else");
                }
            }
        }
    }
}
