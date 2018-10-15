using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public Ctr_panel()
        {
            InitializeComponent();
            FileIniDataParser parser = new FileIniDataParser();
            try
            {
                this.RobotInit = parser.ReadFile("robot.ini");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't open INI-file.\n" + ex.Message);
                Application.Exit();
            }
            Robot = new Main_Bot(
                api_key: RobotInit["Robot"]["api_key"],
                name: RobotInit["Robot"]["name"],
                shortName: RobotInit["Robot"]["shortName"],
                creatorName: RobotInit["Robot"]["tedechan"],
                mods: new List<IModule> {
                new danbooru_api_mod(),
                new micro_logic(),
                new SauceNAO_Mod("257dedb3b7fe24c2ef1c4c9a7a8ff0f22bd2ad3a")
            });

            Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<micro_logic>().SayHello, new List<string>()
            {
                "/start"
            },descr:"Initiating command"));

            Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<micro_logic>().EmExit, new List<string>()
            {
                "/emexit"
            }, 
            commandName: "shutdown", access:TypeOfAccess.Named, descr:"Shut\'s down a bot permanently."));

            Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<micro_logic>().Help, new List<string>()
            {
                "/help"
            },
            commandName: "help", descr: "Display\'s help for a user."));

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
                    "/getdanbooru_tags",
                    "/getdanbooru_tags@anime_pic_finder_bot"
                },
                commandName: "danbooru_tags", descr: "Get tags from Danbooru."));
            Robot.SynkCommands.Add(new SynkCommand(
                Robot.GetModule<danbooru_api_mod>().GetGelboorruTagsAsync,
                new List<string>(){
                    "/getgelbooru_tags",
                    "/getgelbooru_tags@anime_pic_finder_bot"
                },
                commandName: "gelbooru_tags", descr: "Get tags from Gelbooru."));
            SynkCommand sauce_nao_com = new SynkCommand(Robot.GetModule<SauceNAO_Mod>().SearchPic,
                new List<string>
                {
                    "/getsauce"
                }, descr: "Get\'s source of image that has been sent.", commandName: "sauce"
                );
            //sauce_nao_com.Type = TypeOfCommand.Photo;
            Robot.SynkCommands.Add(sauce_nao_com);
            //Robot.SynkCommands.ForEach(delegate (SynkCommand command) { if (command.CommandName == "sauce") command.Type = TypeOfCommand.Photo; });
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
