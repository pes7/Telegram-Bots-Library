﻿using System;
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
        Bot Bot;
        public Main()
        {
            InitializeComponent();
            Bot = new Bot();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }
    }
}
