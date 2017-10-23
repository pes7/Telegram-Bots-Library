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
using Pes7BotCrator.Type;

namespace Pes7BotCrator.Modules
{
    public class SaveLoadModule
    {
        public int InterVal { get; set; }
        public string FileName { get; set; }
        public Bot Parent { get; set; }
        public SaveLoadModule(int i, string fn, Bot par)
        {
            InterVal = i;
            FileName = fn;
            Parent = par;
        }

        private int Curtime;
        public void Start()
        {
            Curtime = 0;
            Thread th = new Thread(Synk);
            th.Start();
        }

        private void Synk()
        {
            if(Curtime >= InterVal)
            {
                if (Parent.LLikes.Count > 0)
                    SaveLikesToFile(Parent.LLikes, FileName);
                Curtime = 0;
            }
            Curtime++;
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
