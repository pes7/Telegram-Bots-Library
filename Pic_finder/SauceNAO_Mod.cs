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

namespace Pic_finder
{
    [Table(Name = "SauceNAO_accs")]
    internal class SauceNAO_Acc
    {
        [Column(Name="Id", IsPrimaryKey = true, IsDbGenerated = true, DbType = "BigInt Primary Key identity")]
        public long Id { get; }
        [Column(Name = "UserId", DbType = "BigInt")]
        public long UserId;
        [Column(Name = "SauceNAO_UserId", DbType = "BigInt")]
        public long SauceNAO_UserId;
        [Column(Name = "Acc_type", DbType = "tinyint")]
        public Int16 AccountType;
        [Column(Name = "ApiKey", DbType = "varchar(40)")]
        public System.String ApiKey;
        [Column(Name = "LongLimit", DbType = "SmallInt")]
        public UInt16 LongLimit;
        [Column(Name = "LongRemaining", DbType = "SmallInt")]
        public UInt16 LongRemaining;
        [Column(Name = "ShortLimit", DbType = "TinyInt")]
        public UInt16 ShortLimit;
        [Column(Name = "ShortRemaining", DbType = "TinyInt")]
        public UInt16 ShortRemaining;
        [Column(Name = "LastRequest", DbType = "DateTime")]
        public System.DateTime LastRequestTime;
        private EntitySet<SearchQuery> _Queries;
        [Association(Storage = "_Queries", OtherKey ="Id")]
        public EntitySet<SearchQuery> Queries
        {
            get { return this._Queries; }
            set { this._Queries.Assign(value); }
        }
    }

    [Table(Name = "SauceSearch")]
    internal class SearchQuery
    {
        private EntityRef<SauceNAO_Acc> AccountRow;
        private EntitySet<SearchResult> _Results;
        [Column(Name = "Id", IsPrimaryKey = true, IsDbGenerated = true, DbType = "BigInt Primary Key identity")]
        public int Id { get; }
        [Column(Name = "AccId", DbType = "BigInt")]
        public long AccountId;
        [Association(Storage = "AccountRow", ThisKey = "AccountId", IsForeignKey =true)]
        public SauceNAO_Acc SauceNAO_Account
        {
            get { return this.AccountRow.Entity; }
            set { this.AccountRow.Entity = value; }
        }
        [Column(Name = "ImageHash", DbType = "varchar(32)")]
        public System.String ImageHash;
        [Column(Name = "SearchStatus", DbType = "tinyint")]
        public UInt16 SearchStatus;
        [Column(Name = "ResultsRequested", DbType = "tinyint")]
        public UInt16 ResultsRequested;
        [Column(Name = "SearchDepth", DbType = "varchar(4)")]
        public System.String SearchDepth;
        [Column(Name = "MinimumSimularity", DbType = "real")]
        public float MinimumSimularity;
        [Column(Name = "ResultsReturned", DbType = "tinyint")]
        public UInt16 ResultsReturned;
        [Column(Name = "JSONresp", DbType = "text")]
        public System.String JSON;
        [Association(Storage = "_Results", OtherKey = "Id")]
        public EntitySet<SearchResult> Results
        {
            get { return this._Results; }
            set { this._Results.Assign(value); }
        }
    }

    [Table(Name = "SauceSearchResult")]
    internal class SearchResult
    {
        private long _QueryId;
        [Column(Name = "Id", IsPrimaryKey = true, IsDbGenerated = true, DbType = "BigInt Primary Key identity")]
        public long Id { get; }
        [Column(Name = "SearchId", DbType = "BigInt")]
        public long SearchId;
        [Column(Name = "Similarity", DbType = "real")]
        public float Similarity;
        [Column(Name = "Thumbnail", DbType = "Text")]
        public System.String Thumbnail;
        [Column(Name = "IndexId", DbType = "tinyint")]
        public UInt16 IndexId;
        [Column(Name = "IndexName", DbType = "varchar(50)")]
        public System.String IndexName;
        private EntityRef<SearchQuery> MainQuery;
        [Association(IsForeignKey =true, Storage = "MainQuery", ThisKey = "SearchId")]
        public SearchQuery Query
        {
            get { return this.MainQuery.Entity; }
            set { this.MainQuery.Entity = value; }
        }
        private EntitySet<ExternalUrls> ExternalUrls;
        [Association(Storage = "ExternalUrls", OtherKey ="Id")]
        public EntitySet<ExternalUrls> Urls
        {
            get { return this.ExternalUrls; }
            set { this.ExternalUrls.Assign(value); }
        }
    }

