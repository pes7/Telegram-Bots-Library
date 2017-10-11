using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GuchiBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread th = new Thread(() => {
                Main mn = new GuchiBot.Main();
                mn.ShowDialog();
            });
            th.Start();
        }
    }
}
