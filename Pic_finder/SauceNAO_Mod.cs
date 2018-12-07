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
using System.Collections.Specialized;
using Telegram.Bot.Args;
using System.Net.Http.Headers;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Data.SqlClient;
using DupImageLib;

namespace Pic_finder
{
    [Table(Name = "SauceNAO_accs")]
    internal class SauceNAO_Acc
    {
        [Column(Name = "Id", IsPrimaryKey = true, IsDbGenerated = true, DbType = "BigInt Primary Key identity")]
        public long Id;
        [Column(Name = "UserId", DbType = "BigInt", CanBeNull = true)]
        public long? UserId;
        [Column(Name = "SauceNAO_UserId", DbType = "BigInt", CanBeNull = true)]
        public long SauceNAO_UserId;
        [Column(Name = "Acc_type", DbType = "tinyint", CanBeNull = true)]
        public Int16 AccountType;
        [Column(Name = "ApiKey", DbType = "varchar(40)", CanBeNull = true)]
        public System.String ApiKey;
        [Column(Name = "LongLimit", DbType = "SmallInt", CanBeNull = true)]
        public UInt16? LongLimit;
        [Column(Name = "LongRemaining", DbType = "SmallInt", CanBeNull = true)]
        public UInt16? LongRemaining;
        [Column(Name = "ShortLimit", DbType = "TinyInt", CanBeNull = true)]
        public UInt16? ShortLimit;
        [Column(Name = "ShortRemaining", DbType = "TinyInt", CanBeNull = true)]
        public UInt16? ShortRemaining;
        [Column(Name = "LastRequest", DbType = "DateTime", CanBeNull = true)]
        public System.DateTime? LastRequestTime;
        /*
        private EntitySet<SearchQuery> _Queries = new EntitySet<SearchQuery>();
        [Association(Storage = "_Queries", OtherKey = "AccountId", ThisKey ="Id", Name = "FK_SrRes_SNAO_ACC")]
        public ICollection<SearchQuery> Queries
        {
            get { return this._Queries; }
            set { this._Queries.Assign(value); }
        }
        */
    }

    [Table(Name = "SauceSearch")]
    internal class SearchQuery
    {
        //private EntityRef<SauceNAO_Acc> AccountRow = new EntityRef<SauceNAO_Acc>();
        //private EntitySet<SearchResult> _Results = new EntitySet<SearchResult>();
        [Column(Name = "Id", IsPrimaryKey = true, IsDbGenerated = true, DbType = "BigInt Primary Key identity")]
        public int Id;
        [Column(Name = "AccId", DbType = "BigInt", CanBeNull = true)]
        public long? AccountId;
        [Column(Name = "ImageHash", DbType = "varchar(32)", CanBeNull = true)]
        public System.String ImageHash;
        [Column(Name = "SearchStatus", DbType = "tinyint", CanBeNull = true)]
        public UInt16? SearchStatus;
        [Column(Name = "ResultsRequested", DbType = "tinyint", CanBeNull = true)]
        public UInt16 ResultsRequested;
        [Column(Name = "SearchDepth", DbType = "varchar(4)", CanBeNull = true)]
        public System.String SearchDepth;
        [Column(Name = "MinimumSimularity", DbType = "real", CanBeNull = true)]
        public float MinimumSimularity;
        [Column(Name = "ResultsReturned", DbType = "tinyint", CanBeNull = true)]
        public UInt16 ResultsReturned;
        [Column(Name = "JSONresp", DbType = "text", CanBeNull = true)]
        public System.String JSON;
        /*
        [Association(Storage = "_Results", OtherKey = "SearchId", ThisKey ="Id", Name = "FK_SrRes_Search")]
        public ICollection<SearchResult> Results
        {
            get { return this._Results; }
            set { this._Results.Assign(value); }
        }
        [Association(Name = "FK_SrRes_SNAO_ACC", IsForeignKey =true, Storage = "AccountRow", ThisKey = "AccountId")]
        public SauceNAO_Acc SauceNAO_Account
        {
            get { return this.AccountRow.Entity; }
            set { this.AccountRow.Entity = value; }
        }
        */

    }

