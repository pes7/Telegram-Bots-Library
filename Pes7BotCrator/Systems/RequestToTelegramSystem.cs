using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pes7BotCrator.Systems
{
    public class RTTS
    {
        public static List<Task> Tasks { get; set; }
        public static void Send(Task ts)
        {
            Tasks.Add(ts);
        }
    }
}
