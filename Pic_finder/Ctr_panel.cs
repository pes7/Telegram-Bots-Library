using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pes7BotCrator.Type;
using Telegram.Bot.Types.InputFiles;
using IniParser;
using IniParser.Model;

namespace Pic_finder
{
    public partial class Ctr_panel : Form
    {
        public Main_Bot Robot;
        private IniData RobotInit;
        public SqlDataAdapter DataAdapter;
        public DataSet Data;
        public Ctr_panel()
        {
            InitializeComponent();
            FileIniDataParser parser = new FileIniDataParser();
            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder();
            try
            {
                this.RobotInit = parser.ReadFile("robot.ini");
                connBuilder.DataSource = RobotInit["DB"]["DataSource"];
                //connBuilder.IntegratedSecurity = bool.Parse(RobotInit["DB"]["IntegratedSecurity"]);
                connBuilder.InitialCatalog = RobotInit["DB"]["InitialCatalog"];
                connBuilder.UserID = RobotInit["DB"]["user"];
                connBuilder.Password = RobotInit["DB"]["password"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't connect.\n" + ex.Source + ":" + ex.Message);
                Application.Exit();
            }
            Robot = new Main_Bot(
                api_key: RobotInit["Robot"]["api_key"],
                name: RobotInit["Robot"]["name"],
                shortName: RobotInit["Robot"]["shortName"],
                creatorName: RobotInit["Robot"]["creatorName"],
                mods: new List<IModule> {
                new danbooru_api_mod(),
                new micro_logic(),
                new SauceNAO_Mod(connBuilder.ConnectionString, RobotInit["SauceNAO"]["SavePicsDir"])
            });

            Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<micro_logic>().SayHello, new List<string>()
            {
                "/start"
            },descr:"Initiating command"));

            Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<micro_logic>().EmExit, new List<string>()
            {
                "/emexit"
            },
            commandName: "shutdown", access: TypeOfAccess.Named, descr: "Shut\'s down a bot permanently.", clearcommand: true));

            Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<micro_logic>().Help, new List<string>()
            {
                "/help"
            },
            commandName: "help", descr: "Display\'s help for a user."));

            /*Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<micro_logic>().DeleteMyMessage, new List<string>()
            {
                "/delete"
            }, commandName: "delete", descr: "Delete\'s replyied to the bot message", clearcommand: false));*/

            Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<danbooru_api_mod>().GetYandereAsync, new List<string>()
            {
                "/getyandere",
                "/getyandere@anime_pic_finder_bot"
            },
            commandName:"yandere", descr:"Get\'s images from yande.re."));

            Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<danbooru_api_mod>().GetDanbooruAsync, new List<string>()
            {
                "/getdanbooru",
                "/getdanbooru@anime_pic_finder_bot"
            },
            commandName:"danbooru", descr:"Get\'s images from Danbooru."));

            Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<danbooru_api_mod>().GetGelboorruAsync, new List<string>()
            {
                "/getgelbooru",
                "/getgelbooru@anime_pic_finder_bot"
            },
            commandName:"gelbooru", descr: "Get\'s images from Gelbooru."));

            Robot.SynkCommands.Add(new SynkCommand(
                Robot.GetModule<danbooru_api_mod>().GetYandereTagsAsync,
                new List<string>()
                {
                    "/getyandere_tags",
                    "/getyandere_tags@anime_pic_finder_bot"
                },
                commandName:"yandere_tags", descr:"Get tags from Yande.re."));
            Robot.SynkCommands.Add(new SynkCommand(
                Robot.GetModule<danbooru_api_mod>().GetDanbooruTagsAsync,
                new List<string>()
                {
                    "/getdanbooru_tags"
                },
                commandName: "danbooru_tags", descr: "Get tags from Danbooru."));
            Robot.SynkCommands.Add(new SynkCommand(
                Robot.GetModule<danbooru_api_mod>().GetGelboorruTagsAsync,
                new List<string>(){
                    "/getgelbooru_tags"
                },
                commandName: "gelbooru_tags", descr: "Get tags from Gelbooru."));
            Robot.SynkCommands.Add(new SynkCommand(
                Robot.GetModule<SauceNAO_Mod>().SearchPic, 
                new List<string>() { "/getsauce" }, 
                commandName: "sauce", descr: "Get's source image of picture.", isPhotoCommand: true));
            Robot.SynkCommands.Add(new SynkCommand(
                Robot.GetModule<SauceNAO_Mod>().AddKeyToDB,
                new List<string>() { "/putkey" },
                commandName: "putkey", descr: "Inputs api key to bot\'s DB."));
            Robot.SynkCommands.Add(new SynkCommand(
                Robot.GetModule<SauceNAO_Mod>().DeleteKeyFromDB,
                new List<string>() { "/deletekey" },
                commandName: "deletekey", descr: "Deletes API-key of certain user from DB"));
            Robot.SynkCommands.Add(new SynkCommand(
                Robot.GetModule<SauceNAO_Mod>().GetMySearchStats,
                new List<string>() { "/getstats" },
                commandName: "stats", descr: "Gets stats of searches of current user."));

            Robot.AutoInvokes.Add(Robot.GetModule<SauceNAO_Mod>().SearchPicOnSend);

            Robot.Start();


        }

        private void Ctr_panel_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int MsgId = radioDelete.Checked ? Convert.ToInt32(MsgIdText.Text) : 0,
                    ReplyMsgId = radioReply.Checked ? Convert.ToInt32(MsgIdText.Text) : 0;
                Telegram.Bot.Types.ChatId ChatId;
                try
                {
                    ChatId = new Telegram.Bot.Types.ChatId(Convert.ToInt32(ChatIdEdit.Text));
                }
                catch (FormatException)
                {
                    ChatId = new Telegram.Bot.Types.ChatId(ChatIdEdit.Text);
                }
                Task<Telegram.Bot.Types.Message> sendProc = null;
                if (this.radioText.Checked && !this.radioDelete.Checked)
                {
                    sendProc = this.Robot.Client.SendTextMessageAsync(ChatId, MsgText.Text, replyToMessageId: ReplyMsgId);
                    sendProc.Wait();
                    AddMsgToLog(sendProc.Result);
                }
                else
                {
                    if (!this.radioDelete.Checked)
                    {
                        foreach (System.String fl in openFileDialog1.FileNames)
                        {
                            InputOnlineFile snd = new InputOnlineFile(System.IO.File.Open(fl, System.IO.FileMode.Open));
                            if (radioPhoto.Checked) sendProc = this.Robot.Client.SendPhotoAsync(ChatId, snd, caption: MsgText.Text, replyToMessageId: ReplyMsgId);
                            if (radioVideo.Checked) sendProc = this.Robot.Client.SendVideoAsync(ChatId, snd, caption: MsgText.Text, replyToMessageId: ReplyMsgId);
                            if (radioFile.Checked) sendProc = this.Robot.Client.SendDocumentAsync(ChatId, snd, caption: MsgText.Text, replyToMessageId: ReplyMsgId);
                            if (sendProc != null)
                            {
                                sendProc.Wait();
                                AddMsgToLog(sendProc.Result);
                            }
                        }
                    }
                }
                //if (radioEdit.Checked) this.Robot.Client.EditMessageTextAsync(MsgIdText.Text, MsgText.Text).Wait();
                if (radioDelete.Checked)
                {
                    this.Robot.Client.DeleteMessageAsync(ChatId, MsgId).Wait();
                    MsgLog.AppendText("Meassage " + MsgId.ToString() + " was deleted.");
                }
            }
            catch (FormatException)
            { MessageBox.Show("Oops, check values which must integer."); }
            catch (OverflowException)
            { MessageBox.Show("Oops, one the values is too big."); }
            catch (Exception ex)
            {
                MsgLog.AppendText(ex.Message);
                MsgLog.AppendText(Environment.NewLine);
            }
        }
        public void AddMsgToLog(Telegram.Bot.Types.Message msg)
        {
            MsgLog.AppendText("Message Id: " + msg.MessageId.ToString() + ". Chat Id: " + msg.Chat.Id + ". Context: \"" + msg.Text + "\".");
            MsgLog.AppendText(Environment.NewLine);
        }

        private void radioPhoto_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioText.Checked) openFileDialog1.ShowDialog();
        }
    }
}
