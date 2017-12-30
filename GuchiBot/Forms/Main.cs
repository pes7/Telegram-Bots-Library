using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Pes7BotCrator;
using Pes7BotCrator.Modules;
using Pes7BotCrator.Type;
using System.Threading;
using Pes7BotCrator.Commands;
using Telegram.Bot.Types.ReplyMarkups;
using LuaAble;
using System.Diagnostics;
using System.Collections;
using GuchiBot.Interface;
using Telegram.Bot.Types;

namespace GuchiBot
{
    public partial class Main : Form
    {
        /*
         Долепить вывод инфы в ListView, туда также передать картинки юзверей что чатяться.
         Дописать сохранение сообщений и ошибков в отделые папки и файлы
        */
        
        public Bot Bot;
        private int Ms = 30000;
        private int CurTime = 0;

        public OLua lua; // Ради фана

        public static string PostToId = "@guchithread";

        public Main()
        {
            InitializeComponent();

            BotLogic bt = new BotLogic();

            /*
             * Нужно написать модуль автопоста, при том что туда будет попадать кастомная функция, а настройка будет производиться в интерфейсе проги. 
             * В опрос надо добавить параметрический ввод своего текста кнопок. Так же сделать не анонимное голосование.
             */
            Bot = new Bot(
                key: "466088141:AAHIcb1aG8F6P5YQSgcQlqaKJBD9vlLuMAw",
                name: "guchimuchibot",
                webmdir: "G:/WebServers/home/apirrrsseer.ru/www/List_down/video",
                gachiimage: "C:/Users/user/Desktop/GachiArch",
                modules: new List<IModule> {
                    new _2chModule(),
                    new SaveLoadModule(60,10*60),
                    new LikeDislikeModule("./like.bot"),
                    new VoteModule("./votes.bot","./voteslike.bot"),
                    new TransitFileModule("./Downloads"),
                    new AnistarModule(),
                    new Statistic(),
                    new TRM()
                }
            );

            // Команды с строки UI
            lua = new OLua(Bot);

            Bot.SynkCommands.Add(Bot.GetModule<LikeDislikeModule>().Command);
            Bot.SynkCommands.Add(new Pes7BotCrator.Commands.Help(Bot));
            Bot.SynkCommands.Add(Bot.GetModule<Statistic>().CommandHelp);
            Bot.SynkCommands.Add(Bot.GetModule<Statistic>().CommandRuntime);
            Bot.SynkCommands.Add(new LogUlog(Bot,"Приветсвую тебя на нашем ГачиКанале.", "Увы, наши пути расходятся..."));
            Bot.SynkCommands.Add(Bot.GetModule<VoteModule>().QueryCommand);
            Bot.SynkCommands.Add(Bot.GetModule<VoteModule>().CreateCommand);
            Bot.SynkCommands.Add(Bot.GetModule<TransitFileModule>().DownloadCommandSynk);
            Bot.SynkCommands.Add(new SynkCommand(new WebmModule().WebmFuncForBot, new List<string>()
            {
                "/sendrandwebm"
            },descr:"Webm с личной колекции."));
            Bot.SynkCommands.Add(new SynkCommand(bt.GachiAttakSynk, new List<string>()
            {
                "/gachiattak"
            },descr:"Секретное оружие."));
            Bot.SynkCommands.Add(new SynkCommand(bt.GetGachiImageLogic, new List<string>()
            {
                "/sendrandimg"
            },descr:"Пикча с личной колекции"));
            Bot.SynkCommands.Add(new SynkCommand(Bot.GetModule<_2chModule>().get2chSmartRandWebm, new List<string>()
            {
                "/2ch"
            },descr:"Пост webm в тред, Argc: `-a` если хотите аниме. `-c:` количество"));
            Bot.SynkCommands.Add(new SynkCommand(Bot.GetModule<_2chModule>().Ragenerated, new List<string>()
            {
                "/regenerate"
            },descr:"Перепарсить двач."));
            Bot.SynkCommands.Add(new SynkCommand(bt.GetArgkSynk, new List<string>()
            {
                "/testmemory"
            },descr: "Бот повторит за вами."));
            Bot.SynkCommands.Add(new SynkCommand(bt.ArgMessage, new List<string>()
            {
                "/testparam"
            },descr:"Выведет сообщение ботом с праметрами `-id` `-text`"));
            //Inline
            Bot.SynkCommands.Add(new SynkCommand(bt.InlineMenu,new List<string>() {"_noon"}));
            Bot.SynkCommands.Add(new SynkCommand(bt.AutoDelMessage, new List<string>()
            {
                "/adp"
            }, "Auto deliting post `text` `time` - time of life."));
            Bot.SynkCommands.Add(Bot.GetModule<AnistarModule>().Command);
            Bot.SynkCommands.Add(new SynkCommand(bt.DefaultSynk, new List<string>()
            {
                "Default"
            }));
            //Example of TimeReley Photo Message. Ps: special for Mordvinov B.
            Bot.SynkCommands.Add(new SynkCommand(async (Telegram.Bot.Types.Message ms, IBot parent, List<ArgC> args) =>
            {
                try
                {
                    Stream st = System.IO.File.Open("./previews/14637724531700.webm.jpg", FileMode.Open);
                    await parent.GetModule<TRM>().SendTimeRelayPhotoAsynkAsync(parent.MessagesLast.First().Chat.Id, new FileToSend("fl.jpg", st), 10, "KEKUS");
                }
                catch { await parent.GetModule<TRM>().SendTimeRelayMessageAsynkAsync(ms.Chat.Id,"Error to send simple .jpg",10); }
            }, new List<string>() { "/kek" }));
            label1.Text = $"{Ms} ms";

            Bot.GetModule<SaveLoadModule>().SaveActions.Add(Bot.GetModule<LikeDislikeModule>().Save);
            Bot.GetModule<SaveLoadModule>().SaveActions.Add(Bot.GetModule<VoteModule>().Save);
        }

