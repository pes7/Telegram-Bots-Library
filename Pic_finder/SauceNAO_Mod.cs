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

namespace Pic_finder
{
    internal class SauceNAO_Mod : Module
    {
        public IBot Bot;
        private HttpClient Client = new HttpClient();
        private System.String Acc_Key;
        private int db_bitmask;
        private readonly System.String TooManyReq = "Unfortunatelly you had reached out from search limit.\nPlease try again in next day.";

        public SauceNAO_Mod(string acc_key) : base("SauceNAO finder", typeof(SauceNAO_Mod))
        {
            this.Acc_Key = acc_key;
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

        public async void SearchPic(Message msg, IBot serving, List<ArgC> args)
        {
            this.Bot = serving;
            try
            {
                dynamic results = null;
                System.IO.Stream photo = await serving.Client.DownloadFileAsync(serving.Client.GetFileAsync(msg.Photo.First()?.FileId).Result.FilePath);
                Task<HttpResponseMessage> th;
                UInt16 i = 0;
                do
                {
                    th = this.DoASearchAsync(photo, this.Acc_Key);
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Searching");
                    if (th.Result.IsSuccessStatusCode) break;
                    if (this.IsSt429(th.Result)) System.Threading.Thread.Sleep(10 * 1000);
                    else break;
                }
                while (i++ < 5 - 1);
                if (IsSt429(th.Result)) throw new Exception(TooManyReq);
                results = JsonConvert.DeserializeObject(th.Result.Content.ReadAsStringAsync().Result);
                if (Convert.ToInt32(results["header"]["user_id"])<=0)
                {
                    await this.Bot.Client.SendTextMessageAsync(msg.Chat.Id, "Unfortunatelly API didn\'t responded.");
                    return;
                }
                foreach(var result in results["results"])
                {
                    /*if (Convert.ToDecimal(results["header"]["minimum_similarity"]) > Convert.ToDecimal(result["header"]["similarity"])) continue;
                    System.IO.Stream get_pic = await Client.GetStreamAsync(result["header"]["thumbnail"]);
                    await this.Bot.Client.SendPhotoAsync(msg.Chat.Id, new InputOnlineFile(get_pic, result["header"]["thumbnail"].Split('/').Last().Split('?')[0]), replyToMessageId: this.Msg.MessageId);
                    */
                    System.String res_str = /*result["header"]["index_name"] +*/ "Source URLs:";
                    foreach (var url in result["data"]["ext_urls"]) res_str += "\n" + url;
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