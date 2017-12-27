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

namespace Pic_finder
{
    public partial class Ctr_panel : Form
    {
        public Main_Bot Robot;
        public Ctr_panel()
        {
            InitializeComponent();
            Robot = new Main_Bot("455187137:AAGs9q50RSsctI75tveQyGfQB6mE09jIu8A", mods: new List<IModule> {
                new danbooru_api_mod(),
                new micro_logic()
            });

            Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<micro_logic>().SayHello, new List<string>()
            {
                "/start"
            }));

            Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<danbooru_api_mod>().GetYandereAsync, new List<string>()
            {
                "/getyandere",
                "/getyandere@anime_pic_finder_bot"
            }));

            Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<danbooru_api_mod>().GetDanbooruAsync, new List<string>()
            {
                "/getdanbooru",
                "/getdanbooru@anime_pic_finder_bot"
            }));

            Robot.SynkCommands.Add(new SynkCommand(Robot.GetModule<danbooru_api_mod>().GetGelboorruAsync, new List<string>()
            {
                "/getgelbooru",
                "/getgelbooru@anime_pic_finder_bot"
            }
            ));
        }

        private void Ctr_panel_Load(object sender, EventArgs e)
        {

        }
    }
}