        private async Task GetInf()
        {
            listBox1.Items.Clear();
            listBox1.Items.AddRange(Bot.getInfForList());
        }

        private bool Triger_Alife = true;
        private void Main_Load(object sender, EventArgs e)
        {
            textBox1.Text = Bot.WebmDir;
            textBox2.Text = Bot.GachiImage;
            textBox3.Text = Bot.PreViewDir;

            Thread timeTh = new Thread(() =>
            {
                while (Triger_Alife)
                {
                    InvokeUI(() => { GetInf(); });
                    Thread.Sleep(1000);
                }
            });
            timeTh.Start();
            label3.Text = $"Trafic to: {PostToId}";
        }


        private bool TimerTrigger = false;
        private void TimeMinus_Click(object sender, EventArgs e)
        {
            if (!TimerTrigger && Ms > 0)
            {
                Ms -= 1000;
                label1.Text = $"{Ms} ms";
            }
        }

        private void TimePlus_Click(object sender, EventArgs e)
        {
            if (!TimerTrigger)
            {
                Ms += 1000;
                label1.Text = $"{Ms} ms";
            }
        }

        private void TimeStart_Click(object sender, EventArgs e)
        {
            CurTime = 0;
            timer1.Start();
            pictureBox1.BackColor = Color.Green;
            label1.Text = $"{CurTime} sec";
            TimerTrigger = true;
        }

        private void TimePause_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            pictureBox1.BackColor = Color.Gray;
            label1.Text = $"{CurTime} sec";
        }

        private void TimeStop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            pictureBox1.BackColor = Color.Red;
            label1.Text = $"{Ms} ms";
            TimerTrigger = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CurTime++;
            TimerSynk();
            label1.Text = $"{CurTime} sec";
        }

        private void TimerSynk()
        {
            if (Bot != null)
            {
                if (CurTime >= Ms/1000)
                {
                    if (_2chModule.WebmCountA > 0 && _2chModule.WebmCountW > 0)
                    {
                        _2chModule Ch = Bot.GetModule<_2chModule>();
                        if (!checkBox1.Checked) {
                            int rd = Bot.Rand.Next(0, _2chModule.WebmCountW);
                            Ch.SendWebm(Bot,  Ch.WebmsW[rd], PostToId);
                            Ch.WebmsW.RemoveAt(rd);
                            _2chModule.WebmCountW = Ch.WebmsW.Count;
                        }
                        else
                        {
                            int rd = Bot.Rand.Next(0, _2chModule.WebmCountA);
                            Ch.SendWebm(Bot, Ch.WebmsA[rd], PostToId);
                            Ch.WebmsA.RemoveAt(rd);
                            _2chModule.WebmCountA = Ch.WebmsA.Count;
                        }
                    }
                    CurTime = 0;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Bot.WebmDir = textBox1.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bot.GetModule<_2chModule>().ParseWebmsFromDvach(Bot);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Triger_Alife = false;
            Bot.GetModule<SaveLoadModule>().saveIt();
            Bot.Dispose();
            base.OnFormClosed(e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<dynamic> files = new List<dynamic>();
            List<Thread> downloadTh = new List<Thread>();

            Thread th = new Thread(async () =>
            {
                foreach (UserM us in Bot.ActiveUsers)
                {
                    await us.DownloadImageToDirectory(Bot);
                    Image mg = Image.FromFile($"./UserPhotoes/{us.Id}.jpg");
                    files.Add(new { id = us.Id, Image = mg });
                }

                foreach (Telegram.Bot.Types.Message ms in Bot.MessagesLast)
                {
                    var afs = await Bot.getFileFrom(ms.Photo.ToList().First().FileId);
                    InvokeUI(() =>
                    {
                        if (ms.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
                        {
                            MessageUI mu = new MessageUI(files.Find(fs => ms.From.Id == fs.id).Image, ms.Text)
                            {
                                Width = flowLayoutPanel1.Width - 25
                            };
                            flowLayoutPanel1.Controls.Add(mu);
                        }
                        
                        else if (ms.Type == Telegram.Bot.Types.Enums.MessageType.PhotoMessage)
                        {
                            MessageUIPhoto mu = new MessageUIPhoto(Image.FromStream(afs));
                            mu.Width = flowLayoutPanel1.Width - 25;
                            flowLayoutPanel1.Controls.Add(mu);
                        }
                        
                    });
                }
            });
            th.Start();
        }

        private void InvokeUI(Action a)
        {
            BeginInvoke(new MethodInvoker(a));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(Bot != null)
                Bot.Exceptions.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                lua.Lua.DoString(textBox4.Text);
            }
            catch { }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
