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
        public UserM(User us, int i = 0) : base()
        {
            this.Username = us.Username;
            this.LastName = us.LastName;
            this.Id = us.Id;
            this.FirstName = us.FirstName;
            MessageCount = i;
        }
        public async Task<bool> DownloadImageToDirectory(IBotBase Parent, bool isOverride = false)
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
            if (photo?.Photos.First() != null)
            {
                if (!Directory.Exists("./UserPhotoes"))
                Directory.CreateDirectory("./UserPhotoes");
                using (var file = System.IO.File.Create($"./UserPhotoes/{this.Id}.jpg", 32 * 8 * 1024, FileOptions.Asynchronous))
                {
                    file.Flush(true);
                    await Parent.Client.GetFileAsync(photo.Photos.First().First().FileId, file);
                }
                PhotoPath = $"./UserPhotoes/{this.Id}.jpg";
                return true;
            }
            else return false;
        }
        public static string nameGet(User us)
        {
            return $"{us.FirstName} {us.LastName}";
        }
        public static string usernameGet(User us)
        {
            return us.Username != null ? us.Username : $"{us.FirstName} {us.LastName}";
        }
    }
}
