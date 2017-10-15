using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuchiBot
{
    public partial class Main : Form
    {
        private Bot Bot;
        private int Ms = 20000;
        private int CurTime = 0;

        public Main()
        {
            InitializeComponent();
            Bot = new Bot("466088141:AAHIcb1aG8F6P5YQSgcQlqaKJBD9vlLuMAw", "F:/Webm");
            Bot.Commands.Add(new SynkCommand(new WebmModule().WebmFuncForBot,new List<string>()
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
            Bot.Commands.Add(new SynkCommand(new BotLogic().GetArgkSynk, new List<string>()
            {
                "/testmemory"
            }));
            Bot.Commands.Add(new SynkCommand(new BotLogic().DefaultSynk, new List<string>()
            {
                "default"
            }));
            label1.Text = $"{Ms} ms";
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
            if (!TimerTrigger)
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
            label1.Text = $"{CurTime} sec";
        }
    }
}
