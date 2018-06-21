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
        public TrueFalse _TrueFalse { get; set; }
        public ActiveUsersMosaic _ActiveUsersMosaic { get; set; }
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
            _ActiveUsersMosaic = new ActiveUsersMosaic();
            _TrueFalse = new TrueFalse();
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
            public InfTrue() : base(ActInf, new List<string>() { "/inf" }, commandName: "инфа", descr: "Проверить правдивость инфы. `-w` инфа") { }
        }
        public class GuchiName : SynkCommand
        {
            public GuchiName() : base(ActGuchi, new List<string>() { "/guchi" }, commandName: "аватар", descr: "`name` имя пациента") { }
        }
        public class ElseElse : SynkCommand
        {
            public ElseElse() : base(ActElse, new List<string>() { "/else" }, commandName: "сравни", descr: "То или другое `-t1` первое `t2` второе") { }
        }
        public class WhoAreYou : SynkCommand
        {
            public WhoAreYou() : base(ActWho, new List<string>() { "/who" }, commandName: "тестик", descr: "Кто ты такой?") { }
        }
        public class ActiveUsersMosaic : SynkCommand
        {
            public ActiveUsersMosaic() : base(ActMosaic, new List<string>() { "/mos" }, commandName: "актив", descr: "А какой у нас тут актив?") { }
        }
        public class Triggered : SynkCommand
        {
            public Triggered() : base(ActTrig, new List<string>() { "/t" }, commandName: "триггер", descr: "TRIGGERED!") { }
        }
        public class TrueFalse : SynkCommand {
            public TrueFalse() : base(ActTrueFalse, new List<string>() { "/tf" }, commandName: "правда", descr: "Проверяет инфу на правду или лошь") { }
        }

        public static void ActTrig(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null)
            {
                var th = Parent.GetModule<FunFunc>();
                var files = "*.png|*.jpg|*.bmp".Split('|').SelectMany(filter => System.IO.Directory.GetFiles(th.Triggers, filter, SearchOption.AllDirectories)).ToArray();
                Parent.Client.SendPhotoAsync(re.Chat.Id, new FileToSend("trigger", System.IO.File.Open($"{files[th.Rand.Next(0, files.Length)]}", FileMode.Open)));
            }
        }

        public static void ActMosaic(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null)
            {
                Thread tg = new Thread(() =>
                {
                    var files = Directory.GetFiles("UserPhotoes","*");
                    List<Image> ActiveImage = new List<Image>();
                    foreach (var im in files)
                    {
                        ActiveImage.Add(new Bitmap(System.IO.File.Open(im, FileMode.Open)));
                    }
                    /*
                    Nullable<int> max_x = null, max_y = null, min_x = null, min_y = null;
                    foreach(var im in ActiveImage)
                    {
                        if(max_x == null && max_y == null)
                        {
                            max_x = im.Size.Width;
                            max_y = im.Size.Height;
                        }
                        if(min_x == null && min_y == null)
                    }
                    */
                    //160,160
                    var all = new Bitmap(500,500);
                    int last_x = 0, last_y = 0;
                    int width = 120, height = 120;
                    using (Graphics graphics = Graphics.FromImage(all))
                    {
                        foreach (var im in ActiveImage)
                        {
                            if ((last_x + width) > 500)
                            {
                                last_y += height;
                                last_x = 0;
                            }
                            graphics.DrawImage(ResizeImageWithAspet(im, width, height, im.Size.Width,im.Size.Height), new PointF(last_x, last_y));
                            last_x += width;
                        }
                    }
                    all.Save("kek.jpg");
                    /*
                    var th = Parent.GetModule<FunFunc>();
                    var ActiveUsers = Parent.ActiveUsers;
                    for (int i = 0; i < ActiveUsers.Count; i++)
                    {
                        var user = ActiveUsers[i];
                        List<Image> ActiveImage = new List<Image>();
                        if (!System.IO.File.Exists($"./UserPhotoes/{user.Id}.jpg"))
                        {
                            await user.DownloadImageToDirectory(Parent);
                            ActiveImage.Add(new Bitmap(System.IO.File.Open($"./UserPhotoes/{user.Id}.jpg",FileMode.Open)));
                        }
                        //500 x 500
                    }
                    */
                });
                tg.Start();
            }
        }
        public static void ActTrueFalse(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null && args != null)
            {
                var w = args.Find(fn => fn.Name == "w");
                var arg = w != null ? w : args.Find(fn => fn.Name == "0");
                if (arg != null) {
                    var th = Parent.GetModule<FunFunc>();
                    var tr = th.Rand.Next(0, 100) <= 50 ? true : false;
                    string txt;
                    if (tr)
                        txt = $"{Parent.NameString} сказал что, \"{arg.Arg}\" правда!";
                    else
                        txt = $"{Parent.NameString} сказал что, \"{arg.Arg}\" ложь!";
                    Parent.Client.SendTextMessageAsync(re.Chat.Id, txt);
                }
            }
        }
        public static void ActInf(Message re, IBot Parent, List<ArgC> args)
        {
            if(re != null && args != null)
            {
                var g = args.Find(fn => fn.Name == "w");
                var arg = g == null ? args.Find(fn => fn.Name == "0") : g;
                if (arg?.Arg != null)
                {
                    Parent.Client.SendTextMessageAsync(re.Chat.Id,$"{arg.Arg}: {Parent.GetModule<FunFunc>().Rand.Next(0,100)}%");
                }
            }
        }
        public static void ActElse(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null && args != null)
            {
                var g = args.Find(fn => fn.Name == "t1");
                var arg1 = g == null ? args.Find(fn=>fn.Name=="0") : g;
                var c = args.Find(fn => fn.Name == "t2");
                var arg2 = c == null ? args.Find(fn => fn.Name == "1") : c;
                if (arg1?.Arg != null && arg2?.Arg != null)
                {
                    if (arg1.Arg.Trim().ToUpper() != arg2.Arg.Trim().ToUpper())
                    {
                        var bmp = new Bitmap(FunRes.bomg);
                        var th = Parent.GetModule<FunFunc>();
                        using (Graphics graphics = Graphics.FromImage(bmp))
                        {
                            using (Font arialFont = new Font("Arial", 10))
                            {
                                string text1, text2;
                                if (th.Rand.Next(0, 100) <= 50)
                                {
                                    text1 = arg1.Arg;
                                    text2 = arg2.Arg;
                                }
                                else
                                {
                                    text1 = arg2.Arg;
                                    text2 = arg1.Arg;
                                }
                                var d = new Font(FontFamily.Families[0], 20f, FontStyle.Regular);
                                graphics.DrawString(text1, d, Brushes.Blue, 30, 20);
                                graphics.DrawString(text2, d, Brushes.Red, 350, 20);
                                graphics.Save();
                            }
                        }
                        string dir = SaveIt(re.From.Id, "test", bmp, th);
                        Parent.Client.SendPhotoAsync(re.Chat.Id, new FileToSend("ochincin", System.IO.File.Open(dir, FileMode.Open)));
                    }
                    else
                    {
                        Parent.Client.SendTextMessageAsync(re.Chat.Id, $"Ты @{re.From.Username} что дурак?");
                    }
                }
            }
        }
        public static void ActGuchi(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null && args != null)
            {
                var th = Parent.GetModule<FunFunc>();
                var g = args.Find(fn => fn.Name == "name");
                var arg1 = g == null ? args.Find(fn => fn.Name == "0") : g;
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
                        string dir = SaveIt(re.From.Id,"guchi", bmp, th);
                        Parent.Client.SendPhotoAsync(re.Chat.Id, new FileToSend("ochincin", System.IO.File.Open(dir, FileMode.Open)));
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
                    string dir = SaveIt(re.From.Id,"who",bmp,th);
                    await Parent.Client.SendPhotoAsync(re.Chat.Id, new FileToSend("ochincin", System.IO.File.Open(dir, FileMode.Open)));
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

        public static Image ResizeImageWithAspet(Image image,
                     /* note changed names */
                     int canvasWidth, int canvasHeight,
                     /* new */
                     int originalWidth, int originalHeight)
        {
            System.Drawing.Image thumbnail =
                new Bitmap(canvasWidth, canvasHeight); // changed parm names
            System.Drawing.Graphics graphic =
                         System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            graphic.Clear(Color.White); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            /* ------------- end new code ---------------- */

            System.Drawing.Imaging.ImageCodecInfo[] info =
                             ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality,
                             100L);
            return thumbnail;
        }

        public static string SaveIt(int id, string prefix, Image what, FunFunc Parent)
        {
            int rander = 0;
            if (!Directory.Exists($"FunPics/{id}"))
                Directory.CreateDirectory($"FunPics/{id}");
            do
            {
                rander = Parent.Rand.Next(0, 999999);
            } while (System.IO.File.Exists($"FunPics/{id}/{prefix}_{rander}.jpg"));
            try
            {
                string savedir = $"FunPics/{id}/{prefix}_{rander}.jpg";
                what.Save(savedir);
                return savedir;
            }
            catch
            {
                return null;
            }
        }
    }
}
