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

        private bool IsSt429(ref HttpResponseMessage resp)
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
            post_data.Add(new StreamContent(push), "file", "image.png");
            return await Client.PostAsync("http://saucenao.com/search.php?output_type=2&numres=1&minsim=" + minsim + "&dbmask=" + Convert.ToString(this.db_bitmask) + "&api_key=" + api_key, post_data);
        }

        public async void SearchPic(Message msg, IBot serving, List<ArgC> args)
        {
            try
            {
                if (msg.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
                {
                    System.IO.Stream photo = await serving.Client.DownloadFileAsync(serving.Client.GetFileAsync(msg.Photo.LastOrDefault()?.FileId).Result.FilePath);
                    HttpResponseMessage th;
                    UInt16 i = 0;
                    do
                    {
                        th = await this.DoASearchAsync(photo, this.Acc_Key);
                        await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Searching");
                        if (th.IsSuccessStatusCode) break;
                        else if (this.IsSt429(ref th)) System.Threading.Thread.Sleep(10 * 1000);
                    }
                    while (i++ < 5 - 1);
                    if (IsSt429(ref th)) throw new Exception(TooManyReq);
                    else
                    {
                        await serving.Client.SendTextMessageAsync(msg.Chat.Id, await th.Content.ReadAsStringAsync());
                    }

                }
                else await serving.Client.SendTextMessageAsync(msg.Chat.Id, "Please paste a photo and type with it a command \"/getsauce\".", replyToMessageId: msg.MessageId);
            }
            catch (Exception ex)
            {
                serving.Exceptions.Add(ex);
                await serving.Client.SendTextMessageAsync(msg.Chat.Id, ex.Message, replyToMessageId: msg.MessageId);
            }
        }

        internal void OnNewMessage(object sender, NotifyCollectionChangedEventArgs e)
        {
            Message msg = this.Bot.MessagesLast.Last();
            if (msg.Type == Telegram.Bot.Types.Enums.MessageType.Photo && msg.Caption == "sauce") this.SearchPic(msg, this.Bot, null);
        }
    }
}