using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    public class ArgC
    {
        public string Name { get; set; }
        public string Arg { get; set; }
        public enum TypeOfArg { Default, Named }
        public TypeOfArg Type { get; set; }
        public ArgC(string name = null, string arg = null, TypeOfArg type = TypeOfArg.Default)
        {
            Name = name;
            Type = type;
            Arg = arg;
        }
        public static List<ArgC> getArgs(string message, IBot Parent)
        {
            List<ArgC> Args = new List<ArgC>();
            if (message != null)
            {
                string[] args_parse = null;
                string realArc = null;
                try
                {
                    args_parse = message.Split('-');
                }
                catch { return null; }

                if(args_parse.Length == 1 || args_parse[0].ToUpper().Contains(Parent.NameString.ToUpper()))
                {
                    args_parse = message.Split(' ');
                    if (args_parse.Length > 1)
                    {
                        if (args_parse[0].ToUpper() == Parent.NameString.ToUpper())
                        {
                            if(args_parse[0]?.Length > 0)
                                message = message.Replace(args_parse[0], "");
                            if (args_parse[1]?.Length > 0)
                                message = message.Replace(args_parse[1], "");
                            Args.Add(new ArgC($"{args_parse[1]}", null, TypeOfArg.Named));
                            var andorelse = message.Split(new string[] { " и ", " или ", " И ", " ИЛИ " }, StringSplitOptions.None);
                            for (int i = 0; i < andorelse.Length; i++)
                            {
                                if (andorelse[i].Trim() != "")
                                {
                                    andorelse[i] = Regex.Replace(andorelse[i], @"^\s+", "");
                                    andorelse[i] = Regex.Replace(andorelse[i], @"\s+$", "");
                                    Args.Add(new ArgC($"{i}", andorelse[i], TypeOfArg.Named));
                                }
                            }
                            return Args;
                        }
                        else return null;
                    }
                    else return null;
                }
                
                if(args_parse.Length == 2 && args_parse.Last().Contains("["))
                {
                    try
                    {
                        Args.Add(new ArgC(args_parse.First()));
                        realArc = message.Split('[').Last();
                        if (realArc != null) Args.Add(new ArgC("default", realArc.Trim(']')));
                        return Args;
                    }
                    catch { Args.Clear(); }
                }
               
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
