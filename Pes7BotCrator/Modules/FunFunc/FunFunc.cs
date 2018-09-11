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
using HtmlAgilityPack;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Modules.FunFunc
{
    public class FunFunc : Module
    {
        public List<UserSerializable> Bosses { get; set; }
        public InfTrue _CommandInf { set; get; }
        public ElseElse _CommandElse { get; set; }
        public GuchiName _CommandGuchi { get; set; }
        public WhoAreYou _CommandWhoAreU { get; set; }
        public Triggered _Triggered { get; set; }
        public TrueFalse _TrueFalse { get; set; }
        public DvachRoll _DvachRoll { get; set; }
        public ChtoEto _ChtoEto { get; set; }
        public Otvetka _Otvetka { get; set; }
        public ActiveUsersMosaic _ActiveUsersMosaic { get; set; }
        public Random Rand { get; set; }
        public BossOfTheGym _BossOfTheGym { get; set; }
        public BossOfTheGymSide _BossOfTheGymSide { get; set; }
        public string FaceImageDir { get; set; }
        public string WhoTitles { get; set; }
        public string WhoAnswers { get; set; }
        public string Triggers { get; set; }
        public string FileNameBoss = "Bosses.bot";
        public FunFunc(string imageDir = null, string whoTitles = null, string whoAnswers = null, string trigger = null) : base("FunFunc", typeof(FunFunc))
        {
            _CommandInf = new InfTrue();
            _CommandElse = new ElseElse();
            _CommandGuchi = new GuchiName();
            _CommandWhoAreU = new WhoAreYou();
            _Triggered = new Triggered();
            _ActiveUsersMosaic = new ActiveUsersMosaic();
            _TrueFalse = new TrueFalse();
            _DvachRoll = new DvachRoll();
            _ChtoEto = new ChtoEto();
            _Otvetka = new Otvetka();
            _BossOfTheGym = new BossOfTheGym();
            _BossOfTheGymSide = new BossOfTheGymSide();
            WhoTitles = whoTitles;
            WhoAnswers = whoAnswers;
            FaceImageDir = imageDir;
            Triggers = trigger;
            if (Directory.Exists("FunPics"))
                Directory.CreateDirectory("FunPics");
            Rand = new Random();
            if (System.IO.File.Exists(FileNameBoss))
                Bosses = LoadBosses();
            else
                Bosses = new List<UserSerializable>();
        }
        public class InfTrue : SynkCommand
        {
            public InfTrue() : base(ActInf, new List<string>() { "/inf" }, commandName: "инфа", descr: "Проверить правдивость инфы. `-w` инфа", clearcommand:false) { }
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
        public class TrueFalse : SynkCommand
        {
            public TrueFalse() : base(ActTrueFalse, new List<string>() { "/tf" }, commandName: "правда", descr: "Проверяет инфу на правду или лошь", clearcommand:false) { }
        }
        public class DvachRoll : SynkCommand
        {
            public DvachRoll() : base(RollAct, new List<string>() { "/roll" }, commandName: "ролл", descr: "Старая-добрая руллетка со времён двоща. Праметры: c, min, max") { }
        }
        public class Otvetka : SynkCommand
        {
            public Otvetka() : base(ActTrig) { }
        }
        //https://www.google.com/imghp?source=lnms&tbm=isch
        public class ChtoEto : SynkCommand
        {
            public ChtoEto() : base(ChtoAct, new List<string>() { "/chto" }, commandName: "расскажи", descr: "Гачи найдёт информацию в гугле. Параметры: text, photo или video", clearcommand:false) { }
        }
        public class BossOfTheGym : SynkCommand
        {
            public BossOfTheGym() : base(BossOfTheGymAct, new List<string>() { "/bossclaim" }, commandName: "захват", descr: "Кто же бос этой качалки?", clearcommand: false) { }
        }
        public class BossOfTheGymSide : SynkCommand
        {
            public BossOfTheGymSide() : base(BossOfTheGymActRegistration, new List<string>() { "/boss" }, commandName: "босс", descr: "Сражаться за ринг.", clearcommand: false) { }
        }

        private List<UserSerializable> LoadBosses()
        {
            return SaveLoadModule.LoadSomething<List<UserSerializable>>(FileNameBoss);
        }

        public void SaveBosses()
        {
            var ls = Bosses;
            SaveLoadModule.SaveSomething(ls, FileNameBoss);
        }

        public static void BossOfTheGymActRegistration(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null)
            {
                var fun = Parent.GetModule<FunFunc>();
                var u = fun.Bosses.Find(fn => UserM.usernameGet(fn.GetUser()).Equals(UserM.usernameGet(re.From)));
                if (u != null)
                {
                    if (u.FromChat.Exists(f => f == re.Chat.Id))
                        Parent.Client.SendTextMessageAsync(re.Chat.Id, $"Ты уже и так не ринге @{UserM.usernameGet(re.From)}!");
                    else
                    {
                        u.FromChat.Add(re.Chat.Id);
                        Parent.Client.SendTextMessageAsync(re.Chat.Id, $"Добро пожаловать на ринг @{UserM.usernameGet(re.From)}!");
                    }
                }
                else
                {
                    Parent.Client.SendTextMessageAsync(re.Chat.Id, $"Добро пожаловать на ринг @{UserM.usernameGet(re.From)}!");
                    var us = new UserM(re.From);
                    us.FromChat.Add(re.Chat.Id);
                    fun.Bosses.Add(new UserSerializable(us));
                    fun.SaveBosses();
                }
            }
        }

        public static void BossOfTheGymAct(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null)
            {
                string path = $"Boss_{re.Chat.Id}.bot";
                TimeSpan timeLoss = new TimeSpan(24);
                string name = null;
                var fun = Parent.GetModule<FunFunc>();
                var now_Time = DateTime.Now;
                var id = re.Chat.Id;
                bool AssWeCan = false;
                if (System.IO.File.Exists(path))
                {
                    var strings = System.IO.File.ReadAllText(path);
                    var split = strings.Split('|');
                    name = split[0];
                    string time = split[1];
                    var old = DateTime.Parse(time);
                    var timeLosser = now_Time - old;
                    timeLoss = new TimeSpan(24, 0, 0) - timeLosser;
                    if (timeLoss.TotalMinutes < 0)
                        AssWeCan = true;
                }
                else AssWeCan = true;
                var usersFromThisChat = fun.Bosses.FindAll(fn => fn.FromChat.Any(g => g == id));
                if (AssWeCan && usersFromThisChat.Count > 0)
                {
                    
                    System.IO.File.Delete(path);
                    Parent.Client.SendTextMessageAsync(id,"Ринг разрывется бурными овациями");
                    Thread.Sleep(1000);
                    Parent.Client.SendTextMessageAsync(id, "Все гачимены выходят на ринг");
                    Thread.Sleep(1000);
                    Parent.Client.SendTextMessageAsync(id, "Кто же на этот раз останеться с порваным очком");
                    Thread.Sleep(1000);
                    Parent.Client.SendTextMessageAsync(id, "БОЙ!!!");
                    Thread.Sleep(2000);
                    var winner = usersFromThisChat[Parent.Rand.Next(0, usersFromThisChat.Count)];
                    Parent.Client.SendPhotoAsync(re.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(System.IO.File.Open("boss.png", FileMode.Open), "boss"));
                    Thread.Sleep(500);
                    Parent.Client.SendTextMessageAsync(id, $"Поздравте этого LezerBOY: @{UserM.usernameGet(winner.GetUser())}, теперь он обладатель титула \"Бездонная Дыра\" и абсолютный чемпион качалки на сегодняшний день.");
                    var file = System.IO.File.Create(path);
                    file.Close();
                    System.IO.File.WriteAllText(path, $"{UserM.nameGet(winner.GetUser())}|{now_Time}");
                }
                else
                {
                    Parent.Client.SendTextMessageAsync(re.Chat.Id, $"Энене, Босс качалки уже предопределён: {name} и будет им на протяжении {timeLoss.TotalHours} часов и {timeLoss.Minutes} минут.");
                }
            }
        }

        public static void ActTrig(Update up, IBot Parent, List<ArgC> args)
        {
            if (args == null)
            {
                if(up.Message != null && up.Message?.Text != null)
                {
                    if (up.Message.Text.ToUpper().Contains(Parent.NameString.ToUpper()))
                    {
                        string[] otvetka = {"Да?", "Что?", "Нет","Да","Чо","Что хочешь?","Сдесь","Слушаю","Сам такой" };
                        Parent.Client.SendTextMessageAsync(up.Message.Chat.Id, $"{otvetka[Parent.Rand.Next(0,otvetka.Length)]}", Telegram.Bot.Types.Enums.ParseMode.Default);
                    }
                }
            }
        }

        //БАгнутая Дич, нужно убрать или доработать
        public static void ChtoAct(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null)
            {
                var what = ArgC.GetArg(args, "text", "0");
                var addition = ArgC.GetArg(args, "photo", "1");
                if(addition == null)
                    addition = ArgC.GetArg(args, "video", "1");
                var web = new HtmlWeb();
                //lnms&tbm=isch - photo
                try
                {
                    if (what != null && addition == null)
                    {
                        HtmlDocument html = web.Load($"https://www.google.com/search?biw=1920&bih=525&ei=hEcyW7WVLs6LmgXuu6agDA&q={what.Arg}&oq={what.Arg}&gs_l=psy-ab.12...0.0.0.598.0.0.0.0.0.0.0.0..0.0....0...1c..64.psy-ab..0.0.0....0.qKc87Pgh1a4");
                        var d = html.GetElementbyId("search");
                        var c = d.ChildNodes.Where(gn => gn.Name == "div").First().ChildNodes.Where(gn => gn.Name == "ol").First().
                            ChildNodes.Where(gn => gn.Name == "div").First().ChildNodes.Where(gn => gn.Name == "div").First().
                            ChildNodes.Where(gn => gn.Name == "span").First().InnerText;
                        Parent.Client.SendTextMessageAsync(re.Chat.Id, $"{c}", Telegram.Bot.Types.Enums.ParseMode.Html);
                        /*
                        var c = d.ChildNodes.Where(gn => gn.Name == "div").First(). //DATA-VED
                                ChildNodes.Where(gn => gn.Name == "div").First(). //data-async
                                ChildNodes.Where(gn => gn.Name == "div").First(). //eid
                                ChildNodes.Where(gn => gn.Name == "div").First(). //bkWMgd
                                ChildNodes.Where(gn => gn.Name == "div").First().   //srg
                                ChildNodes.Where(gn => gn.Name == "div").First(); //g
                        var g = c.ChildNodes.Where(gn => gn.Name == "div").First(). //data-hveid
                                ChildNodes.Where(gn => gn.Name == "div").First(). //class=rc
                                ChildNodes.Where(gn => gn.Name == "h3").First(). //class=r
                                ChildNodes.Where(gn => gn.Name == "a").First().InnerText; //a href
                        */
                        //var k = g.ChildNodes;
                        /*
                        var h = g.Where(sb => sb.Name == "a");
                        var e = h.First().InnerText;
                        */
                    } else if (what != null && addition != null)
                    {
                        //недоделано
                        if (addition.Arg.Contains("фото") || addition.Name.Contains("photo"))
                        {
                            HtmlDocument html = web.Load($"https://www.google.com/search?q={what.Arg}&source=lnms&tbm=isch&sa=X&ved=0ahUKEwj75PLKivbbAhXGWywKHbukCQoQ_AUICigB&biw=1920&bih=974");
                            var d = html.GetElementbyId("ires");
                            var f = d.ChildNodes.Where(gn => gn.Name == "table").First().ChildNodes.Where(gn => gn.Name == "tr").First().ChildNodes.Where(gn => gn.Name == "td").First();
                            var c = f.ChildNodes.First().Attributes.Where(fb => fb.Name == "href").First().Value;
                            var sf = $"https://www.google.com{c}";
                            Parent.Client.SendTextMessageAsync(re.Chat.Id, $"{sf}", Telegram.Bot.Types.Enums.ParseMode.Html);
                        } else {

                        }
                    }
                }
                catch { }
            }
        }

        public static void RollAct(Message re, IBot Parent, List<ArgC> args)
        {

            if (re != null)
            {
                ArgC arg = null, arg1 = null, arg2 = null;
                string username = re.From.Username.Replace("_", "[_]");
                if (args != null)
                {
                    arg1 = ArgC.GetArg(args, "min", "0");
                    arg2 = ArgC.GetArg(args, "max", "1");
                    if (arg1 == null || arg2 == null)
                        arg = ArgC.GetArg(args, "c", "0");
                }
                if (arg1 == null || arg2 == null)
                {
                    if (arg?.Arg == null)
                    {
                        var d = Parent.Rand.Next(0, 1000);
                        var s = Parent.Rand.Next(0, 9);
                        if (d < 150 && d > 50)
                            d = int.Parse($"{s}{s}");
                        if (d < 50 && d > 7)
                            d = int.Parse($"{s}{s}{s}");
                        if (d < 7)
                            d = int.Parse($"{s}{s}{s}{s}");
                        if (d < 3)
                            d = int.Parse($"1488");
                        Parent.Client.SendTextMessageAsync(re.Chat.Id, $"@{username}: `{d}`", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                    else
                    {
                        int c = -1;
                        int.TryParse(arg.Arg, out c);
                        if (c > 0 && c < 7)
                        {
                            string k = "9";
                            int h = 1;
                            while (h != c)
                            {
                                k = $"{k}9";
                                h++;
                            }
                            var d = Parent.Rand.Next(0, int.Parse(k));
                            Parent.Client.SendTextMessageAsync(re.Chat.Id, $"@{username}: `{d}`", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        }
                        else
                        {
                            Parent.GetModule<TRM>().SendTimeRelayMessageAsynkAsync(re.Chat.Id, $"@{re.From.Username} ты что дурак?", 10);
                        }
                    }
                }
                else
                {
                    int min = -1, max = -1;
                    int.TryParse(arg1.Arg, out min);
                    int.TryParse(arg2.Arg, out max);
                    if (min >= 0 && max >= 0 && min < max) {
                        var d = Parent.Rand.Next(min, max);
                        Parent.Client.SendTextMessageAsync(re.Chat.Id, $"@{username}: `{d}`", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                    else
                    {
                        Parent.GetModule<TRM>().SendTimeRelayMessageAsynkAsync(re.Chat.Id, $"@{re.From.Username} ты что дурак?", 10);
                    }
                }
            }
        }

        public static void ActTrig(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null)
            {
                var th = Parent.GetModule<FunFunc>();
                var files = "*.png|*.jpg|*.bmp".Split('|').SelectMany(filter => System.IO.Directory.GetFiles(th.Triggers, filter, SearchOption.AllDirectories)).ToArray();
                Parent.Client.SendPhotoAsync(re.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(System.IO.File.Open($"{files[th.Rand.Next(0, files.Length)]}", FileMode.Open), "trigger"));
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
                var arg = ArgC.GetArg(args, "w", "0");
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
                var arg = ArgC.GetArg(args, "w", "0");
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
                var arg1 = ArgC.GetArg(args, "t1", "0");
                var arg2 = ArgC.GetArg(args, "t2", "1");
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
                        Parent.Client.SendPhotoAsync(re.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(System.IO.File.Open(dir, FileMode.Open), "ochincin"));
                    }
                    else
                    {
                        Parent.Client.SendTextMessageAsync(re.Chat.Id, $"Ты @{re.From.Username} что ?");
                    }
                }
            }
        }
        public static void ActGuchi(Message re, IBot Parent, List<ArgC> args)
        {
            if (re != null && args != null)
            {
                var th = Parent.GetModule<FunFunc>();
                var arg1 = ArgC.GetArg(args, "name", "0");
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
                        Parent.Client.SendPhotoAsync(re.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(System.IO.File.Open(dir, FileMode.Open), "ochincin"));
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
                    await Parent.Client.SendPhotoAsync(re.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(System.IO.File.Open(dir, FileMode.Open), "ochincin"));
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
