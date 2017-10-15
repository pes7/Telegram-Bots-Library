using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    public class Command
    {
        public string cCommand { get; }
        public List<string> cArgs { get; }

        public long UserId { get; }
        public long ChatId { get; }

        public Command(string comand, long userid, long chatid, List<string> args = null)
        {
            cArgs = args;
            cCommand = comand;
            UserId = userid;
            ChatId = chatid;
            cArgs = new List<string>();
        }
        public void AddArgc(string argc)
        {
            cArgs.Add(argc);
        }
    }
}
