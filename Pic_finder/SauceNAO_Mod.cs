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
            Bitmap to_push = new Bitmap(take);
            System.IO.Stream push = new System.IO.MemoryStream();
            to_push.Save(push, System.Drawing.Imaging.ImageFormat.Png);
            MultipartFormDataContent post_data = new MultipartFormDataContent();
            HttpContent content = new StreamContent(push);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
            post_data.Add(content, "file", "image.png");
            return await Client.PostAsync("http://saucenao.com/search.php?output_type=2&numres=1&minsim=" + minsim + "&dbmask=999" /*+ Convert.ToString(this.db_bitmask)*/ + "&api_key=" + api_key, post_data);
        }

        public async void SearchPic(Message msg, IBot serving, List<ArgC> args)
        {
            try
            {
                System.IO.Stream photo = await serving.Client.DownloadFileAsync(serving.Client.GetFileAsync(msg.Photo.First()?.FileId).Result.FilePath);
                Task<HttpResponseMessage> th;
                UInt16 i = 0;
                do
                {
                    th = this.DoASearchAsync(photo, this.Acc_Key);
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Searching");
                    if (th.Result.IsSuccessStatusCode) break;
                    else if (this.IsSt429(th.Result)) System.Threading.Thread.Sleep(10 * 1000);
                }
                while (i++ < 5 - 1);
                if (IsSt429(th.Result)) throw new Exception(TooManyReq);
                else
                {
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, await th.Result.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                serving.Exceptions.Add(ex);
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, ex.Message, replyToMessageId: msg.MessageId);
            }
        }
    }
}