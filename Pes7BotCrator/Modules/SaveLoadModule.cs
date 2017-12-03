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
using Pes7BotCrator.Type;

namespace Pes7BotCrator.Modules
{
    public class SaveLoadModule : IModule
    {
        public int InterVal { get; set; }
        public string FileName { get; set; }
        public dynamic Parent { get; set; }
        System.Type Type { get; set; }
        public string Name { get; set; }
        public dynamic Modulle { get; set; }
        public Thread MainThread { get; set; }

        System.Type IModule.Type { get; set; }

        public void AbortThread()
        {
            MainThread.Abort();
        }

        public SaveLoadModule(int i, string fn, dynamic parent)
        {
            InterVal = i;
            FileName = fn;
            Parent = parent;
            Name = "SaveLoadModule";
            this.Type = typeof(SaveLoadModule);
            Modulle = this;
        }

        private int Curtime = 0;
        public void Start()
        {
            Curtime = 0;
            MainThread = new Thread(async () => { await SynkAsync(); });
            MainThread.Start();
        }

        private async Task SynkAsync()
        {
            while (true)
            {
                if (Curtime >= InterVal)
                {
                    IBotBase bt = Parent?.Bot;
                    if (bt != null) {
                        LikeDislikeModule LDModule = bt.GetModule<LikeDislikeModule>();
                        if (LDModule != null) {
                            if (LDModule.LLikes.Count > 0)
                            {
                                List<Likes> ls = LDModule.LLikes;
                                SaveLikesToFile(ls, FileName);
                            }
                        }
                    }
                    Curtime = 0;
                }
                Curtime++;
                Thread.Sleep(1000);
            }
        }

        public static void SaveLikesToFile(List<Likes> likes, string fileName)
        {
            if (likes == null) { return; }
            try
            {
                FileStream outFile = File.Create(fileName);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(outFile, likes);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static List<Likes> LoadLikesFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return null; }

            List<Likes> list = new List<Likes>();
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream aFile = new FileStream(fileName, FileMode.Open);
            byte[] buffer = new byte[aFile.Length];
            aFile.Read(buffer, 0, (int)aFile.Length);
            MemoryStream stream = new MemoryStream(buffer);
            return (List<Likes>)formatter.Deserialize(stream);
        }
    }
}
