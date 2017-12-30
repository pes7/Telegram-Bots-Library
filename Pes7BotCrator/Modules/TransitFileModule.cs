using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Modules
{
    public class TransitFileModule : Module
    {
        public string Dir { get; set; }
        public DownloadCommand DownloadCommandSynk { get; set; }
        public TransitFileModule(string dir) : base("TransitFileModule", typeof(TransitFileModule))
        {
            Dir = dir;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            DownloadCommandSynk = new DownloadCommand();
        }

        public class DownloadCommand : SynkCommand
        {
            public DownloadCommand() : base(ActAsync, new List<string> { "/down" }, "Download file with `url`") { }
        }

        public static void ActAsync(Telegram.Bot.Types.Message re, IBot Parent, List<ArgC> args)
        {
            if (args != null)
            {
                ArgC url = args.Find(sn => sn.Name.Trim() == "default");
                if (url != null)
                {
                    using (var client = new WebClient())
                    {
                        //Ошибка в создании превьюшки
                        var content = client.DownloadData(url.Arg);
                        string dir = Parent.GetModule<TransitFileModule>().Dir;
                        string[] d = url.Arg.Split('/');
                        var path = $"{dir}/{d[d.Length - 1]}";

                        Thread th = new Thread(async () =>
                        {
                            if (!System.IO.File.Exists(path))
                            {
                                using (FileStream sourceStream = new FileStream(path,
                                    FileMode.Append, FileAccess.Write, FileShare.None,
                                    bufferSize: 4096 * 4, useAsync: true))
                                {
                                    await sourceStream.WriteAsync(content, 0, content.Length);
                                    sourceStream.Close();
                                };
                            }
                            using (var iostr = System.IO.File.Open(path, FileMode.Open))
                            {
                                Parent.Client.SendDocumentAsync(re.Chat.Id, new FileToSend(Path.GetFileName(path), iostr));
                            }
                        });
                        th.Start();
                    }
                }
            }
        }

        private static void GetPreViewOfFile(string videopath, string dir)
        {
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            var name = $"{dir}/{Path.GetFileName(videopath)}";
            ffMpeg.GetVideoThumbnail(videopath, $"{name}.jpg", 5);
        }
    }
}
