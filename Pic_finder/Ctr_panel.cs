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
        public SauceNAO_Mod SN = null;
        public Ctr_panel()
        {
            InitializeComponent();
            SN = new SauceNAO_Mod("257dedb3b7fe24c2ef1c4c9a7a8ff0f22bd2ad3a");
            Robot = new Main_Bot("455187137:AAGs9q50RSsctI75tveQyGfQB6mE09jIu8A", mods: new List<ModuleInterface> { SN });

            Robot.Commands.Add(new SynkCommand(SN.SearchPic, new List<string>()
            {
                "/getsauce",
                "/getsauce@anime_pic_finder_bot"
            }));
        }
    }
}
