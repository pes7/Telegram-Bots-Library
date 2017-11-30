using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuchiBot.Interface
{
    public partial class MessageUI : UserControl
    {
        public MessageUI(Image img, string text)
        {
            InitializeComponent();
            pictureBox1.Image = img;
            label1.Text = text;
        }
        public MessageUI()
        {
            InitializeComponent();
        }
    }
}