    [Table(Name = "ExtURLs_of_Search")]
    internal class ExternalUrls
    {
        [Column(Name = "Id", DbType = "BigInt Primary Key identity", IsDbGenerated = true, IsPrimaryKey = true)]
        public long Id { get; }
        [Column(Name = "ResId", DbType = "BigInt")]
        public long ResultId;
        [Column(Name = "ResultURL", DbType = "text")]
        public System.String URL;
        private EntityRef<SearchResult> MainResult;
        [Association(IsForeignKey =true, Storage = "MainResult", ThisKey = "ResultId")]
        public SearchResult Search
        {
            get { return this.MainResult.Entity; }
            set { this.MainResult.Entity = value; }
        }
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
        private int db_bitmask;
        private readonly System.String TooManyReq = "Unfortunatelly you had reached out from search limit.\nPlease try again in next day.";

        public SauceNAO_Mod(SqlConnection sql) : base("SauceNAO finder", typeof(SauceNAO_Mod))
        {
            if (sql != null)
            { if (sql.State == System.Data.ConnectionState.Broken || sql.State == System.Data.ConnectionState.Closed) return; }
            else return;
            this.dataContext = new DataContext(sql);
            this.Users = this.dataContext.GetTable<SauceNAO_Acc>();
            this.SearchQueries= this.dataContext.GetTable<SearchQuery>();
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
        }

        private bool IsSt429(HttpResponseMessage resp)
        {
            return resp.StatusCode.ToString().Contains("429");
        }


        private async Task<HttpResponseMessage> DoASearchAsync(System.IO.Stream take, System.String api_key)
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
                return await Client.PostAsync("http://saucenao.com/search.php?output_type=2&numres=1&minsim=" + minsim + "&dbmask=999" /*+ Convert.ToString(this.db_bitmask)*/ + "&api_key=" + api_key, post_data);
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
                    this.Args.RemoveAt(0);
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
            this.NormalizeArgs();
            var users = from u in this.Users
                        where u.UserId == msg.From.Id
                        select u;
            if (users.Count<SauceNAO_Acc>() == 0)
            {
                SauceNAO_Acc acc = new SauceNAO_Acc
                {
                    UserId = this.Msg.From.Id,
                    SauceNAO_UserId = 0,
                    AccountType = 0,
                    ApiKey = ArgC.GetArg(this.Args, "key").Arg,
                    LongLimit = 1,
                    LongRemaining = 1,
                    ShortLimit = 1,
                    ShortRemaining = 1,
                    LastRequestTime = DateTime.Now
                };
                this.Users.InsertOnSubmit(acc);
            }
            else users.FirstOrDefault().ApiKey = ArgC.GetArg(this.Args, "key").Arg;
            this.dataContext.SubmitChanges();
        }

        public async void SearchPic(Message msg, IBot serving, List<ArgC> args)
        {
            this.Bot = serving;
            try
            {
                var keys = from key in this.Users
                           where key.UserId == msg.From.Id
                           select key.ApiKey;
                if (keys.Count<System.String>() == 0)
                {
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "You doesnt registered your key");
                    return;
                }

                JObject results = null;
                System.IO.Stream photo = await serving.Client.DownloadFileAsync(serving.Client.GetFileAsync(msg.Photo.First()?.FileId).Result.FilePath);
                Task<HttpResponseMessage> th;
                UInt16 i = 0;
                do
                {
                    th = this.DoASearchAsync(photo, keys.FirstOrDefault());
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Searching");
                    if (th.Result.IsSuccessStatusCode) break;
                    if (this.IsSt429(th.Result)) System.Threading.Thread.Sleep(10 * 1000);
                    else break;
                }
                while (i++ < 5 - 1);
                if (IsSt429(th.Result)) throw new Exception(TooManyReq);
                results = JObject.Parse(th.Result.Content.ReadAsStringAsync().Result);
                if (results == null)
                {
                    await Bot.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunatelly we have an error with results.");
                    return;
                }
                if (Convert.ToInt32(results["header"]["user_id"].Value<int>()) <= 0)
                {
                    await this.Bot.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunatelly API didn\'t responded.");
                    return;
                }
                foreach (var result in results["results"].Children())
                {
                    if (results["header"]["minimum_similarity"].Value<decimal>() > result["header"]["similarity"].Value<decimal>()) continue;
                    System.IO.Stream get_pic = await Client.GetStreamAsync(result["header"]["thumbnail"].Value<String>());
                    await this.Bot.Client.SendPhotoAsync(msg.Chat.Id, new InputOnlineFile(get_pic, result["header"]["thumbnail"].Value<System.String>().Split('/').Last().Split('?')[0]), replyToMessageId: msg.MessageId);
                    System.String res_str = result["header"]["index_name"].Value<System.String>() + "Source URLs:";
                    foreach (var url in result["data"]["ext_urls"].Values<System.String>()) res_str += "\n" + url;
                    await this.Bot.Client.SendTextMessageAsync(msg.Chat.Id, res_str);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message != TooManyReq) serving.Exceptions.Add(ex);
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, ex.Message);
            }
        }
    }
}