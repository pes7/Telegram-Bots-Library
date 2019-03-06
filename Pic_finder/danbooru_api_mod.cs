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
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace Pic_finder
{
    public class danbooru_api_mod : Module //Module to handle with danbooru like API`s.
    {
        private HttpClient Client = new HttpClient();
        private readonly List<System.String> Service_Args = new List<string>() //Service tags for bot workaround, but doesn`t include into URL constructor.
        {
            "file",
            "show_any",
            "id"
        };
        //private HttpResponseMessage resp = null;
        //private IBot Serving;
        //private Message Msg;
        //private List<ArgC> Args = null;
        //private bool to_file = false; //Do file must be sent as just file.
        //private bool show_a = false; //Do picture`s showing even if they has non-safe rating.
        //private bool is_res = false; //Did the result`s has been sent.
        public danbooru_api_mod():base("Danbooru API service\'s collection", typeof(danbooru_api_mod)) { }
        
        public async void UpdateRequests(Update update, IBot serving, List<ArgC> args)
        {
            switch (update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                    try
                    {
                        List<ArgC> call_args = new List<ArgC>();
                        System.String[] n_args = update.CallbackQuery.Data.Split(' ');
                        foreach (System.String n_a in n_args)
                        {
                            System.String[] n_as = n_a.Split('=');
                            call_args.Add(new ArgC(n_as[0], n_as.Length > 1 ? n_as[1] : null));
                        }
                        call_args.RemoveAt(0);
                        if (update.CallbackQuery.Data.Contains("action=get_pics"))
                        {
                            //call_args.RemoveAt(call_args.Count - 1);
                            if (call_args.FirstOrDefault().Name.Contains("yandere")) this.GetYandereAsync(update.CallbackQuery.Message, serving, call_args);
                            if (call_args.FirstOrDefault().Name.Contains("danbooru")) this.GetDanbooruAsync(update.CallbackQuery.Message, serving, call_args);
                            if (call_args.FirstOrDefault().Name.Contains("gelbooru")) this.GetGelboorruAsync(update.CallbackQuery.Message, serving, call_args);
                            if (call_args.FirstOrDefault().Name.Contains("konachan")) this.GetKonachanAsync(update.CallbackQuery.Message, serving, call_args);
                        }
                        /*if (update.CallbackQuery.Data.Contains("action=download_to_file"))
                        {
                            System.String url = ArgC.GetArg(call_args, "url")?.Arg;
                            await serving.Client.SendDocumentAsync(update.CallbackQuery.Message.Chat.Id, new InputOnlineFile(await this.Client.GetStreamAsync(url)));
                        }*/
                    }
                    catch (Exception ex)
                    {
                        serving.Exceptions.Add(ex);
                        try
                        { await serving.Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Oops… Something got wrong."); }
                        catch (Exception ex2)
                        { serving.Exceptions.Add(ex2); }
                        
                    }
                    break;
            }
        }

        private System.String GenerateURL(System.String base_url, UInt16 max_lim, System.Boolean id_to_tags=true, List<ArgC> args=null) //URL Generator.
        {

            Dictionary<System.String, System.String> Post = new Dictionary<string, string>();
            List<string> Tags = new List<string>();
            if (args != null)
            {
                foreach (ArgC p in args)
                {
                    if (p.Name != null && p.Arg != null && p.Arg != System.String.Empty)
                    {
                        if (!this.Service_Args.Contains(p.Name)) //If arg isn`t for a service, it is using to build URL
                        {
                            if (p.Name != "tag") Post.Add(p.Name, p.Arg); //If it`s a tag, it`s adding to the search Tags list. }}
                            else Tags.Add(p.Arg);
                        }
                    }
                    else if (this.Service_Args.IndexOf(p?.Name) == -1) throw new ArgumentNullException(p.Name);
                }
                var ida = ArgC.GetArg(args, "id");
                if (ida == null)
                {
                    var lim = ArgC.GetArg(args, "limit");
                    if (lim != null)
                    {
                        if (Convert.ToInt32(lim?.Arg) > max_lim) throw new ArgumentOutOfRangeException("Limit can\'t be more than "+Convert.ToString(max_lim)+"."); //If maximal limit was overrided.
                    }
                    else Post.Add("limit", "1"); //Automatically adding limit. 
                }
                else //If post id was specified.
                {
                    Post.Clear();
                    Tags.Clear();
                    if (ida.Arg == null) throw new ArgumentNullException("id.");
                    else if (id_to_tags) Tags.Add("id:" + ida?.Arg);
                    else Post.Add("id", ida?.Arg);
                }
                if (Tags.Count > 0) Post.Add("tags", string.Join("+", Tags.ToArray())); //Join a tag`s in a parametr.
            }
            return base_url + (args != null ? string.Join("&", Post.Select(p => p.Key + "=" + p.Value).ToArray()) : "limit=1"); //Finally constructing and returning API-URL.
        }

        private async Task<HttpResponseMessage> GetResAsync(System.String base_url, UInt16 m_lim, Message msg, IBot serving, System.Boolean id_to_tags = true, List<ArgC> args=null) //Getting API document.
        {
            HttpResponseMessage resp;
            try
            {
                resp = await this.Client.GetAsync(this.GenerateURL(base_url, m_lim, id_to_tags, args));
            }
            catch (Exception ex)
            {
                serving.Exceptions.Add(ex);
                throw new BotGetsWrongException(ex.Message, ex);
            }
            if (resp!=null)
            {
                if (!resp.IsSuccessStatusCode || resp.Content.Headers.ContentLength == 0) throw new BotGetsWrongException("Unfortunately, request is unsuccesful.");
            }
            return resp;
        }

        private async Task GetAndSendPicAsync(System.String url, Message msg, IBot serving, System.String rate = "", System.String erate = "e", System.String command_name="", Int64 post_id=0, bool sd_fl=false, bool shw_a=false) //Getting and sending a pic from API-result`s.
        {
            bool succ = true, is_res=false; //If current operation was successed.
            if (rate == null) rate = System.String.Empty;
            if (erate == null) erate = "e";
            System.String exc = System.String.Empty;
            if (url != null) do
                {
                    try
                    {
                        if (succ && rate == erate && !shw_a) throw new BotGetsWrongException("This post is \"unsafe\" or has undefined rating.\nPlease be careful before open it!"); //Prevention of sending an explicit pic without confirmation.
                        System.IO.Stream get_pic = await Client.GetStreamAsync(url);
                        /*if (get_pic.Length > (10 * 1024) && !sd_fl && exc == System.String.Empty)
                        {
                            sd_fl = true;
                            exc = "Image is too large";
                        }*/
                        if (sd_fl) await serving.Client.SendDocumentAsync(msg.Chat.Id, new InputOnlineFile(get_pic, url.Split('/').Last()), exc, disableNotification: true/*, replyToMessageId: this.Msg.MessageId*/);
                        else await serving.Client.SendPhotoAsync(msg.Chat.Id, new InputOnlineFile(get_pic, url.Split('/').Last()), disableNotification: true, replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton()
                        {
                            Text = "Download as file",
                            CallbackData = "action=get_pics " + command_name + " file show_any id=" + post_id.ToString()
                        }))/*, replyToMessageId: this.Msg.MessageId)*/;
                        exc = System.String.Empty;
                        is_res = true;
                        succ = true;
                    }
                    catch (Exception ex)
                    {
                        if (!(ex is Telegram.Bot.Exceptions.ApiRequestException || ex is BotGetsWrongException)) serving.Exceptions.Add(ex); //If Exception was untypical, it`s recording.
                        exc = ex.Message;
                        if (sd_fl) await serving.Client.SendTextMessageAsync(msg.Chat.Id, exc + (!(ex is Telegram.Bot.Exceptions.ApiRequestException || ex is BotGetsWrongException) ? "\nURL which has caused this:\n" + url : System.String.Empty));
                        sd_fl = true;
                        succ = false;
                    }
                }
                while (!succ);
            else await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Can\'t send the post: download link wasn\'t provided."/*, replyToMessageId: this.Msg.MessageId*/);
            if (!is_res) throw new BotGetsWrongException("Unfortunately we have no results.");
        }
        /*
        private void NormalizeArgs()
        {
            if (this.Args == null) return;
            try
            {
                this.Args.RemoveAt(0); //A little "crunch".
                if (this.Args.Count>0)
                if (this.Args.ElementAt(0).Type == ArgC.TypeOfArg.Default)
                {
                    this.Args.ForEach(delegate (ArgC to_norm) //Normalizing the arg`s to prevent a blank space`s.
                    {
                        int inx = this.Args.IndexOf(to_norm);
                        if (to_norm.Name != null) this.Args.ElementAt(inx).Name = !to_norm.Name.Contains("\"") ? to_norm.Name.Replace(" ", "") : to_norm.Name;
                        if (to_norm.Arg != null) this.Args.ElementAt(inx).Arg = !to_norm.Arg.Contains("\"") ? to_norm.Arg.Replace(" ", "") : to_norm.Arg;
                    }); //End of the "crunch".
                }
                else if (this.Args.ElementAt(0).Type == ArgC.TypeOfArg.Named)
                {
                    foreach (System.String to_arg in this.Args.ElementAt(0).Arg.Split(' '))
                    {
                        System.String[] to_arg_div = to_arg.Split('=');
                        this.Args.Add(new ArgC(to_arg_div.ElementAt(0), to_arg_div.Length > 1 ? to_arg_div.ElementAt(1) : null));
                    }
                        if (this.Args.Count != 0) this.Args.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                this.Serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Oops, something got wrong.\n"+ex.Message);
            }
        }
        */
        private async Task DoAStJobAsync(System.String req_url, Message msg, List<ArgC> args, IBot serving, UInt16 max_lim = 100, System.String fl_url = "file_url", System.String rt_prop = "rating", System.String e_rate = "e", System.String url_prefix = "", Func<List<ArgC>, IBot, Message, List<ArgC>> prep_args = null) //Do a typical job to get art`s.
        {
            try
            {
                HttpResponseMessage resp = null;
                bool to_file = false, show_a = false;
                ArgC command = args?.First();
                args = serving.GetModule<micro_logic>().NormalizeArgs(msg, serving, args);
                if (args != null)
                {
                    to_file = ArgC.GetArg(args, "file") != null ? true : false;
                    show_a = ArgC.GetArg(args, "show_any") != null ? true : false;
                }
                List<ArgC> before_prep = new List<ArgC>();
                if (args != null)
                    foreach (ArgC arg in args)
                    {
                        before_prep.Add(new ArgC(
                            name: arg.Name ?? System.String.Empty,
                            arg: arg.Arg ?? System.String.Empty
                            ));
                    }
                else command = new ArgC(msg.Text);
                if (prep_args != null) args = prep_args(args, serving, msg);
                resp = await this.GetResAsync(req_url, max_lim, msg, serving, args: args); //Getting a doc.
                dynamic result = JsonConvert.DeserializeObject(await resp.Content.ReadAsStringAsync()); //Doing it`s dynamical parsing.
                foreach (var post in result)
                {
                    System.String url = post[fl_url] != null ? url_prefix + post[fl_url] : null, rating = post[rt_prop],
                        cm = System.String.Empty;
                    cm += command.Name ?? System.String.Empty;
                    cm += command.Arg ?? System.String.Empty;
                    await this.GetAndSendPicAsync(url, msg, serving, rating, e_rate, command_name: cm, post_id: Convert.ToInt64(post.id), sd_fl: to_file, shw_a: show_a);
                }
                    if (before_prep.Exists(p => p.Name.Contains("id"))) return;
                    System.String next_req = System.String.Empty;
                    before_prep.Insert(0, command);
                    if (ArgC.GetArg(before_prep, "page") == null) before_prep.Add(new ArgC("page", "2"));
                    else
                    {
                        ArgC page_arg = before_prep.Where(a => a.Name?.ToLower() == "page").FirstOrDefault();
                        int ind = before_prep.IndexOf(page_arg);
                        if (page_arg.Arg != null)
                        {
                            page_arg.Arg = (Convert.ToUInt32(page_arg.Arg) + 1).ToString();
                            before_prep.RemoveAt(ind);
                            before_prep.Add(page_arg);
                        }
                    }
                    foreach (ArgC arg in before_prep)
                    {
                        next_req += arg.Name ?? System.String.Empty;
                        next_req += arg.Arg != null ? "=" : System.String.Empty;
                        next_req += arg.Arg ?? System.String.Empty;
                        next_req += " ";
                    }
                    next_req = next_req.Remove(next_req.Count() - 1);
                    /*await serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, "You can help AniPic.", disableNotification: true, replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton()
                    {
                        Text = "Learn more",
                        CallbackData = "help=donate"
                    }));*/
                    await serving.Client.SendTextMessageAsync(
                        msg.Chat.Id,
                        "Do you wanna get next results?",
                        replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton()
                        {
                            Text = "Get it",
                            CallbackData = "action=get_pics " + next_req
                        }), disableNotification: true);
                //else await serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Posts has been sent."/*, replyToMessageId: this.Msg.MessageId*/);
            }
            catch (BotGetsWrongException ex)
            { await serving.Client.SendTextMessageAsync(msg.Chat.Id, ex.Message); }
            catch(Exception ex)
            {
                serving.Exceptions.Add(ex);
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Oops… Something got wrong.");
            }
        }

        /*private async Task DoAStJobTagsAsync(System.String req_url, System.String serv_name, System.String tag_name ="name", System.UInt16 max_lim=100, Action prep_args = null)
        {
            //this.NormalizeArgs();
            if (prep_args != null) prep_args();
            try
            {
                if (!await this.GetResAsync(req_url, max_lim, false)) return; //Getting a doc.
                dynamic result = JsonConvert.DeserializeObject(await this.resp.Content.ReadAsStringAsync()); //Doing it`s dynamical parsing.
                System.String rep_msg = "Avalible tags on " + serv_name + " is:\n";
                foreach (var tag in result)
                {
                    rep_msg += " — \'" + tag[tag_name] + "\';\n";
                }
                if (result.Count == 0) rep_msg += "\'nothing\'";
                await this.Serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, rep_msg);
            }
            catch (Exception ex)
            {
                await this.Serving.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Sorry, something got wrong.");
            }
        }
        */

        public async void GetYandereAsync(Message msg, IBot serving, List<ArgC> args) //Function to browse yande.re.
        {
            await this.DoAStJobAsync("https://yande.re/post.json?", msg, serving: serving, args: args);
        }

        /*public async void GetYandereTagsAsync(Message msg, IBot serving, List<ArgC> args) //Function to browse yande.re tags.
        {
            //this.Msg = msg;
            this.Serving = serving;
            //this.Args = args;
            await this.DoAStJobTagsAsync("https://yande.re/tag.json?", "Yande.re");
        }*/

        public async void GetDanbooruAsync(Message msg, IBot serving, List<ArgC> args) //Function to browse Danbooru.
        {
            await this.DoAStJobAsync("https://danbooru.donmai.us/posts.json?", msg, serving: serving, args: args, prep_args: delegate(List<ArgC> args1, IBot serv, Message msg1)
                 {
                     if (args1 != null)
                     {
                         if (ArgC.GetArg(args, "tag")?.Arg.Split('+').Length > 2) //Caution about using more than two tags.
                         {
                             serv.Client.SendTextMessageAsync(msg1.Chat.Id, "Unfortunatelly you can\'t input more than two tags, using danbooru."/*, replyToMessageId: msg.MessageId*/).Wait();
                             throw new BotGetsWrongException("Unfortunatelly you can\'t input more than two tags, using danbooru.");
                         }
                     }
                     return args1;
                 });
        }

        /*public async void GetDanbooruTagsAsync(Message msg, IBot serving, List<ArgC> args) //Function to browse Danbooru tags.
        {
            this.Msg = msg;
            this.Serving = serving;
            this.Args = args;
            await this.DoAStJobTagsAsync("https://danbooru.donmai.us/tags.json?", "Danbooru", prep_args: delegate ()
            {
                System.String[] serv_tags = { "limit", "page" };
                if (this.Args != null)
                {
                    foreach (ArgC arg in this.Args) if (arg.Name != null && !serv_tags.Contains(arg.Name)) arg.Name = "search[" + arg.Name + "]";
                }
            });
        }*/

        public async void GetGelboorruAsync(Message msg, IBot serving, List<ArgC> args) //Function to browse Gelbooru.
        {
            await this.DoAStJobAsync("https://gelbooru.com/index.php?page=dapi&s=post&q=index&json=1&", msg, serving: serving, args: args, prep_args: delegate (List<ArgC> args1, IBot serv, Message msg1)
            {
                if (args1 != null) args.ForEach(delegate (ArgC arg) { if (arg.Name.IndexOf("page") != -1) args1.ElementAt(args1.IndexOf(arg)).Name = arg.Name.Replace("page", "pid"); }); //Replacing "page" to "pid" for normal browsing.
                return args1;
            });
        }

        /*public async void GetGelboorruTagsAsync(Message msg, IBot serving, List<ArgC> args) //Function to browse Gelbooru.
        {
            this.Msg = msg;
            this.Serving = serving;
            this.Args = args;
            await this.DoAStJobTagsAsync("https://gelbooru.com/index.php?page=dapi&s=tag&q=index&json=1&", "Gelbooru", tag_name:"tag", prep_args: delegate ()
            {
                if (this.Args != null) args.ForEach(delegate (ArgC arg) { if (arg.Name.IndexOf("page") != -1) this.Args.ElementAt(this.Args.IndexOf(arg)).Name = arg.Name.Replace("page", "pid"); }); //Replacing "page" to "pid" for normal browsing.
            });
        }*/

        public async void GetKonachanAsync(Message msg, IBot serving, List<ArgC> args)
        {
            await this.DoAStJobAsync("https://konachan.com/post.json?", msg, serving: serving, args: args);
        }
    }
}
