using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    public class ArgC
    {
        public string Name { get; set; }
        public string Arg { get; set; }
        public ArgC(string name = null, string arg = null)
        {
            Name = name;
            Arg = arg;
        }
        public static List<ArgC> getArgs(string message)
        {
            List<ArgC> Args = new List<ArgC>();
            if (message != null)
            {
                string[] args_parse = null;
                try
                {
                    args_parse = message.Split('-');
                }
                catch { return null; }
                if (args_parse.Length > 1)
                {
                    for (int i = 0; i < args_parse.Length; i++)
                    {
                        var sf = new ArgC();
                        try
                        {
                            string[] ssf = args_parse[i].Split(':');
                            if (ssf.Length > 1)
                            {
                                sf.Name = ssf[0];
                                sf.Arg = ssf[1];
                            }
                            else
                            {
                                sf.Name = ssf[0];
                            }
                        }
                        catch { sf.Name = args_parse[i]; }
                        Args.Add(sf);
                    }
                    return Args;
                }
                else return null;
            }
            else return null;
        }
    }
}
