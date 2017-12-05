using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Systems
{
    public class DownloadFile
    {
        public async Action<bool> DownloadFromTelegramTo(File file, string path)
        {
            return true;
        }
        public async Action<bool> DownloadFromTelegramTo(long id, string path)
        {
            return true;
        }
        public async Action<bool> DownloadFromTelegramTo(PhotoSize photo, string path)
        {
            return true;
        }
        public async Action<bool> DownloadFromUrlTo(string url, string path)
        {
            return true;
        }
    }
}
