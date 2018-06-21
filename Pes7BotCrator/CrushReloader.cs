using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Pes7BotCrator.Commands;
using Pes7BotCrator.Modules.Types;
using Pes7BotCrator.Type;
using System.Diagnostics;

namespace Pes7BotCrator
{
    public class CrushReloader
    {
        public Thread MainTh;
        public CrushReloader()
        {
            MainTh = new Thread(() =>
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
                void MyHandler(object sender, UnhandledExceptionEventArgs args)
                {
                    Process.Start($"{Application.ExecutablePath}/GuchiBot.exe");
                }
            });
            MainTh.Start();
        }
    }
}
