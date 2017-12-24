using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace Pes7BotCrator.Modules
{
    public class SaveLoadModule : Module
    {
        public List<Action> SaveActions;
        private int Curtime = 0;
        public SaveLoadModule(int interV) : base("SaveLoadModule", typeof(SaveLoadModule))
        {
            SaveActions = new List<Action>();
            MainThread = new Thread(() =>
            {
                while (true)
                {
                    if (Curtime >= interV && Parent != null)
                    {
                        saveIt();
                        Curtime = 0;
                    }
                    Curtime++;
                    Thread.Sleep(1000);
                }
            });
            MainThread.Start();
        }

        public void saveIt()
        {
            foreach (var act in SaveActions)
            {
                act.DynamicInvoke();
            }
        }

        public static void SaveSomething<T>(T Data, string FileName)
        {
            if (Data == null) { return; }
            T data = Data;
            try
            {
                FileStream outFile = File.Create(FileName);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(outFile, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static T LoadSomething<T>(string FileName)
        {
            if (string.IsNullOrEmpty(FileName)) { throw new Exception("Can't read File."); }
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream aFile = new FileStream(FileName, FileMode.Open);
            return (T)formatter.Deserialize(aFile);
        }
    }
}
