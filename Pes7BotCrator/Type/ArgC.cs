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
                    args_parse = message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                }
                catch { return null; }

                args_parse = args_parse.Select(delegate (System.String inp)
                {
                    inp = inp.Trim();
                    return inp.StartsWith("-") ? System.String.Concat(inp.Skip(1).ToArray()) : inp;
                }).Select(p=>p.ToString()).ToArray();
                if(args_parse[0].Trim().ToUpper().Contains(Parent.NameString.ToUpper()))
                {
                    if (args_parse.Length > 1)
                    {
                        if (args_parse[0].ToUpper() == Parent.NameString.ToUpper())
                        {
                            if(args_parse[0]?.Length > 0)
                                message = message.Replace(args_parse[0], System.String.Empty);
                            if (args_parse[1]?.Length > 0)
                                message = message.Replace(args_parse[1], System.String.Empty);
                            Args.Add(new ArgC($"{args_parse[1]}", null, TypeOfArg.Named));
                            var andorelse = message.Split(new string[] { " и ", " или ", " И ", " ИЛИ ", "and", "AND", "or", "OR" }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < andorelse.Length && andorelse.Length>1; i++)
                            {
                                if (andorelse[i].Trim() != "")
                                {
                                    andorelse[i] = Regex.Replace(andorelse[i], @"^\s+", System.String.Empty);
                                    andorelse[i] = Regex.Replace(andorelse[i], @"\s+$", System.String.Empty);
                                    Args.Add(new ArgC($"{i}", andorelse[i], TypeOfArg.Named));
                                }
                            }
                            if (andorelse.Length==1 && args_parse.Length>2)
                            {
                                foreach (System.String arg_parse in args_parse.Skip(2))
                                {
                                    string[] mspl = arg_parse.Trim().Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                                    Args.Add(new ArgC(mspl[0], mspl.Length > 1 ? mspl[1] : null, TypeOfArg.Named));
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
                            string[] ssf = args_parse[i].IndexOfAny(new char[] { ':', '=' }) >= 0 ?
                                args_parse[i].Split(args_parse[i].ElementAt(args_parse[i].IndexOfAny(new char[] { ':', '=' }))) : 
                                new string[] { args_parse[i] };
                            if (ssf.Length > 1)
                            {
                                sf.Name = ssf[0].Trim();
                                sf.Arg = ssf[1].Trim();
                            }
                            else
                            {
                                sf.Name = ssf[0].Trim();
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
        public static ArgC GetArg(List<ArgC> a, string name, string num = null)
        {
            if (num != null)
            {
                var g = a.Find(fn => fn.Name == name);
                return g == null ? a.Find(fn => fn.Name == num) : g;
            }
            else
            {
                return a.Find(fn => fn.Name == name);
            }
        }
    }
}
