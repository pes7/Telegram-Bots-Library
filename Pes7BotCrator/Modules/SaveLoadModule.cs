using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Pes7BotCrator.Type;

namespace Pes7BotCrator.Modules
{
    public class SaveLoadModule
    {
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
