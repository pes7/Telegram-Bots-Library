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

namespace Pes7BotCrator.Modules
{
    public class SaveLoadModule : Module
    {
        /*Надо написать BackUp фукції + логування еррорів і чату в файл*/
        public List<Action> SaveActions;
        private int CurtimeSave = 0;
        private int CurtimeBack = 0;
        public SaveLoadModule(int interV, int backupIntervalV) : base("SaveLoadModule", typeof(SaveLoadModule))
        {
            SaveActions = new List<Action>();
            var SaveThread = new Thread(() =>
            {
                while (true)
                {
                    if (CurtimeSave >= interV)
                    {
                        saveIt();
                        CurtimeSave = 0;
                    }
                    CurtimeSave++;
                    Thread.Sleep(1000);
                }
            });
            SaveThread.Start();
            MainThreads.Add(SaveThread);
            var ThBackUp = new Thread(() =>
            {
                while (true)
                {
                    if (CurtimeBack >= backupIntervalV)
                    {
                        backupIt();
                        CurtimeBack = 0;
                    }
                    CurtimeBack++;
                    Thread.Sleep(1000);
                }
            });
            ThBackUp.Start();
            MainThreads.Add(ThBackUp);
        }

        public void backupIt()
        {
            if (!Directory.Exists("./BackUp"))
                Directory.CreateDirectory("./BackUp");
            var files = Directory.GetFiles("./","*.bot");
            foreach (var file in files)
            {
                var i = DateTime.UtcNow.ToString().Split(' ');
                var j = Path.GetFileName(file);
                var k = $"./BackUp/{String.Join("_",i[0].Split('.'))}_{String.Join("_", i[1].Split(':'))}_{j}";
                try
                {
                    File.Copy(file, $"{k}");
                }
                catch { }
            }
        }

        public void saveIt()
        {
            backupIt();
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
                outFile.Close();
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
            try
            {
                FileStream aFile = new FileStream(FileName, FileMode.Open);
                var Return = (T)formatter.Deserialize(aFile);
                aFile.Close();
                return Return;
            }
            catch (SerializationException) {
                Console.WriteLine("Starting the proces of restoring data...");
                Console.WriteLine("Serching BackUps...");
                var str = RestoreData(FileName);
                if(str != null)
                {
                    FileStream aFile = new FileStream(str, FileMode.Open);
                    var Return = (T)formatter.Deserialize(aFile);
                    aFile.Close();
                    return Return;
                }
                else
                {
                    throw new Exception("Error u don't have any backups");
                }
            }            
        }

        public static string RestoreData(string FileName)
        {
            if (Directory.Exists("BackUp"))
            {
                FileName = FileName.Trim('.', '/');
                var directory = new DirectoryInfo("BackUp");
                var myFiles = (from f in directory.GetFiles("*.bot")
                              orderby f.LastWriteTime descending
                              select f);
                foreach(var file in myFiles)
                {
                    if (file.Name.Split('_').Last() == FileName)
                        return $"BackUp/{file.Name}";
                }
                return null;
            }
            else { new Exception("No BackUp Folder"); return null; }
        }
    }
}
