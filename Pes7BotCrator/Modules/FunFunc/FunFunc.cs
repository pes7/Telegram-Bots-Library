using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Modules.FunFunc
{
    public class FunFunc : Module
    {
        public InfTrue _CommandInf { set; get; }
        public ElseElse _CommandElse { get; set; }
        public GuchiName _CommandGuchi { get; set; }
        public WhoAreYou _CommandWhoAreU { get; set; }
        public Triggered _Triggered { get; set; }
        public Random Rand { get; set; }
        public string FaceImageDir { get; set; }
        public string WhoTitles { get; set; }
        public string WhoAnswers { get; set; }
        public string Triggers { get; set; }
        public FunFunc(string imageDir = null, string whoTitles = null, string whoAnswers = null, string trigger = null) : base("FunFunc", typeof(FunFunc))
        {
            _CommandInf = new InfTrue();
            _CommandElse = new ElseElse();
            _CommandGuchi = new GuchiName();
            _CommandWhoAreU = new WhoAreYou();
            _Triggered = new Triggered();
            WhoTitles = whoTitles;
            WhoAnswers = whoAnswers;
            FaceImageDir = imageDir;
            Triggers = trigger;
            if (Directory.Exists("FunPics"))
                Directory.CreateDirectory("FunPics");
            Rand = new Random();
        }
        public class InfTrue : SynkCommand
        {
            public InfTrue() : base(ActInf, new List<string>() { "/inf" }, descr: "Проверить правдивость инфы. `-w` инфа") { }

        }
        public class GuchiName : SynkCommand
        {
            public GuchiName() : base(ActGuchi, new List<string>() { "/guchi" }, descr: "`name` имя пациента") { }
        }
        public class ElseElse : SynkCommand
        {
            public ElseElse() : base(ActElse, new List<string>() { "/else" }, descr: "То или другое `-t1` первое `t2` второе") { }
        }
        public class WhoAreYou : SynkCommand
        {
            public WhoAreYou() : base(ActWho, new List<string>() { "/who" }, descr: "Кто ты такой?") { }
        }
        public class Triggered : SynkCommand
        {
            public Triggered() : base(ActTrig, new List<string>() { "/t" }, descr: "TRIGGERED!") { }
        }
        public static void ActTrig(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null)
            {
                var th = Parent.GetModule<FunFunc>();
                var files = "*.png|*.jpg|*.bmp".Split('|').SelectMany(filter => System.IO.Directory.GetFiles(th.Triggers, filter, SearchOption.AllDirectories)).ToArray();
                Parent.Client.SendPhotoAsync(re.Chat.Id, new FileToSend("trigger", System.IO.File.Open($"{files[th.Rand.Next(0,files.Length)]}", FileMode.Open)));
            }
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
                            if(Parent.GetModule<FunFunc>().Rand.Next(0, 10) <= 5)
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
                    //MemoryStream memoryStream = new MemoryStream();
                    //bmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    int count = Directory.GetFiles("FunPics", "*").Length;
                    bmp.Save($"FunPics/test{count}.jpg");
                    Parent.Client.SendPhotoAsync(re.Chat.Id, new FileToSend("ochincin", System.IO.File.Open($"FunPics/test{count}.jpg",FileMode.Open)));
                }
            }
        }
        public static void ActGuchi(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null)
            {
                var th = Parent.GetModule<FunFunc>();
                var arg1 = args.Find(fn => fn.Name == "name");
                if (arg1?.Arg != null && th.FaceImageDir != null)
                {
                    var files = "*.png|*.jpg|*.bmp".Split('|').SelectMany(filter => System.IO.Directory.GetFiles(th.FaceImageDir, filter, SearchOption.AllDirectories)).ToArray();
                    using (var bmp = new Bitmap(files[th.Rand.Next(0, files.Length)]))
                    {
                        using (Graphics graphics = Graphics.FromImage(bmp))
                        {
                            /*
                            string s = arg1.Arg;
                            Font font = new Font(new FontFamily("Segoe Script"), 25);
                            RectangleF rect = new RectangleF(new Point(bmp.Width / 2 - 30, 20),new SizeF(100,100));
                            StringFormat format = StringFormat.GenericTypographic;
                            float dpi = graphics.DpiY;
                            GraphicsPath path = new GraphicsPath();
                            // Convert font size into appropriate coordinates
                            float emSize = dpi * font.SizeInPoints / 72;
                            path.AddString(s, font.FontFamily, (int)font.Style, emSize, rect, format);
                            graphics.DrawPath(Pens.Black, path);
                            graphics.Save();
                            */

                            var d = new Font(new FontFamily("Segoe Script"), 25);
                            graphics.DrawString(arg1.Arg, d, Brushes.White, bmp.Width / 2 - 30, 20);
                            graphics.Save();

                        }
                        //MemoryStream memoryStream = new MemoryStream();
                        //bmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        int count = Directory.GetFiles("FunPics", "*").Length;
                        bmp.Save($"FunPics/guchi{count}.jpg");
                        Parent.Client.SendPhotoAsync(re.Chat.Id, new FileToSend("ochincin", System.IO.File.Open($"FunPics/guchi{count}.jpg", FileMode.Open)));
                    }
                }
            }
        }
        public static void ActWho(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null)
            {
                var th = Parent.GetModule<FunFunc>();
                //var files = "*.png|*.jpg|*.bmp".Split('|').SelectMany(filter => System.IO.Directory.GetFiles("dir", filter, SearchOption.AllDirectories)).ToArray();
                //using (var bmp = new Bitmap(files[th.Rand.Next(0, files.Length)]))
                //{
                Thread ther = new Thread(async () =>
                {
                    
                    var filesWho = "*.png|*.jpg|*.bmp".Split('|').SelectMany(filter => System.IO.Directory.GetFiles(th.WhoAnswers, filter, SearchOption.AllDirectories)).ToArray();
                    var filesTitles = "*.png|*.jpg|*.bmp".Split('|').SelectMany(filter => System.IO.Directory.GetFiles(th.WhoTitles, filter, SearchOption.AllDirectories)).ToArray();

                    var bmp = Bitmap.FromFile(filesTitles[th.Rand.Next(0, filesTitles.Length)]);
                    var answer = Bitmap.FromFile(filesWho[th.Rand.Next(0, filesWho.Length)]);
                    using (Graphics graphics = Graphics.FromImage(bmp))
                    {
                        UserM us = new UserM(re.From);
                        if (!System.IO.File.Exists($"./UserPhotoes/{re.From.Id}.jpg"))
                        {
                            await us.DownloadImageToDirectory(Parent);
                        }
                        Image mg;
                        try
                        {
                            mg = Image.FromFile($"./UserPhotoes/{us.Id}.jpg");
                            graphics.DrawImage(ResizeImage(mg, 285, 180), 5, 10);
                        }
                        catch { }
                        var d = new Font(new FontFamily("Segoe Script"), 25);
                        graphics.DrawString($"{us.FirstName}", d, Brushes.DeepSkyBlue, 50, 10);
                        graphics.DrawImage(ResizeImage(answer, 400, 270), 300, 220);
                        graphics.Save();
                    }
                    //MemoryStream memoryStream = new MemoryStream();
                    //bmp.Save(memoryStream, ImageFormat.Jpeg);
                    int count = Directory.GetFiles("FunPics", "*").Length;
                    bmp.Save($"FunPics/who{count}.jpg");
                    await Parent.Client.SendPhotoAsync(re.Chat.Id, new FileToSend("ochincin", System.IO.File.Open($"FunPics/who{count}.jpg", FileMode.Open)));
                });
                ther.Start();
            }
        }
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
