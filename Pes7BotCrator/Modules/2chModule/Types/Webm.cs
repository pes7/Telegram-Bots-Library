using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pes7BotCrator.Modules.Types
{
    public class Webm
    {
        public string Path { get; set; }
        public int DurationSec { get; set; }
        public string Thumbnail { get; set; }
        public string FullName { get; set; }
        public Webm(string path, string thub = null, string fullname = null, int duration = 0)
        {
            Path = path;
            Thumbnail = thub;
            FullName = fullname;
            DurationSec = duration;
        }
    }
}
