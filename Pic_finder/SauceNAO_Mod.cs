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
    public class SauceNAO_Mod:Module
    {
        private HttpClient Client;
        public System.String Acc_Key;
        private UInt16 sh_rem;
        private UInt16 ln_rem;

        private System.String TooManyReq;

        public SauceNAO_Mod(System.String acc_key):base("SauceNAO finder", typeof(SauceNAO_Mod))
        {
            this.Client = new HttpClient();
            this.Acc_Key = acc_key;
            Modulle = this;
            TooManyReq = "Unfortunatelly you had reached out from search limit.\nPlease try again in next day.";
        }

        private async Task<HttpResponseMessage>DoASearchAsync(System.IO.Stream take, System.String api_key)
        {
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
                index_yandere = "1",

                minsim = "80";
            int db_bitmask = Convert.ToInt32(index_yandere + index_nijie + index_drawr + index_danbooru + index_seigaillust + index_anime + index_pixivhistorical + index_pixiv + index_ddbsamples + index_ddbobjects + index_hcg + index_hanime + index_hmags, 2);
            Bitmap to_push = new Bitmap(take);
            System.IO.Stream push = new System.IO.MemoryStream();
            to_push.Save(push, System.Drawing.Imaging.ImageFormat.Png);
            MultipartFormDataContent post_data = new MultipartFormDataContent();
            post_data.Add(new StreamContent(push), "file", "image.png");
            return await Client.PostAsync("http://saucenao.com/search.php?output_type=2&numres=1&minsim=" + minsim + "&dbmask=" + Convert.ToString(db_bitmask) + "&api_key=" + api_key, post_data);
        }

        private bool IsSt429(ref HttpResponseMessage resp)
        {
            return resp.StatusCode.ToString().Contains("429");
        }

        private async void PrintRes(System.IO.Stream proc, BotInteface serv, ChatId ch_id, int rep = 0)
        {
            HttpResponseMessage th;
            UInt16 i = 0;
            do
            {
                th = await DoASearchAsync(proc, Acc_Key);
                if (IsSt429(ref th)) System.Threading.Thread.Sleep(10 * 1000);
                else break;
            }
            while (IsSt429(ref th) && i++ < 5);
            if (IsSt429(ref th))
            {
                await serv.Client.SendTextMessageAsync(ch_id, TooManyReq, replyToMessageId: rep);
                throw new Exception(TooManyReq);
            }
            else await serv.Client.SendTextMessageAsync(ch_id, await th.Content.ReadAsStringAsync(), replyToMessageId: rep);
        }

        public async void SearchPic(Message msg, BotInteface serving, List<ArgC> args)
        {
            foreach(var ph in msg.Photo)
            {
                try
                {
                    PrintRes(ph.FileStream, serving, msg.Chat.Id, msg.MessageId);
                }
                catch(Exception ex)
                {
                    await serving.Client.SendTextMessageAsync(msg.Chat.Id, ex.Message, replyToMessageId: msg.MessageId);
                    break;
                }
            }
        }
    }
}
