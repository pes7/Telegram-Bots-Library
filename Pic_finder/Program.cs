using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pic_finder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Thread th = new Thread(() => {
                Ctr_panel mn = new Pic_finder.Ctr_panel();
                mn.ShowDialog();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }
    }
}
