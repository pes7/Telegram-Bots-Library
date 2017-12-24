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
using System.Drawing;
using System.Threading;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pic_finder
{
    class danbooru_api_mod : Module
    {
        private HttpClient Client = new HttpClient();
        private List<System.String> Service_Args = new List<string>()
        {
            "file",
            "show_any",
            "id"
        };
        private HttpResponseMessage resp = null;
        private IBot Serving;
        private Message Msg;
        List<ArgC> Args = null;
        private bool to_file = false;
        private bool show_a = false;
        private bool is_res = false;
        public danbooru_api_mod():base("Danbooru API service\'s collection", typeof(danbooru_api_mod)) { }

        private System.String GenerateURL(System.String base_url, UInt16 max_lim)
        {

            Dictionary<System.String, System.String> Post = new Dictionary<string, string>();
            List<string> Tags = new List<string>();
            if (this.Args != null)
            {
                foreach (ArgC p in this.Args)
                {
                    if (!this.Service_Args.Contains(p.Name))
                    {
                        if (p.Name != "tag") Post.Add(p.Name, p.Arg);
                        else Tags.Add(p.Arg);
                    }
                }
                ArgC ida;
                if ((ida = this.Args.Find(fn => fn.Name == "id")) == null)
                {
                    if (this.Args.Find(fn => fn.Name == "limit") == null) Post.Add("limit", "1");
                    else if (Convert.ToInt32(this.Args.Find(fn => fn.Name == "limit").Arg) > max_lim) throw new ArgumentOutOfRangeException("Limit can\'t be more than 100.");
                }
                else
                {
                    Post.Clear();
                    Tags.Clear();
                    Tags.Add("id:" + ida.Arg);
                }
                if (Tags.Count > 0) Post.Add("tags", string.Join("+", Tags.ToArray()));
            }
            return base_url + (this.Args != null ? string.Join("&", Post.Select(p => p.Key + "=" + p.Value).ToArray()) : "limit=1");
        }

        private async Task<bool> GetRes(System.String base_url, UInt16 m_lim)
        {
            if (this.Args != null)
            {
                this.to_file = this.Args.Find(fg => fg.Name == "file") != null ? true : false;
                this.show_a = this.Args.Find(fg => fg.Name == "show_any") != null ? true : false;
            }
            else
            {
                this.to_file = false;
                this.show_a = false;
            }
            this.is_res = false;
            try
            {
                this.resp = await this.Client.GetAsync(this.GenerateURL(base_url, m_lim));
            }
            catch (Exception ex)
            {
                this.Serving.Exceptions.Add(ex);
                await this.Serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, ex.Message, replyToMessageId: this.Msg.MessageId);
                return false;
            }
            if (!this.resp.IsSuccessStatusCode)
            {
                await this.Serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Unfortunately we had error.", replyToMessageId: this.Msg.MessageId);
                return false;
            }
            else return true;
        }

        private async Task GetAndSendPic(System.String url, System.String rate = "", System.String srate = "s")
        {
            bool sd_fl = this.to_file, shw_a = this.show_a;
            bool succ = true;
            if (rate == null) rate = System.String.Empty;
            if (srate == null) srate = "s";
            System.String exc = System.String.Empty;
            if (url != null) do
                {
                    try
                    {
                        if (succ && rate != srate && !shw_a) throw new Exception("This post is \"unsafe\", \"questionable\" or undefined rating.\nPlease be careful before open it!");
                        System.IO.Stream get_pic = await Client.GetStreamAsync(url);
                        if (sd_fl) await this.Serving.Client.SendDocumentAsync(this.Msg.Chat.Id, new FileToSend(url.Split('/').Last(), get_pic), exc, replyToMessageId: this.Msg.MessageId);
                        else await this.Serving.Client.SendPhotoAsync(this.Msg.Chat.Id, new FileToSend(url.Split('/').Last(), get_pic), replyToMessageId: this.Msg.MessageId);
                        this.is_res = true;
                        succ = true;
                    }
                    catch (Exception ex)
                    {
                        this.Serving.Exceptions.Add(ex);
                        exc = ex.Message;
                        sd_fl = true;
                        succ = false;
                    }
                }
                while (!succ);
            else await this.Serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Can\'t send the post: download link wasn\'t provided.", replyToMessageId: this.Msg.MessageId);
        }

        private async Task DoAStJob(System.String req_url, UInt16 max_lim=100, System.String fl_url= "file_url", System.String rt_prop= "rating", System.String s_rate="s", System.String url_prefix = "")
        {
            this.Args?.RemoveAt(0);
            this.Args?.ForEach(delegate (ArgC to_norm)
            {
                int inx = this.Args.IndexOf(to_norm);
                if (to_norm.Name != null) this.Args.ElementAt(inx).Name = !to_norm.Name.Contains("\"") ? to_norm.Name.Replace(" ", "") : to_norm.Name;
                if (to_norm.Arg != null) this.Args.ElementAt(inx).Arg = !to_norm.Arg.Contains("\"") ? to_norm.Arg.Replace(" ", "") : to_norm.Arg;
            });
            if (!(await this.GetRes(req_url, max_lim))) return;
            dynamic result = JsonConvert.DeserializeObject(await this.resp.Content.ReadAsStringAsync());
            foreach (var post in result)
            {
                System.String url = post[fl_url] != null ? url_prefix + post[fl_url] : null, rating = post[rt_prop];
                await this.GetAndSendPic(url, rating, s_rate);
            }
            if (!this.is_res) await this.Serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Unfortunately we have no result\'s.", replyToMessageId: this.Msg.MessageId);
            else await this.Serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Posts has been sent.", replyToMessageId: this.Msg.MessageId);
        }

        public async void GetYandereAsync(Message msg, IBot serving, List<ArgC> args)
        {
            this.Msg = msg;
            this.Serving = serving;
            this.Args = args;
            await this.DoAStJob("https://yande.re/post.json?");
            //if (!(await this.GetRes("https://yande.re/post.json?", 100))) return;
            //dynamic result = JsonConvert.DeserializeObject(await this.resp.Content.ReadAsStringAsync());
            //foreach (var post in result)
            //{
            //    System.String url = post.file_url, rate = post.rating;
            //    await GetAndSendPic(url, rate, "s");
            //}
            //if (!this.is_res) await this.Serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Unfortunately we have no result\'s.", replyToMessageId: this.Msg.MessageId);
            //else await this.Serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Posts has been sent.", replyToMessageId: this.Msg.MessageId);
        }

        public async void GetDanbooruAsync(Message msg, IBot serving, List<ArgC> args)
        {
            if (args?.FindAll(fn => fn.Name == "tag").Count > 2)
            {
                await serving.Client.SendTextMessageAsync(msg.MessageId, "Unfortunatelly you can\'t input more than two tags, using danbooru.", replyToMessageId: msg.MessageId);
                return;
            }
            this.Msg = msg;
            this.Serving = serving;
            this.Args = args;
            await this.DoAStJob("https://danbooru.donmai.us/posts.json?", url_prefix: "https://danbooru.donmai.us");
            //if (!(await this.GetRes("https://danbooru.donmai.us/posts.json?", 100))) return;
            //dynamic result = JsonConvert.DeserializeObject(await this.resp.Content.ReadAsStringAsync());
            //foreach (var post in result)
            //{
            //    if (post.GetType().GetProperty("file_url") != null)
            //    {
            //        System.String url = "https://danbooru.donmai.us" + post.file_url, rate = post.rating;
            //        await GetAndSendPic(url, rate, "s");
            //    }
            //    else await this.Serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Unfortunately we have no links to download photo for you.", replyToMessageId: this.Msg.MessageId);

            //}
            //if (!this.is_res) await this.Serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Unfortunately we have no result\'s.", replyToMessageId: this.Msg.MessageId);
            //else await this.Serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Posts has been sent.", replyToMessageId: this.Msg.MessageId);
        }

        public async void GetGelboorruAsync(Message msg, IBot serving, List<ArgC> args)
        {
            this.Msg = msg;
            this.Serving = serving;
            this.Args = args;
            await this.DoAStJob("https://gelbooru.com/index.php?page=dapi&s=post&q=index&json=1&");
        }
    }
}
