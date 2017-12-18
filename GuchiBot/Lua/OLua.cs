using LuaInterface;
using Pes7BotCrator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pes7BotCrator.Type;

namespace LuaAble
{
    public class OLua
    {
        public List<LuaFile> LuaScripts = new List<LuaFile>();
        public Lua Lua { get; set; }
        public IBotBase Bot { get; set; }

        public OLua(IBotBase bot)
        {
            Bot = bot;
            Lua = new Lua();
            Lua["LuaFunc"] = new LuaFunc();

            /*Register your command here*/
            Lua.RegisterFunction("sendM", this, typeof(OLua).GetMethod("sendM"));
            Lua.RegisterFunction("sendML", this, typeof(OLua).GetMethod("sendML"));
            Lua.RegisterFunction("sendStI", this, typeof(OLua).GetMethod("sendStI"));
            Lua.RegisterFunction("sendStS", this, typeof(OLua).GetMethod("sendStS"));
        }
        public void sendM(string id, string message)
        {
            Bot.Client.SendTextMessageAsync(id, message);
        }
        public void sendML(string message)
        {
            Bot.Client.SendTextMessageAsync(Bot.MessagesLast.Last().Chat.Id,message);
        }
        public void sendStI(int id,string st)
        {
            Bot.Client.SendTextMessageAsync(id,st);
        }
        public void sendStS(string id, string st)
        {
            Bot.Client.SendTextMessageAsync(id, st);
        }

        public void LoadScriptsFromDirectory(string path = "/Scripts")
        {
            string folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + path;
            string[] Pathes = Directory.GetFiles(folder, "*lua");
            foreach (string str in Pathes)
            {
                LuaScripts.Add(new LuaFile(str,Path.GetFileName(str)));
            }
        }

        public void UseLua(string name)
        {
            try
            {
                Lua.DoFile(LuaScripts.Find(e => e.Name == name).Path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with lua code.\n{ex.Data}");
            }
        }

        public void UseLua(LuaFile file)
        {
            try
            {
                Lua.DoFile(file.Path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with lua code.\n{ex.Data}");
            }
        }

        public void UseAllLua()
        {
            foreach (LuaFile file in LuaScripts)
            {
                try
                {
                    Lua.DoFile(file.Path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with lua code.\n{ex.Data}");
                }
            }
        }
    }
}