    [Table(Name = "SauceSearchResult")]
    internal class SearchResult
    {
        [Column(Name = "Id", IsPrimaryKey = true, IsDbGenerated = true, DbType = "BigInt Primary Key identity")]
        public long Id;
        [Column(Name = "SearchId", DbType = "BigInt", CanBeNull = true)]
        public long? SearchId;
        [Column(Name = "Similarity", DbType = "real", CanBeNull = true)]
        public float Similarity;
        [Column(Name = "Thumbnail", DbType = "Text", CanBeNull = true)]
        public System.String Thumbnail;
        [Column(Name = "IndexId", DbType = "tinyint", CanBeNull = true)]
        public UInt16 IndexId;
        [Column(Name = "IndexName", DbType = "nText", CanBeNull = true)]
        public System.String IndexName;
        /*
        private EntityRef<SearchQuery> MainQuery;
        [Association(IsForeignKey =true, Storage = "MainQuery", ThisKey = "SearchId", Name = "FK_SrRes_Search")]
        public SearchQuery Query
        {
            get { return this.MainQuery.Entity; }
            set { this.MainQuery.Entity = value; }
        }
        private EntitySet<ExternalUrls> ExternalUrls = new EntitySet<ExternalUrls>();
        [Association(Storage = "ExternalUrls", OtherKey = "ResultId", ThisKey ="Id", Name = "FK_ExtURL_Search")]
        public ICollection<ExternalUrls> Urls
        {
            get { return this.ExternalUrls; }
            set { this.ExternalUrls.Assign(value); }
        }
        */
    }

    [Table(Name = "ExtURLs_of_Search")]
    internal class ExternalUrls
    {
        [Column(Name = "Id", DbType = "BigInt Primary Key identity", IsDbGenerated = true, IsPrimaryKey = true)]
        public long Id;
        [Column(Name = "ResId", DbType = "BigInt", CanBeNull = true)]
        public long? ResultId;
        [Column(Name = "ResultURL", DbType = "text", CanBeNull = true)]
        public System.String URL;
        /*
        private EntityRef<SearchResult> MainResult;
        [Association(IsForeignKey =true, Storage = "MainResult", ThisKey = "ResultId", Name = "FK_ExtURL_Search")]
        public SearchResult Search
        {
            get { return this.MainResult.Entity; }
            set { this.MainResult.Entity = value; }
        }
        */
    }

    public class SauceNAO_Mod : Module
    {
        public IBot Bot;
        public List<ArgC> Args;
        public Message Msg;
        private HttpClient Client = new HttpClient();
        public DataContext dataContext;
        private Table<SauceNAO_Acc> Users;
        private Table<SearchQuery> SearchQueries;
        private Table<SearchResult> SearchResults;
        private Table<ExternalUrls> ExternalUrls;
        private ImageHashes imageHasher = new ImageHashes(new ImageMagickTransformer());
        //private int db_bitmask;
        public System.String SavePicsDir;
        public readonly UInt16 DefNumres = 7;
        private readonly System.String TooManyReq = "Unfortunatelly you had reached out from search limit.\nPlease try again in a next day.";
        private readonly System.String GotWrong = "Oops, something got wrong.\n";
        private readonly System.String NoKey = "You have no registred SauceNAO API-key in the bot.\nYou can get it by following instructions from a /help command.";

        public SauceNAO_Mod(System.String sql_conn_string, System.String save_dir) : base("SauceNAO finder", typeof(SauceNAO_Mod))
        {
            if (sql_conn_string == null) return;
            this.dataContext = new DataContext(sql_conn_string);
            this.Users = this.dataContext.GetTable<SauceNAO_Acc>();
            this.SearchQueries = this.dataContext.GetTable<SearchQuery>();
            this.SearchResults = this.dataContext.GetTable<SearchResult>();
            this.ExternalUrls = this.dataContext.GetTable<ExternalUrls>();
            this.SavePicsDir = save_dir;
            /*
            System.String index_hmags = "0",
                index_hanime = "0",
                index_hcg = "0",
                index_ddbobjects = "0",
                index_ddbsamples = "0",
                index_pixiv = "1",
                index_pixivhistorical = "1",
                index_anime = "0",
                index_seigaillust = "1",
                index_danbooru = "1",
                index_drawr = "0",
                index_nijie = "1",
                index_yandere = "1";
            this.db_bitmask = Convert.ToInt32(index_yandere + index_nijie + index_drawr + index_danbooru + index_seigaillust + index_anime + index_pixivhistorical + index_pixiv + index_ddbsamples + index_ddbobjects + index_hcg + index_hanime + index_hmags, 2);
            */
        }

        private bool IsSt429(HttpResponseMessage resp)
        {
            return resp.StatusCode.ToString().Contains("429");
        }


