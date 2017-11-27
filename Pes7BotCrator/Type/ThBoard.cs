using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    public class ThBoard
    {
        public string Discription { get; }
        public string Date { get; }
        public string Files_count { get; }
        public string Id { get; }
        public string Subject { get; }
        public ThBoard(string id = null, string disc = null, string date = null, string file_count = null, string subject = null)
        {
            Discription = disc;
            Date = date;
            Files_count = file_count;
            Id = id;
            Subject = subject;
        }
        public override string ToString()
        {
            return Id.ToString();
        }
        public static dynamic GetJson(string url)
        {
            try
            {
                string irl = Uri.EscapeUriString(url);
                string doc = "";
                using (System.Net.WebClient client = new System.Net.WebClient()) // WebClient class inherits IDisposable
                {
                    doc = client.DownloadString(irl);
                    Encoding utf8 = Encoding.GetEncoding("UTF-8");
                    Encoding win1251 = Encoding.GetEncoding("Windows-1251");

                    byte[] utf8Bytes = win1251.GetBytes(doc);
                    byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);
                    return JObject.Parse(win1251.GetString(win1251Bytes));
                }
            }
            catch { return null; }
        }
    }
}
