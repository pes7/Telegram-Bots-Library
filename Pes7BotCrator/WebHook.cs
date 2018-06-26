using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Modules;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace Pes7BotCrator
{
    public class WebHook
    {
        private IBot Parent { get; set; }
        public WebHook(IBot parent)
        {
            Parent = parent;
            Parent.Client.SetWebhookAsync("");
            PreDefineAllwaysInWebHook();
        }
        private int time_noConnect = 0;
        public async void Start()
        {
            int offset = 0;
            while (true)
            {
                try
                {
                    var update = await Parent.Client.GetUpdatesAsync(offset);
                    time_noConnect = 0;
                    foreach (var up in update)
                    {
                        offset = up.Id + 1;
                        new Thread(() => {
                            try
                            {
                                MessageSynk(up);
                            }
                            catch (Exception ex){ Parent.Exceptions.Add(ex); }
                        }).Start();
                    }
                }
                catch
                {
                    Parent.Exceptions.Add(new Exception($"NOPE of Internet Acess {{{time_noConnect}}} sec."));
                    time_noConnect += 10;
                    Thread.Sleep(10000);
                }
            }
        }

        private const int messageCountCheck = 10;
        private int cc = 0;
        private async void MessageSynk(Update Up) {
            Message ms;
            ms = Up.Message;
            var args = ArgC.getArgs(ms?.Text, Parent);
            var str = (args == null) ? ms?.Text : args.First().Name.Trim();
            var com = tryToParseNameBotCommand(ms?.Text);
            switch (Up.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.MessageUpdate:
                    switch (Up.Message.Type) {
                        case Telegram.Bot.Types.Enums.MessageType.VoiceMessage:
                            /* Скачует хорошо, но вот роспознать проблема.
                            var voice = Up.Message.Voice;
                            var file = await Parent.Client.GetFileAsync(voice.FileId);
                            if (!Directory.Exists("Voice"))
                                Directory.CreateDirectory("Voice");
                            var filename = $"Voice/{voice.FileId}.{voice.FileId.Split('.').Last()}.mp3";
                            using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                            {
                                await file.FileStream.CopyToAsync(saveImageStream);
                            }
                            rec(file.FileStream);
                            */
                            /* Роспозновалка, нужно сконвертить mp3 в waw что бы заработала.
                             public void rec(Stream source)
                            {
                                using (SpeechRecognitionEngine recognizer =
                                    new SpeechRecognitionEngine(
                                        new System.Globalization.CultureInfo("en-US")))
                                {
                                    recognizer.LoadGrammar(new DictationGrammar());
                                    recognizer.SetInputToAudioStream(source,new System.Speech.AudioFormat.SpeechAudioFormatInfo(8000,System.Speech.AudioFormat.AudioBitsPerSample.Eight,System.Speech.AudioFormat.AudioChannel.Mono));
                                    recognizer.InitialSilenceTimeout = TimeSpan.FromSeconds(2);
                                    RecognitionResult result = recognizer.Recognize();

                                    if (result != null)
                                    {
                                        Parent.Exceptions.Add(new Exception(result.Text));
                                    }
                                    else
                                    {
                                        Parent.Exceptions.Add(new Exception("SHIT"));
                                    }
                                }
                            } 
                             */
                            //} catch (Exception ex) { Parent.Exceptions.Add(ex); }
                            break;
                        case Telegram.Bot.Types.Enums.MessageType.PhotoMessage:
                            SimpleMessageHere(ms);
                            foreach (SynkCommand sy in Photo_commands.Where(
                                fn => (fn.CommandLine.Exists(sn => sn == str || (sn == com && com != null)) ||
                                fn.CommandName.ToUpper() == args?.ElementAt(0)?.Name.ToUpper() && fn.CommandName != null)
                                ))
                            {
                                if (sy.TypeOfAccess == TypeOfAccess.Admin && ms.Chat.Type != Telegram.Bot.Types.Enums.ChatType.Private)
                                {
                                    var ty = await IsAdminAsync(Parent, ms.Chat.Id, ms.From.Id);
                                    if (ty == false)
                                    {
                                        await Parent.GetModule<TRM>().SendTimeRelayMessageAsynkAsync(ms.Chat.Id, $"You, @{ms.From.Username}, not have access to this command.", 10);
                                        break;
                                    }
                                    else if (sy.TypeOfAccess == TypeOfAccess.Named)
                                    {
                                        if (Parent.UserNameOfCreator != ms.From.Username)
                                        {
                                            await Parent.GetModule<TRM>().SendTimeRelayMessageAsynkAsync(ms.Chat.Id, $"You, @{ms.From.Username}, not have access to this command.", 10);
                                            break;
                                        }
                                    }
                                }
                                await BotBase.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
                                sy.doFunc.DynamicInvoke(ms, Parent, args);
                                break;
                            }
                            break;
                        case Telegram.Bot.Types.Enums.MessageType.TextMessage:
                            SimpleMessageHere(ms);
                            foreach (SynkCommand sy in Standart_commands.Where(
                                fn => (fn.CommandLine.Exists(sn => sn == str || (sn == com && com != null)) ||
                                fn.CommandName?.ToUpper() == args?.ElementAt(0)?.Name?.ToUpper() && fn.CommandName != null)
                                ))
                            {
                                /*Рефакторнуть код*/
                                if (sy.TypeOfAccess == TypeOfAccess.Admin && ms.Chat.Type != Telegram.Bot.Types.Enums.ChatType.Private)
                                {
                                    var ty = await IsAdminAsync(Parent, ms.Chat.Id, ms.From.Id);
                                    if (ty == false)
                                    {
                                        await Parent.GetModule<TRM>().SendTimeRelayMessageAsynkAsync(ms.Chat.Id, $"You, @{ms.From.Username}, not have access to this command.", 10);
                                        break;
                                    }
                                } else if (sy.TypeOfAccess == TypeOfAccess.Named)
                                {
                                    if (Parent.UserNameOfCreator != ms.From.Username)
                                    {
                                        await Parent.GetModule<TRM>().SendTimeRelayMessageAsynkAsync(ms.Chat.Id, $"You, @{ms.From.Username}, not have access to this command.", 10);
                                        break;
                                    }
                                }
                                await BotBase.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
                                sy.doFunc.DynamicInvoke(ms, Parent, args);
                                break;
                            }
                            try
                            {
                                Parent.SynkCommands.Find(fn => fn.CommandLine.Exists(nf => nf == "Default"))?.doFunc.DynamicInvoke(ms, Parent, args);
                            } catch { }
                            cc++;
                            break;
                        case Telegram.Bot.Types.Enums.MessageType.StickerMessage:

                            break;
                        case Telegram.Bot.Types.Enums.MessageType.ServiceMessage:
                            foreach (SynkCommand sy in Service_commands)
                            {
                                doInNewThread(() =>
                                {
                                    sy.doFunc.DynamicInvoke(Up, Parent);
                                });
                            }
                            break;
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQueryUpdate:
                    CallbackQuery qq = Up.CallbackQuery;
                    foreach (SynkCommand sy in Query_commands.Where(fn => fn.CommandLine.Exists(nf => Up.CallbackQuery.Data.Contains(nf))))
                    {
                        sy.doFunc.DynamicInvoke(qq, Parent);
                        break;
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.InlineQueryUpdate:
                    var query = Up.InlineQuery;
                    foreach (SynkCommand sy in InlineQuery_commands)
                    {
                        doInNewThread(() =>
                        {
                            sy.doFunc.DynamicInvoke(query, Parent);
                        });
                    }
                    break;
            }
            foreach (ISynkCommand sn in AllwaysInWebHook_commands)
            {
                List<ArgC> arg = null;
                if (Up.Message != null)
                    arg = args;
                doInNewThread(() =>
                {
                    sn.doFunc.DynamicInvoke(Up, Parent, arg);
                });
            }
            Parent.OnWebHoockUpdated?.Invoke();
        }
        private void SimpleMessageHere(Message ms)
        {
            Parent.MessagesLast.Add(ms);
            LogSystem(ms.From);
            if (cc >= messageCountCheck)
            {
                cc = 0;
                FixSimmilarUsersFromAsynkThread();
            }
        }
        private List<ISynkCommand> AllwaysInWebHook_commands;
        private List<ISynkCommand> InlineQuery_commands;
        private List<ISynkCommand> Query_commands;
        private List<ISynkCommand> Service_commands;
        private List<ISynkCommand> Standart_commands;
        private List<ISynkCommand> Photo_commands;
        public void PreDefineAllwaysInWebHook()
        {
            AllwaysInWebHook_commands = Parent.SynkCommands.Where(fn => fn.Type == TypeOfCommand.AllwaysInWebHook).ToList<ISynkCommand>();
            InlineQuery_commands = Parent.SynkCommands.Where(fn => fn.Type == TypeOfCommand.InlineQuery).ToList<ISynkCommand>();
            Query_commands = Parent.SynkCommands.Where(fn => fn.Type == TypeOfCommand.Query).ToList<ISynkCommand>();
            Service_commands = Parent.SynkCommands.Where(fn => fn.Type == TypeOfCommand.Service).ToList<ISynkCommand>();
            Standart_commands = Parent.SynkCommands.Where(fn => fn.Type == TypeOfCommand.Standart).ToList<ISynkCommand>();
            Photo_commands = Parent.SynkCommands.Where(fn => fn.Type == TypeOfCommand.Photo).ToList<ISynkCommand>();
        }
        private void LogSystem(User us)
        {
            UserM mu = Parent.ActiveUsers.Find(f => UserM.usernameGet(f) == UserM.usernameGet(us));
            if (mu == null)
            {
                Parent.ActiveUsers.Add(new UserM(us, 1));
            } else
            {
                mu.MessageCount++;
            }
        }
        private void FixSimmilarUsersFromAsynkThread()
        {
            for(int i=0; i < Parent.ActiveUsers.Count; i++)
            {
                var k = Parent.ActiveUsers.Where(fs => fs.Id == Parent.ActiveUsers[i].Id).ToList();
                if (k.Count > 1)
                {
                    for (int j = 1; j < k.Count; j++)
                    {
                        k[0].MessageCount++;
                        Parent.ActiveUsers.Remove(k[j]);
                    }
                    Parent.ActiveUsers.Find(fn => UserM.nameGet(fn) == UserM.nameGet(k[0])).MessageCount = k[0].MessageCount;
                }
            }
        }
        private void doInNewThread(Action act)
        {
            new Thread(() => { act.Invoke(); }).Start();
        }
        private string tryToParseNameBotCommand(string s)
        {
            try
            {
                return s.Split('@').First();
            }
            catch { return null; }
        }
        /// <summary>
        /// WARNING: NOT FOR PRIVATE CHAT
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="idchat"></param>
        /// <param name="iduser"></param>
        /// <returns></returns>
        public static async Task<bool> IsAdminAsync(IBot Parent,long idchat, int iduser)
        {
            var admins = await Parent.Client.GetChatAdministratorsAsync(idchat);
            var yuo = admins.Where(fn => fn.User.Id == iduser);
            if (yuo.Count() > 0)
                return true;
            else
                return false;
        }
    }
}
