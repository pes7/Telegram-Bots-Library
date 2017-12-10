using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.IO;
using System.Net.Http;
using Telegram.Bot.Types;
using Pes7BotCrator;
using Pes7BotCrator.Type;
using System.Drawing;
using System.Threading;
using Telegram.Bot;

namespace Pic_finder
{
    class Yande_re_MOD:Module
    {
        private System.String Acc_Key;
        private HttpClient Client = new HttpClient();
        private List<System.String> Tags = new List<System.String>();
        public Yande_re_MOD(System.String acc_key):base("Yande.re Module", type:typeof(Yande_re_MOD))
        {
            this.Acc_Key = acc_key;
        }


        private System.String GenerateURL(List<ArgC> param = null)
        {
            Dictionary<System.String, System.String> Post = new Dictionary<string, string>();
            List<string> Tags = new List<string>();
            if (param != null)
            {
                foreach (ArgC p in param)
                {
                    if (p.Name != "file")
                    {
                        if (p.Name != "tag") Post.Add(p.Name, p.Arg);
                        else Tags.Add(p.Arg);
                    }
                }
                ArgC ida;
                if ((ida = param.Find(fn => fn.Name == "id")) == null)
                {
                    if (param.Find(fn => fn.Name == "limit") == null) Post.Add("limit", "1");
                    else if (Convert.ToInt32(param.Find(fn => fn.Name == "limit").Arg) > 100) throw new ArgumentOutOfRangeException("Limit can\'t be more than 100.");
                }
                else
                {
                    Post.Clear();
                    Tags.Clear();
                    Tags.Add("id:" + ida.Arg);
                }
                Post.Add("tags", string.Join("+", Tags.ToArray()));
            }
            return "https://yande.re/post.json?" + (param != null ? string.Join("&", Post.Select(p => p.Key + "=" + p.Value).ToArray()) : "limit=1");
        }

        public async void GetFromYandereAsync(Message msg, IBotBase serving, List<ArgC> args)
        {
            System.String url = null;
            try
            {
                url = this.GenerateURL(args);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, ex.Message, replyToMessageId: msg.MessageId);
                return;
            }
            catch (Exception ex)
            {
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, ex.Message, replyToMessageId: msg.MessageId);
            }
            HttpResponseMessage th = await Client.GetAsync(url);
            if (!th.IsSuccessStatusCode)
            {
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunately we had error.", replyToMessageId: msg.MessageId);
                return;
            }
            dynamic ress = JsonConvert.DeserializeObject(await th.Content.ReadAsStringAsync());
            bool is_res = false, sd_fl = false;
            if (args != null)
            {
                if (args.Find(fg => fg.Name == "file") != null) sd_fl = true;
            }
            foreach (var post in ress)
            {
                System.String fn = post.file_url;
                System.IO.Stream get_pic = await Client.GetStreamAsync(fn);
                if (sd_fl) await serving.Client.SendDocumentAsync(msg.Chat.Id, new FileToSend(fn.Split('/').Last(), get_pic), replyToMessageId: msg.MessageId);
                else await serving.Client.SendPhotoAsync(msg.Chat.Id, new FileToSend(fn.Split('/').Last(), get_pic), replyToMessageId: msg.MessageId);
                is_res = true;
            }
            if (!is_res) await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunately we have no result\'s.");
        }
    }
}
