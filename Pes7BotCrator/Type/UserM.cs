using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Type
{
    public class UserM : User
    {
        public string PhotoPath { get; set; } = null;
        public int MessageCount { get; set; }
        public List<long> FromChat { get; set; }
        public List<MessageM> Messages { get; set; }
        public bool Stack { get; set; }
        public UserM(User us, bool stackMessages = false, int i = 0) : base()
        {
            this.FromChat = new List<long>();
            this.Username = us.Username;
            this.LastName = us.LastName;
            this.Id = us.Id;
            this.FirstName = us.FirstName;
            MessageCount = i;
            Stack = stackMessages;
            if (stackMessages)
                Messages = new List<MessageM>();
        }
        public async Task<bool> DownloadImageToDirectory(IBot Parent, bool isOverride = false)
        {
            if (!isOverride)
            {
                if (System.IO.File.Exists($"./UserPhotoes/{this.Id}.jpg"))
                {
                    PhotoPath = $"./UserPhotoes/{this.Id}.jpg";
                    return true;
                }
            }
            var photo = await Parent.Client.GetUserProfilePhotosAsync(this.Id, 0, 1);
            if (photo?.Photos.Length > 0)
            {
                if (photo.Photos.First() != null)
                {
                    if (!Directory.Exists("./UserPhotoes"))
                        Directory.CreateDirectory("./UserPhotoes");

                    var file = await Parent.Client.GetFileAsync(photo.Photos.First().First().FileId);

                    var PhotoPath = $"./UserPhotoes/{this.Id}.jpg";

                    using (var saveImageStream = System.IO.File.Open(PhotoPath, FileMode.Create))
                    {
                        await Parent.Client.DownloadFileAsync(file.FilePath, saveImageStream);
                    }
                    return true;
                }
                else return false;
            }
            else return false;
        }
        public static string nameGet(User us)
        {
            return $"{us.FirstName} {us.LastName}";
        }
        public static string usernameGet(User us)
        {
            if(us!= null)
                return us?.Username != null ? us.Username : $"{us.FirstName} {us.LastName}";
            return null;
        }
    }
}
