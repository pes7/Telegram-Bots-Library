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
using Pes7BotCrator.Systems;
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
             */
        
        public Bot Bot;
        private int Ms = 20000;
        private int CurTime = 0;
        private static _2chModule Ch = null;
        private static SaveLoadModule Sv = null;
        private string LikePath = AppDomain.CurrentDomain.BaseDirectory + "like.bot";
        //private string loc = $"{AppDomain.CurrentDomain.BaseDirectory}bot.xml";

        public OLua lua; // Ради фана

        public Main()
        {
            InitializeComponent();

            /*
             * Нужно написать модуль автопоста, при том что туда будет попадать кастомная функция, а настройка будет производиться в интерфейсе проги. 
             * 
             */
            Ch = new _2chModule();
            Sv = new SaveLoadModule(60, LikePath, this);
            Bot = new Bot("466088141:AAHIcb1aG8F6P5YQSgcQlqaKJBD9vlLuMAw", "G:/WebServers/home/apirrrsseer.ru/www/List_down/video", "C:/Users/user/Desktop/GachiArch",
                modules: new List<ModuleInterface> {
                    Ch,
                    Sv,
                    new LikeDislikeComponent()
                }
            );

            // Ради фана
            lua = new OLua(Bot);
            //lua.LoadScriptsFromDirectory();
            //

            if (System.IO.File.Exists(LikePath))
            {
                (Bot.GetModule<LikeDislikeComponent>() as LikeDislikeComponent).LLikes = SaveLoadModule.LoadLikesFromFile(LikePath);
            }
            Bot.Commands.Add(new LikeDislikeComponent().Command);
            Bot.Commands.Add(new SynkCommand(new WebmModule().WebmFuncForBot, new List<string>()
            {
                "/sendrandwebm@guchimuchibot",
                "/sendrandwebm"
            }));
            Bot.Commands.Add(new SynkCommand(new BotLogic().GachiAttakSynk, new List<string>()
            {
                "/gachiattak@guchimuchibot",
                "/gachiattak"
            }));
            Bot.Commands.Add(new SynkCommand(new BotLogic().GetGachiImageLogic, new List<string>()
            {
                "/sendrandimg@guchimuchibot",
                "/sendrandimg"
            }));
            Bot.Commands.Add(new SynkCommand(Ch.get2chSmartRandWebm, new List<string>()
            {
                "/2ch@guchimuchibot",
                "/2ch"
            }));
            Bot.Commands.Add(new SynkCommand(Ch.get2chSmartRandWebm, new List<string>()
            {
                "/2ch@guchimuchibot -a",
                "/2ch -a"
            }));
            Bot.Commands.Add(new SynkCommand(Ch.Ragenerated, new List<string>()
            {
                "/regenerate@guchimuchibot",
                "/regenerate"
            }));
            Bot.Commands.Add(new SynkCommand(new BotLogic().GetArgkSynk, new List<string>()
            {
                "/testmemory"
            }));
            Bot.Commands.Add(new SynkCommand(new BotLogic().Oprosic, new List<string>()
            {
                "/opros"
            }));
            Bot.Commands.Add(new SynkCommand(async (InlineQuery query, BotInteface Parent, Update up) => {
                if (Parent.Modules.Exists(fn => fn.Name == "_2chModule"))
                {
                    _2chModule.Webm webm = Parent.GetModule<_2chModule>().WebmsSent.Find(fn => fn.Path == query.Query);
                    if (webm != null)
                    {
                        var msg = new Telegram.Bot.Types.InputMessageContents.InputTextMessageContent
                        {
                            MessageText = $"{webm.Thumbnail}\n{webm.Path}",
                            ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html
                        };

                        Telegram.Bot.Types.InlineQueryResults.InlineQueryResult[] results = {
                                new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle{
                                    Id = "0",
                                    InputMessageContent = msg,
                                    ReplyMarkup = LikeDislikeComponent.getKeyBoard(),
                                    Title = "WEBM",
                                    Description = "POST"
                                }
                        };
                        await Parent.Client.AnswerInlineQueryAsync(query.Id, results);
                    }
                }
            },new List<string>() {"_noon"}));
            Bot.Commands.Add(new SynkCommand(new BotLogic().DefaultSynk, new List<string>()
            {
                "Default"
            }));
            label1.Text = $"{Ms} ms";

            /*
            Thread th = new Thread(async () =>
            {
                while (true)
                {
                    await this.GetInf();
                    Thread.Sleep(1000);
                }
            });
            th.Start();
            */
        }

        private async Task GetInf()
        {
            listBox1.Items.Clear();
            listBox1.Items.AddRange(Bot.getInfForList());
        }

        private void Main_Load(object sender, EventArgs e)
        {
            textBox1.Text = Bot.WebmDir;
            textBox2.Text = Bot.GachiImage;
            textBox3.Text = Bot.PreViewDir;
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
                        if (Bot.Rand.Next(0, 1) == 0)
                        {
                            int rd = Bot.Rand.Next(0, _2chModule.WebmCountW);
                            Ch.SendWebm(Bot,  Ch.WebmsW[rd]);
                            Ch.WebmsW.RemoveAt(rd);
                            _2chModule.WebmCountW = Ch.WebmsW.Count;
                        }
                        else
                        {
                            int rd = Bot.Rand.Next(0, _2chModule.WebmCountA);
                            Ch.SendWebm(Bot, Ch.WebmsA[rd]);
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
            Ch.ParseWebmsFromDvach(Bot);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Bot != null)
            {
                Thread th = new Thread( async() => {
                    try
                    {
                        await Bot.Client.SendTextMessageAsync(Bot.MessagesLast.Last().Chat.Id, "test");
                    }
                    catch (Telegram.Bot.Exceptions.ApiRequestException)
                    {
                        
                    }
                });
                th.Start();
                //Bot.Client.SendTextMessageAsync(chatId: Bot.MessagesLast.Last().Chat.Id,text: "@Pro100RedBull ЛГБТ", replyMarkup: (new ForceReply() { Force = true }));
                //Bot.SendMessage(Bot.MessagesLast.Last().Chat.Id, "Test Kek");
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            LikeDislikeComponent LDModule = (Bot.GetModule<LikeDislikeComponent>() as LikeDislikeComponent);
            if (LDModule.LLikes.Count > 0)
            {
                SaveLoadModule.SaveLikesToFile(LDModule.LLikes, LikePath);
            }
            Bot.Dispose();
            base.OnFormClosed(e);
        }

        private void updateUI()
        {

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
                    InvokeUI(() =>
                    {
                        MessageUI mu = new MessageUI(files.Find(fs => ms.From.Id == fs.id).Image, ms.Text);
                        mu.Width = flowLayoutPanel1.Width - 25;
                        flowLayoutPanel1.Controls.Add(mu);
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
            lua.Lua.DoString(textBox4.Text);
        }
    }
}