        private async Task<HttpResponseMessage> DoASearchAsync(System.IO.Stream take, System.String api_key, UInt16 numres)
        {
            System.String minsim = "80";
            using (MemoryStream push = new MemoryStream())
            {
                Bitmap to_push = new Bitmap(take);
                to_push.Save(push, System.Drawing.Imaging.ImageFormat.Png);
                MultipartFormDataContent post_data = new MultipartFormDataContent();
                HttpContent content = new ByteArrayContent(push.ToArray());
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
                post_data.Add(content, "file", "image.png");
                return await Client.PostAsync("http://saucenao.com/search.php?output_type=2&numres=" + numres.ToString() + "&minsim=" + minsim + "&db=999" /*"&dbmask=999" *//*+ Convert.ToString(this.db_bitmask)*/ + "&api_key=" + api_key, post_data);
            }
        }

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
                this.Bot.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Oops, something got wrong.\n"+ex.Message);
            }
        }

        public async void AddKeyToDB(Message msg, IBot serving, List<ArgC> args)
        {
            this.Msg = msg;
            this.Bot = serving;
            this.Args = args;
            try
            {
                System.String api_key;
                this.NormalizeArgs();
                try
                {
                    api_key = ArgC.GetArg(this.Args, "key").Arg;
                }
                catch(NullReferenceException)
                {
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "You should put your key in the \"key\" parameter");
                    return;
                }
                var users = from u in this.Users
                            where u.UserId == msg.From.Id
                            select u;
                if (users.Count<SauceNAO_Acc>() == 0)
                {
                    this.Users.InsertOnSubmit(new SauceNAO_Acc
                    {
                        UserId = this.Msg.From.Id,
                        SauceNAO_UserId = 0,
                        AccountType = 0,
                        ApiKey = api_key,
                        LongLimit = 1,
                        LongRemaining = 1,
                        ShortLimit = 1,
                        ShortRemaining = 1,
                        LastRequestTime = DateTime.Now
                    });
                }
                else users.FirstOrDefault().ApiKey = ArgC.GetArg(this.Args, "key").Arg;
                this.dataContext.SubmitChanges();
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Your key is succesfully added(changed) in to my DB.\n" +
                    "Now m̶y̶ ̶m̶a̶s̶t̶e̶r̶ ̶c̶a̶n̶ ̶u̶s̶e̶ ̶i̶t̶ ̶f̶o̶r̶ ̶i̶t̶'̶s̶ ̶o̶w̶n̶ ̶s̶i̶n̶i̶s̶t̶e̶r̶ ̶a̶i̶m̶s̶ you can search for links of originals of images, which you found in social networks.");
            }
            catch (Exception ex)
            {
                serving.Exceptions.Add(ex);
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, this.GotWrong + ex.Message);
            }
        }

        public async void DeleteKeyFromDB(Message msg, IBot serving, List<ArgC> args)
        {
            try
            {
                var users = from u in this.Users
                            where u.UserId == msg.From.Id
                            select u;
                if (users.Count() != 0)
                {
                    this.Users.DeleteAllOnSubmit(users);
                    this.dataContext.SubmitChanges();
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Your API-key has been deleted from DB succesfully.");
                }
                else await serving.Client.SendTextMessageAsync(msg.Chat.Id, "You have no account to delete.");
            }
            catch (Exception ex)
            {
                serving.Exceptions.Add(ex);
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Oops, something got wrong.\n" + ex.Message);
            }
        }

        public async void GetMySearchStats(Message msg, IBot serving, List<ArgC> args)
        {
            var users = from u in this.Users
                        where u.UserId == msg.From.Id
                        select u;
            if (users.Count() != 0)
            {
                SauceNAO_Acc user = users.FirstOrDefault();
                if ((DateTime.Now - user.LastRequestTime).Value.TotalHours > 24.0)
                {
                    user.LongRemaining = user.LongLimit;
                    user.ShortRemaining = user.ShortLimit;
                }
                else
                {
                    if (user.LongRemaining != 0 && user.ShortRemaining == 0 && (DateTime.Now - user.LastRequestTime).Value.TotalSeconds > 10.0) user.ShortRemaining = user.ShortLimit;
                }
                this.dataContext.SubmitChanges();
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Your stats.\n(Information can be incorrect, because it\'s bases on your last request.)\n" +
                    "Long count of avalieble searches is " + user.LongRemaining.ToString() + " out of " + user.LongLimit.ToString() +
                    ".\nShort count – " + user.ShortRemaining.ToString() + " out of " + user.ShortLimit.ToString() + "." +
                    ((DateTime.Now - user.LastRequestTime).Value.TotalHours < 24.0 && user.LongRemaining == 0 ? "\nNext searches will be avalieble in " + (24.0 - (DateTime.Now - user.LastRequestTime).Value.TotalHours).ToString() + " hours." : ""));
            }
            else await serving.Client.SendTextMessageAsync(msg.Chat.Id, this.NoKey);
        }
        
        public async void SearchPic(Message msg, IBot serving, List<ArgC> args)
        {
            this.Bot = serving;
            this.Args = args;
            this.NormalizeArgs();
            try
            {
                bool use_db = false;
                var users = from key in this.Users
                            where key.UserId == msg.From.Id
                            select key;
                if (users.Count() == 0)
                {
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, this.NoKey);
                    return;
                }
                SauceNAO_Acc user = users.FirstOrDefault();
                if (user.LongRemaining == 0 && (DateTime.Now - user.LastRequestTime).Value.TotalHours < 24.0)
                {
                    await this.Bot.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunally your count of searches didn\'t restocked.\'nYou have to wait for it about " + (user.LastRequestTime - DateTime.Now).Value.TotalHours.ToString() + " hours.");
                    return;
                }
                else if (user.LongRemaining != 0 && user.ShortRemaining == 0 && (DateTime.Now - user.LastRequestTime).Value.TotalSeconds < 10.0) Thread.Sleep(new TimeSpan(0, 0, 10));
                JObject results = null;
                System.IO.Stream photo = await serving.Client.DownloadFileAsync(serving.Client.GetFileAsync(msg.Photo[2]?.FileId).Result.FilePath);
                System.String hash = System.String.Empty;
                Task<HttpResponseMessage> th;
                UInt16 i = 0, numres = this.DefNumres;
                try
                { numres = Convert.ToUInt16(ArgC.GetArg(this.Args, "limit").Arg); }
                catch(NullReferenceException)
                { numres = this.DefNumres; }
                do
                {
                    th = this.DoASearchAsync(photo, user.ApiKey, numres);
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Searching");
                    if (th.Result.IsSuccessStatusCode) break;
                    if (this.IsSt429(th.Result)) System.Threading.Thread.Sleep(10 * 1000);
                    else break;
                }
                while (i++ < 5 - 1);
                if (IsSt429(th.Result))
                {
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunally there was too many requests from you.\n" +
                        "Please wait until a next day.");
                    use_db = true;
                }
                if (!use_db)
                {
                    System.String res = th.Result.Content.ReadAsStringAsync().Result;
                    results = JObject.Parse(res);
                    if (results == null)
                    {
                        await Bot.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunatelly we have an error with results.");
                        use_db = true;
                    }
                    if (!use_db)
                    {
                        if (Convert.ToInt32(results["header"]["user_id"]?.Value<int>()) <= 0)
                        {
                            await this.Bot.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunatelly API didn\'t responded.");
                            use_db = true;
                        }
                        if (Convert.ToInt32(results["header"]["results_returned"]?.Value<int>()) <= 0)
                        {
                            await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunatelly SauceNAO didn\'t returned results for this image, sorry…");
                            return;
                        }
                    }
                }
                SearchQuery query = null;
                photo.Seek(0, SeekOrigin.Begin);
                if (use_db)
                {
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Trying to use cashed results for other users.");
                    hash = this.imageHasher.CalculateDifferenceHash64(photo).ToString();
                    var q = this.SearchQueries.Where(rq => rq.ImageHash == hash);
                    if (q.Count() != 0) results = JObject.Parse(q.FirstOrDefault().JSON);
                    else
                    {
                        await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunally nobody didn\'t looked for image that looks like yours.");
                        return;
                    }
                }
                else
                {
                    user.LastRequestTime = DateTime.Now;
                    if (user.AccountType != results["header"]["account_type"].Value<short>()) user.AccountType = results["header"]["account_type"].Value<short>();
                    if (user.SauceNAO_UserId != results["header"]["user_id"].Value<short>()) user.SauceNAO_UserId = results["header"]["user_id"].Value<long>();
                    if (user.LongLimit != results["header"]["long_limit"].Value<ushort>()) user.LongLimit = results["header"]["long_limit"].Value<ushort>();
                    if (user.ShortLimit != results["header"]["short_limit"].Value<ushort>()) user.ShortLimit = results["header"]["short_limit"].Value<ushort>();
                    user.LongRemaining = results["header"]["long_remaining"].Value<ushort>();
                    user.ShortRemaining = results["header"]["short_remaining"].Value<ushort>();
                    query = new SearchQuery
                    {
                        JSON = th.Result.Content.ReadAsStringAsync().Result,
                        MinimumSimularity = results["header"]["minimum_similarity"].Value<float>(),
                        ResultsRequested = results["header"]["results_requested"].Value<ushort>(),
                        ResultsReturned = results["header"]["results_returned"].Value<ushort>(),
                        SearchDepth = results["header"]["search_depth"].Value<System.String>(),
                        SearchStatus = results["header"]["status"].Value<ushort>(),
                        ImageHash = hash,
                        AccountId = user.Id
                    };
                }
                bool inc_low;
                try
                {
                    if (ArgC.GetArg(this.Args, "unsimilar") != null) inc_low = true;
                    else inc_low = false;
                }
                catch(NullReferenceException)
                { inc_low = false; }
                bool unsent = true;
                foreach (var result in results["results"].Children())
                {
                    if (results["header"]["minimum_similarity"].Value<decimal>() > result["header"]["similarity"].Value<decimal>())
                    { if (!inc_low) break; }
                    if (!use_db)
                    {
                        try
                        {
                            System.IO.Stream get_pic = await Client.GetStreamAsync(result["header"]["thumbnail"].Value<String>());
                            await this.Bot.Client.SendPhotoAsync(msg.Chat.Id, new InputOnlineFile(get_pic, result["header"]["thumbnail"].Value<System.String>().Split('/').Last().Split('?')[0]));
                        }
                        catch { }
                    }
                    System.String res_str = System.String.Empty;
                    try
                    {
                        res_str = result["header"]["index_name"].Value<System.String>() + "\nSimilarity " + result["header"]["similarity"].Value<decimal>().ToString() + "\nSource URLs:";
                        if (result["data"]["ext_urls"] == null) res_str += " unfortunally links wasn\'t provided.";
                        else
                        {
                            foreach (var url in result["data"]["ext_urls"].Values<System.String>())
                            {
                                res_str += "\n" + url;
                            }
                        }
                    }
                    catch { }
                    finally
                    {
                        if (results["header"]["minimum_similarity"].Value<decimal>() > result["header"]["similarity"].Value<decimal>()) await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Low similarity result bellow.");
                        await this.Bot.Client.SendTextMessageAsync(msg.Chat.Id, res_str);
                    }
                    unsent = false;
                }
                if (results["results"].Count() == 0) await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunatelly we have no results some how, sorry.");
                if (unsent && !inc_low) await this.Bot.Client.SendTextMessageAsync(msg.Chat.Id, "If results wasn\'t sent, try to send this image with caption \"anipic sauce unsimilar=yes\"");
                //if (unsent && inc_low) await this.Bot.Client.SendTextMessageAsync(msg.Chat.Id, "Sorry but we didn\'t got results for your request.");
                if (!use_db)
                {
                    hash = this.imageHasher.CalculateDifferenceHash64(photo).ToString();
                    query.ImageHash = hash;
                    this.SearchQueries.InsertOnSubmit(query);
                    this.dataContext.SubmitChanges();
                    foreach (var result in results["results"].Children())
                    {
                        SearchResult searchResult = new SearchResult
                        {
                            IndexId = result["header"]["index_id"].Value<ushort>(),
                            IndexName = result["header"]["index_name"].Value<System.String>(),
                            Similarity = result["header"]["similarity"].Value<float>(),
                            Thumbnail = result["header"]["thumbnail"].Value<String>(),
                            SearchId = query.Id
                        };
                        this.SearchResults.InsertOnSubmit(searchResult);
                        this.dataContext.SubmitChanges();
                        if (result["data"]["ext_urls"] != null)
                        {
                            foreach (var url in result["data"]["ext_urls"].Values<System.String>())
                            {
                                this.ExternalUrls.InsertOnSubmit(new ExternalUrls
                                {
                                    URL = url,
                                    ResultId = searchResult.Id
                                });
                            }
                        }
                        this.dataContext.SubmitChanges();

                    }
                    System.String filepath = this.SavePicsDir + hash.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + ".jpg";
                    FileStream file = System.IO.File.Create(filepath);
                    photo.Seek(0, SeekOrigin.Begin);
                    photo.CopyTo(file);
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message != TooManyReq) serving.Exceptions.Add(ex);
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, ex.Message);
            }
        }

        public void SearchPicOnSend(IBot serving, Message msg)
        {
            try
            {
                if (msg.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private && msg.Type == Telegram.Bot.Types.Enums.MessageType.Photo && (msg.Caption == null ? true : (msg.Caption == System.String.Empty || !(msg.Caption.ToLower().Contains("anipic") && msg.Caption.ToLower().Contains("sauce"))))) this.SearchPic(msg, serving, null);
            }
            catch (Exception ex)
            {
                serving.Exceptions.Add(ex);
            }
        }
    }
}