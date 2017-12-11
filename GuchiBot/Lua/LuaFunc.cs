using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuaAble
{
    class LuaFunc
    {
        public void CPrint(string str)
        {
            Console.WriteLine(str);
        }
        public void MPrint(string str)
        {
            MessageBox.Show(str);
        }
    }
}
