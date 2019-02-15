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
using HtmlAgilityPack;
using Telegram.Bot.Types.ReplyMarkups;

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
        private System.String ConnStr;
        /*
        public DataContext dataContext;
        private Table<SauceNAO_Acc> Users;
        private Table<SearchQuery> SearchQueries;
        private Table<SearchResult> SearchResults;
        private Table<ExternalUrls> ExternalUrls;
        */
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
            try
            {
                using (SqlConnection connection = new SqlConnection(sql_conn_string)) connection.Open();
            }
            catch (Exception ex)
            {
                System.Environment.Exit(1);
            }
            this.ConnStr = sql_conn_string;
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

        private async Task<HttpResponseMessage> DoTrivialRequest(System.IO.Stream take)
        {
            using (MemoryStream push = new MemoryStream())
            {
                Bitmap to_push = new Bitmap(take);
                to_push.Save(push, System.Drawing.Imaging.ImageFormat.Png);
                MultipartFormDataContent post_data = new MultipartFormDataContent();
                HttpContent content = new ByteArrayContent(push.ToArray());
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
                post_data.Add(content, "file", "image.png");
                return await Client.PostAsync("https://iqdb.org/", post_data);
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
                this.Bot.Exceptions.Add(ex);
                this.Bot.Client.SendTextMessageAsync(this.Msg.Chat.Id, "Oops, something got wrong.\n"+ex.Message);
            }
        }

        public async void AddKeyToDB(Message msg, IBot serving, List<ArgC> args)
        {
            this.Msg = msg;
            this.Bot = serving;
            this.Args = args;
            using (DataContext dataContext = new DataContext(this.ConnStr))
            {
                try
                {
                    Table<SauceNAO_Acc> Users = dataContext.GetTable<SauceNAO_Acc>();
                    System.String api_key;
                    this.NormalizeArgs();
                    try
                    {
                        api_key = ArgC.GetArg(this.Args, "key").Arg;
                    }
                    catch (NullReferenceException)
                    {
                        await serving.Client.SendTextMessageAsync(msg.Chat.Id, "You should put your key in the \"key\" parameter");
                        return;
                    }
                    var users = from u in Users
                                where u.UserId == msg.From.Id
                                select u;
                    if (users.Count<SauceNAO_Acc>() == 0)
                    {
                        Users.InsertOnSubmit(new SauceNAO_Acc
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
                    try
                    {
                        dataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
                    }
                    catch (ChangeConflictException)
                    {
                        foreach (ObjectChangeConflict changeConflict in dataContext.ChangeConflicts)
                            changeConflict.Resolve(RefreshMode.KeepChanges);
                        dataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);
                    }
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Your key is succesfully added(changed) in to my DB.\n" +
                        "Now m̶y̶ ̶m̶a̶s̶t̶e̶r̶ ̶c̶a̶n̶ ̶u̶s̶e̶ ̶i̶t̶ ̶f̶o̶r̶ ̶i̶t̶'̶s̶ ̶o̶w̶n̶ ̶s̶i̶n̶i̶s̶t̶e̶r̶ ̶a̶i̶m̶s̶ you can search for links of originals of images, which you found in social networks.");
                }
                catch (Exception ex)
                {
                    serving.Exceptions.Add(ex);
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, this.GotWrong + ex.Message);
                }
            }  
        }

        public async void DeleteKeyFromDB(Message msg, IBot serving, List<ArgC> args)
        {
            using (DataContext dataContext = new DataContext(this.ConnStr))
            {
                //Table<SearchQuery> SearchQueries = dataContext.GetTable<SearchQuery>();
                //Table<SearchResult> SearchResults = dataContext.GetTable<SearchResult>();
                //Table<ExternalUrls> ExternalUrls = dataContext.GetTable<ExternalUrls>();
                try
                {
                    Table<SauceNAO_Acc> Users = dataContext.GetTable<SauceNAO_Acc>();
                    var users = from u in Users
                                where u.UserId == msg.From.Id
                                select u;
                    if (users.Count() != 0)
                    {
                        Users.DeleteAllOnSubmit(users);
                        try
                        {
                            dataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
                        }
                        catch (ChangeConflictException)
                        {
                            foreach (ObjectChangeConflict changeConflict in dataContext.ChangeConflicts)
                                changeConflict.Resolve(RefreshMode.KeepChanges);
                            dataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);
                        }
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
        }

        public async void GetMySearchStats(Message msg, IBot serving, List<ArgC> args)
        {
            using (DataContext dataContext = new DataContext(this.ConnStr))
            {
                try
                {
                    Table<SauceNAO_Acc> Users = dataContext.GetTable<SauceNAO_Acc>();
                    //Table<SearchQuery> SearchQueries = dataContext.GetTable<SearchQuery>();
                    //Table<SearchResult> SearchResults = dataContext.GetTable<SearchResult>();
                    //Table<ExternalUrls> ExternalUrls = dataContext.GetTable<ExternalUrls>();
                    var users = from u in Users
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
                        try
                        {
                            dataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
                        }
                        catch (ChangeConflictException)
                        {
                            foreach (ObjectChangeConflict changeConflict in dataContext.ChangeConflicts)
                                changeConflict.Resolve(RefreshMode.KeepChanges);
                            dataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);
                        }
                        await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Your stats.\n(Information can be incorrect, because it\'s bases on your last request.)\n" +
                            "Long count of avalieble searches is " + user.LongRemaining.ToString() + " out of " + user.LongLimit.ToString() +
                            ".\nShort count – " + user.ShortRemaining.ToString() + " out of " + user.ShortLimit.ToString() + "." +
                            ((DateTime.Now - user.LastRequestTime).Value.TotalHours < 24.0 && user.LongRemaining == 0 ? "\nNext searches will be avalieble in " + (24.0 - (DateTime.Now - user.LastRequestTime).Value.TotalHours).ToString() + " hours." : ""));
                    }
                    else await serving.Client.SendTextMessageAsync(msg.Chat.Id, this.NoKey);
                }
                catch (Exception ex)
                {
                    this.Bot.Exceptions.Add(ex);
                }
            }
        }

        public async void SearchPic(Message msg, IBot serving, List<ArgC> args)
        {
            if (msg.Type != Telegram.Bot.Types.Enums.MessageType.Photo) return;
            this.Bot = serving;
            this.Args = args;
            this.NormalizeArgs();
            using (DataContext dataContext = new DataContext(this.ConnStr))
            {
                Table<SauceNAO_Acc> Users = dataContext.GetTable<SauceNAO_Acc>();
                try
                {
                    bool use_iqdb = false;
                    var users = from key in Users
                                where key.UserId == msg.From.Id
                                select key;
                    if (users.Count() == 0)
                    {
                        //await serving.Client.SendTextMessageAsync(msg.Chat.Id, this.NoKey);
                        this.SearchWithoutSauceNAO(msg, serving, args);
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
                    if (msg.Photo.Length<=0)
                    {
                        await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Images wasn`t found.\nSorry…");
                        return;
                    }
                    System.IO.Stream photo = await serving.Client.DownloadFileAsync(serving.Client.GetFileAsync(msg.Photo.Last().FileId).Result.FilePath);
                    System.String hash = System.String.Empty;
                    Task<HttpResponseMessage> th;
                    UInt16 i = 0, numres = this.DefNumres;
                    try
                    { numres = Convert.ToUInt16(ArgC.GetArg(this.Args, "limit").Arg); }
                    catch (NullReferenceException)
                    { numres = this.DefNumres; }
                    do
                    {
                        th = this.DoASearchAsync(photo, user.ApiKey, numres);
                        await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Searching…" + (i > 0 ? ("\nNumber of retry – " + i.ToString()) : ""), disableNotification: true);
                        if (th.Result.IsSuccessStatusCode) break;
                        if (this.IsSt429(th.Result)) System.Threading.Thread.Sleep(10 * 1000);
                        else break;
                    }
                    while (i++ < 5 - 1);
                    if (IsSt429(th.Result))
                    {
                        await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunally there was too many requests from you.\n" +
                            "Please wait until a next day.");
                        use_iqdb = true;
                    }
                    if (!use_iqdb)
                    {
                        System.String res = th.Result.Content.ReadAsStringAsync().Result;
                        results = JObject.Parse(res);
                        if (results == null)
                        {
                            await Bot.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunatelly we have an error with results.");
                            use_iqdb = true;
                        }
                        if (!use_iqdb)
                        {
                            if (Convert.ToInt32(results["header"]["user_id"]?.Value<int>()) <= 0)
                            {
                                await this.Bot.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunatelly API didn\'t responded.");
                                use_iqdb = true;
                            }
                            if (Convert.ToInt32(results["header"]["results_returned"]?.Value<int>()) <= 0)
                            {
                                await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunatelly SauceNAO didn\'t returned results for this image, sorry…");
                                return;
                            }
                        }
                        if (results?["results"].Count() == 0)
                        {
                            await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunatelly we have no results some how, sorry…");
                            use_iqdb = true;
                        }
                    }
                    if (use_iqdb)
                    {
                        await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Trying to use the IQDB instead.");
                        this.SearchWithoutSauceNAO(msg, serving, args);
                        return;
                    }
                    user.LastRequestTime = DateTime.Now;
                    if (user.AccountType != results["header"]["account_type"].Value<short>()) user.AccountType = results["header"]["account_type"].Value<short>();
                    if (user.SauceNAO_UserId != results["header"]["user_id"].Value<short>()) user.SauceNAO_UserId = results["header"]["user_id"].Value<long>();
                    if (user.LongLimit != results["header"]["long_limit"].Value<ushort>()) user.LongLimit = results["header"]["long_limit"].Value<ushort>();
                    if (user.ShortLimit != results["header"]["short_limit"].Value<ushort>()) user.ShortLimit = results["header"]["short_limit"].Value<ushort>();
                    user.LongRemaining = results["header"]["long_remaining"].Value<ushort>();
                    user.ShortRemaining = results["header"]["short_remaining"].Value<ushort>();
                    SearchQuery query = new SearchQuery
                    {
                        JSON = th.Result.Content.ReadAsStringAsync().Result,
                        MinimumSimularity = results["header"]["minimum_similarity"].Value<float>(),
                        ResultsRequested = results["header"]["results_requested"].Value<ushort>(),
                        ResultsReturned = results["header"]["results_returned"].Value<ushort>(),
                        SearchDepth = results["header"]["search_depth"].Value<System.String>(),
                        SearchStatus = results["header"]["status"].Value<ushort>(),
                        AccountId = user.Id
                    };
                    Dictionary<SearchResult, List<ExternalUrls>> Results = new Dictionary<SearchResult, List<ExternalUrls>>();
                    foreach (var result in results["results"].Children())
                    {
                        List<ExternalUrls> urls = new List<ExternalUrls>();
                        try
                        {
                            if (result["data"]["ext_urls"].Values<System.String>() != null)
                                foreach (System.String url in result["data"]["ext_urls"].Values<System.String>())
                                    urls.Add(new Pic_finder.ExternalUrls
                                    { URL = url });
                        }
                        catch(NullReferenceException)
                        { }
                        Results.Add(new SearchResult
                        {
                            IndexId = result["header"]["index_id"].Value<ushort>(),
                            IndexName = result["header"]["index_name"].Value<System.String>(),
                            Similarity = result["header"]["similarity"].Value<float>(),
                            Thumbnail = result["header"]["thumbnail"].Value<String>()
                        }, urls);
                    }
                    Dictionary<SearchResult, List<ExternalUrls>> ResSim = new Dictionary<SearchResult, List<ExternalUrls>>();
                    try
                    {
                        if (ArgC.GetArg(args, "unsimilar") != null) this.SendResults(serving, msg, Results);
                        else
                        {
                            ResSim = (Dictionary<SearchResult, List<ExternalUrls>>)Results.Where(res => res.Key.Similarity >= query.MinimumSimularity);
                            this.SendResults(serving, msg, ResSim);
                        }
                    }
                    catch(NullReferenceException)
                    {
                        ResSim = Results.Where(res => res.Key.Similarity >= query.MinimumSimularity).ToDictionary(x => x.Key, x => x.Value);
                        this.SendResults(serving, msg, ResSim);
                    }
                    this.SaveResultsToDB(photo, query, Results);
                    if (Results.Count>ResSim.Count)
                    {
                        await serving.Client.SendTextMessageAsync(
                            msg.Chat.Id,
                            "There are results which marked as unsimilar.",
                            replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton()
                            {
                                Text = "Show it",
                                CallbackData = "action=receive_unsimilar query_id=" + query.Id.ToString()
                            }), disableNotification: true);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message != TooManyReq) serving.Exceptions.Add(ex);
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Oops… Something got wrong…");
                }
            }
        }

        private InlineKeyboardMarkup DownloadFromSourceButtons(List<ExternalUrls> urls)
        {
            try
            {
                List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();
                foreach (ExternalUrls url_db in urls)
                {
                    if (url_db.URL.Contains("danbooru.donmai.us"))
                        buttons.Add(new InlineKeyboardButton[]
                            {new InlineKeyboardButton(){
                                Text = "Download from Danbooru",
                                CallbackData = "action=download_from_danbooru danbooru_id=" + url_db.URL.Split('/').Last()
                            }});
                    if (url_db.URL.Contains("yande.re"))
                        buttons.Add(new InlineKeyboardButton[]
                            {new InlineKeyboardButton(){
                                Text = "Download from Yandere",
                                CallbackData = "action=download_from_yandere yandere_id=" + url_db.URL.Split('/').Last()
                            }});
                    if (url_db.URL.Contains("gelbooru.com") && url_db.URL.Contains("id="))
                        buttons.Add(new InlineKeyboardButton[]
                            {new InlineKeyboardButton(){
                                Text = "Download from Gelbooru",
                                CallbackData = "action=download_from_gelbooru gelbooru_id=" + url_db.URL.Split('&').Where(p => p.Contains("id")).FirstOrDefault().Split('=').Last()
                            }});
                    if (url_db.URL.Contains("konachan.com"))
                        buttons.Add(new InlineKeyboardButton[]
                            {new InlineKeyboardButton(){
                                Text = "Download from Konachan",
                                CallbackData = "action=download_from_konachan konachan_id=" + url_db.URL.Split('/').Last()
                            }});
                }
                if (buttons.Count > 0) return new InlineKeyboardMarkup(buttons.ToArray());
            }
            catch(Exception ex)
            {
                this.Bot.Exceptions.Add(ex);
            }
            return null;
        }

        private async void DownloadSource(IBot serving, Message msg, System.String callback)
        {
            try
            {
                System.String dnb_id = callback.Split(' ')[1].Split('=')[1];
                if (callback.Contains("download_from_danbooru")) serving.GetModule<danbooru_api_mod>().GetDanbooruAsync(msg, serving,
                    new List<ArgC>()
                    {
                        new ArgC("/getdanbooru"),
                        new ArgC("file"),
                        new ArgC("id", dnb_id)
                    });
                if (callback.Contains("download_from_yandere")) serving.GetModule<danbooru_api_mod>().GetYandereAsync(msg, serving,
                    new List<ArgC>()
                    {
                        new ArgC("/getyandere"),
                        new ArgC("file"),
                        new ArgC("id", dnb_id)
                    });
                if (callback.Contains("download_from_gelbooru")) serving.GetModule<danbooru_api_mod>().GetGelboorruAsync(msg, serving,
                    new List<ArgC>()
                    {
                        new ArgC("/getgelbooru"),
                        new ArgC("file"),
                        new ArgC("id", dnb_id)
                    });
                if (callback.Contains("download_from_konachan")) serving.GetModule<danbooru_api_mod>().GetKonachanAsync(msg, serving,
                    new List<ArgC>()
                    {
                        new ArgC("/getkonachan"),
                        new ArgC("file"),
                        new ArgC("id", dnb_id)
                    });
            }
            catch(Exception ex)
            {
                serving.Exceptions.Add(ex);
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Oops… Something got wrong.");
            }
        }

        private async void SendResults(IBot serving, Message msg, Dictionary<SearchResult, List<ExternalUrls>> results)
        {
            this.Bot = serving;
            //if (results == null) return;
            foreach (KeyValuePair<SearchResult, List<ExternalUrls>> result in results)
            {
                int image_msg_id = 0;
                try
                {
                    System.IO.Stream get_pic = await Client.GetStreamAsync(result.Key.Thumbnail);
                    if (get_pic != null)
                        image_msg_id = (await serving.Client.SendPhotoAsync(msg.Chat.Id, new InputOnlineFile(get_pic, result.Key.Thumbnail.Split('/').Last().Split('?')[0]), disableNotification: true)).MessageId;
                }
                catch
                { }
                System.String res_str = System.String.Empty;
                try
                {
                    res_str = result.Key.IndexName + ".\nSimilarity – " + result.Key.Similarity.ToString() + "%.\nSource URLs:";
                    if (result.Value != null ? result.Value.Count == 0 : true) res_str += "\nunfortunally links wasn\'t provided.";
                    else
                    {
                        foreach (var url in result.Value)
                        {
                            res_str += "\n" + url.URL;
                        }
                    }
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, res_str, replyMarkup: this.DownloadFromSourceButtons(result.Value), disableNotification: true, replyToMessageId: image_msg_id);
                }
                catch (Exception ex)
                { serving.Exceptions.Add(ex); }
            }
        }

        private async void SaveResultsToDB(System.IO.Stream photo, SearchQuery query, Dictionary<SearchResult, List<ExternalUrls>> results)
        {
            photo.Seek(0, SeekOrigin.Begin);
            System.String hash = this.imageHasher.CalculateDifferenceHash64(photo).ToString();
            using (DataContext dataContext = new DataContext(this.ConnStr))
            {
                //Table<SauceNAO_Acc> Users = dataContext.GetTable<SauceNAO_Acc>();
                bool db_unppt = false;
                Table<SearchQuery> SearchQueries = dataContext.GetTable<SearchQuery>();
                Table<SearchResult> SearchResults = dataContext.GetTable<SearchResult>();
                Table<ExternalUrls> ExternalUrls = dataContext.GetTable<ExternalUrls>();

                query.ImageHash = hash;
                SearchQueries.InsertOnSubmit(query);
                do
                {
                    try
                    {
                        dataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
                        db_unppt = false;
                    }
                    catch (ChangeConflictException)
                    {
                        foreach (ObjectChangeConflict changeConflict in dataContext.ChangeConflicts)
                            changeConflict.Resolve(RefreshMode.KeepChanges);
                        db_unppt = true;
                    }
                }
                while (db_unppt);
                foreach (KeyValuePair<SearchResult, List<ExternalUrls>> keyValue in results)
                {
                    keyValue.Key.SearchId = query.Id;
                    SearchResults.InsertOnSubmit(keyValue.Key);
                    do
                    {
                        try
                        {
                            dataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
                            db_unppt = false;
                        }
                        catch (ChangeConflictException)
                        {
                            foreach (ObjectChangeConflict changeConflict in dataContext.ChangeConflicts)
                                changeConflict.Resolve(RefreshMode.KeepChanges);
                            db_unppt = true;
                        }
                    }
                    while (db_unppt);
                    foreach (ExternalUrls externalUrls in keyValue.Value)
                    {
                        externalUrls.ResultId = keyValue.Key.Id;
                        ExternalUrls.InsertOnSubmit(externalUrls);
                    }
                    do
                    {
                        try
                        {
                            dataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
                            db_unppt = false;
                        }
                        catch (ChangeConflictException)
                        {
                            foreach (ObjectChangeConflict changeConflict in dataContext.ChangeConflicts)
                                changeConflict.Resolve(RefreshMode.KeepChanges);
                            db_unppt = true;
                        }
                    }
                    while (db_unppt);
                }
            }
            System.String filepath = this.SavePicsDir + hash.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + ".jpg";
            FileStream file = System.IO.File.Create(filepath);
            photo.Seek(0, SeekOrigin.Begin);
            await photo.CopyToAsync(file);
            file.Close();
        }

        private async void SendUnsimilar(IBot serving, Message msg, System.String Callback)
        {
            using (DataContext dataContext = new DataContext(this.ConnStr))
            {
                try
                {
                    Table<SearchQuery> SearchQueries = dataContext.GetTable<SearchQuery>();
                    Table<SearchResult> SearchResults = dataContext.GetTable<SearchResult>();
                    Table<ExternalUrls> ExternalUrls = dataContext.GetTable<ExternalUrls>();

                    var queries = from q in SearchQueries
                                  where q.Id == Convert.ToInt32(Callback.Split(' ')[1].Split('=')[1])
                                  select q;
                    if (queries.Count() != 0)
                    {
                        SearchQuery query = queries.FirstOrDefault();
                        Dictionary<SearchResult, List<ExternalUrls>> results = new Dictionary<SearchResult, List<ExternalUrls>>();
                        var results_db = from r in SearchResults
                                         where query.Id == r.SearchId && r.Similarity < query.MinimumSimularity
                                         select r;
                        foreach (var res_db in results_db)
                        {
                            var urls_db = from u in ExternalUrls
                                          where res_db.Id == u.ResultId
                                          select u;
                            results.Add(res_db, urls_db.ToList());
                        }
                        this.SendResults(serving, msg, results);
                    }
                    else await serving.Client.SendTextMessageAsync(msg.Chat.Id, "There is no query with this Id.");
                }
                catch (Exception ex)
                {
                    serving.Exceptions.Add(ex);
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Oops… Something got wrong.");
                }
            }
        }

        public async void SearchPicOnSend(Update update, IBot serving, List<ArgC> args)
        {
            switch (update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    this.SearchPicOnSend(update.Message, serving, args);
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                    if (update.CallbackQuery.Data.Contains("receive_unsimilar")) this.SendUnsimilar(serving, update.CallbackQuery.Message, update.CallbackQuery.Data);
                    if (update.CallbackQuery.Data.Contains("action=download_from"))
                        this.DownloadSource(serving, update.CallbackQuery.Message, update.CallbackQuery.Data);
                    break;
            }
        }

        public async void SearchPicOnSend(Message msg, IBot serving, List<ArgC> args = null)
        {
            try
            {
                if (msg.ReplyToMessage!=null && (msg.Text!=null || msg.Caption!=null))
                {
                    System.String inc = msg.Text ?? System.String.Empty;
                    inc += msg.Caption ?? System.String.Empty;
                    if (msg.ReplyToMessage.Type == Telegram.Bot.Types.Enums.MessageType.Photo && inc.ToLower().Contains("anipic") && inc.ToLower().Contains("sauce")) this.SearchPic(msg.ReplyToMessage, serving, args);
                }
                if (msg.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private && msg.Type == Telegram.Bot.Types.Enums.MessageType.Photo && (msg.Caption == null ? true : (msg.Caption == System.String.Empty || !(msg.Caption.ToLower().Contains("anipic") && msg.Caption.ToLower().Contains("sauce"))))) this.SearchPic(msg, serving, args);
            }
            catch //(Exception ex)
            {
                //serving.Exceptions.Add(ex);
            }
        }

        public async void SearchWithoutSauceNAO(Message msg, IBot serving, List<ArgC> args)
        {
            try
            {
                if (msg.Photo.Count() <= 0)
                {
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Images wasn`t found.\nSorry…");
                    return;
                }
                System.IO.Stream photo = await serving.Client.DownloadFileAsync(serving.Client.GetFileAsync(msg.Photo.Last().FileId).Result.FilePath);
                HttpResponseMessage httpResponse = await this.DoTrivialRequest(photo);
                System.String res = await httpResponse.Content.ReadAsStringAsync();
                if (!httpResponse.IsSuccessStatusCode)
                {
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Oops, something got wrong…");
                    return;
                }
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Searching");
                HtmlDocument parse = new HtmlDocument();
                parse.LoadHtml(res);
                HtmlNodeCollection pages = parse.DocumentNode.SelectNodes("//div[contains(@id, 'pages')]/div");
                SearchQuery query = new SearchQuery
                {
                    JSON = res,
                    MinimumSimularity = 0,
                    ResultsRequested = 0,
                    SearchDepth = null,
                    ResultsReturned = Convert.ToUInt16(pages.Count - 1),
                    AccountId = null
                };
                Dictionary<SearchResult, List<ExternalUrls>> Results = new Dictionary<SearchResult, List<ExternalUrls>>();
                foreach (HtmlNode page in pages)
                {
                    try
                    {
                        if (page == pages[0]) continue;
                        /*
                        System.String link = "http:" + page.SelectSingleNode("table/tbody/tr/td[@class='image']/a").Attributes.GetNamedItem("href").Value;
                        */
                        HtmlNodeCollection rows = page.ChildNodes["table"].ChildNodes;
                        if (page == pages[1])
                        {
                            System.String ch = rows[0].ChildNodes[0].InnerText;
                            if (ch == "No relevant matches")
                            {
                                query.SearchStatus = 1;
                                query.ResultsReturned = Convert.ToUInt16(query.ResultsReturned - 1);
                                await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunatelly there`s no images that close to your`s, so the`re some of them, that can possibly match.");
                                continue;
                            }
                            else { query.SearchStatus = 0; }
                        }
                        Results.Add(new SearchResult
                        {
                            IndexId = 0,
                            IndexName = rows[2].ChildNodes[0].InnerText,
                            Similarity = Convert.ToInt16(new String(rows[4].ChildNodes[0].InnerText.Where(Char.IsDigit).ToArray())),
                            Thumbnail = "https://iqdb.org/" + rows[1].ChildNodes["td"].ChildNodes["a"].ChildNodes["img"].GetAttributeValue("src", "")
                        },
                        new List<ExternalUrls>()
                        {
                        new ExternalUrls
                        {
                            URL ="https:" + rows[1].ChildNodes["td"].ChildNodes["a"].GetAttributeValue("href", "")
                        }
                        });
                        if (rows[2].ChildNodes[0].HasChildNodes)
                        {
                            if (rows[2].ChildNodes[0].ChildNodes["span"] != null)
                            {
                                if (rows[2].ChildNodes[0].ChildNodes["span"].HasChildNodes)
                                {
                                    foreach (HtmlNode htmlNode in rows[2].ChildNodes[0].ChildNodes["span"].ChildNodes)
                                    {
                                        Results.Last().Value.Add(new ExternalUrls
                                        {
                                            URL = "https:" + htmlNode.GetAttributeValue("href", "")
                                        });
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        serving.Exceptions.Add(e);
                    }
                }
                this.SendResults(serving, msg, Results);
                this.SaveResultsToDB(photo, query, Results);
            }
            catch (Exception e)
            {
                serving.Exceptions.Add(e);
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Oops… Something got wrong.");
            }
        }
    }
}