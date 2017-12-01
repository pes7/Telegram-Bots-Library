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
    public partial class MessageUIPhoto : UserControl
    {
        public MessageUIPhoto(Image im)
        {
            InitializeComponent();
            pictureBox1.Image = im;
        }

        public MessageUIPhoto()
        {
            InitializeComponent();
        }
    }
}
