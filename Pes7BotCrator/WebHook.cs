using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Commands;
using Pes7BotCrator.Modules;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;
using System.Diagnostics;
using System.Collections;

namespace Pes7BotCrator
{
    public class WebHook
    {
        private IBot Parent { get; set; }
        public WebHook(IBot parent)
        {
            Parent = parent;
            Parent.Client.SetWebhookAsync("");
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
                        new Thread(() => { MessageSynk(up); }).Start();
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
            switch (Up.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.MessageUpdate:
                    ms = Up.Message;
                    switch (Up.Message.Type) {
                        case Telegram.Bot.Types.Enums.MessageType.PhotoMessage:
                            Parent.MessagesLast.Add(ms);
                            LogSystem(ms.From);
                            if (cc >= messageCountCheck)
                            {
                                cc = 0;
                                FixSimmilarUsersFromAsynkThread();
                            }
                            foreach (SynkCommand sy in Parent.SynkCommands.Where(
                                fn => fn.Type == TypeOfCommand.Photo &&
                                fn.CommandLine.Exists(sn => sn == ((ArgC.getArgs(ms.Text) == null) ? ms.Text : ArgC.getArgs(ms.Text).First().Name.Trim()) ||
                                                           (sn == tryToParseNameBotCommand(ms.Text) && tryToParseNameBotCommand(ms.Text) != null))
                                ))
                            {
                                await BotBase.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
                                Thread the = new Thread(() =>
                                {
                                    try
                                    {
                                        sy.doFunc.DynamicInvoke(ms, Parent, ArgC.getArgs(ms.Text));
                                    }
                                    catch (Exception ex) { Parent.Exceptions.Add(ex); }
                                });
                                the.Start();
                                break;
                            }
                            break;
                        case Telegram.Bot.Types.Enums.MessageType.TextMessage:
                            Parent.MessagesLast.Add(ms);
                            LogSystem(ms.From);
                            if (cc >= messageCountCheck)
                            {
                                cc = 0;
                                FixSimmilarUsersFromAsynkThread();
                            }
                            foreach (SynkCommand sy in Parent.SynkCommands.Where(
                                fn => fn.Type == TypeOfCommand.Standart &&
                                fn.CommandLine.Exists(sn => sn == ((ArgC.getArgs(ms.Text) == null) ? ms.Text : ArgC.getArgs(ms.Text).First().Name.Trim()) ||
                                                           (sn == tryToParseNameBotCommand(ms.Text) && tryToParseNameBotCommand(ms.Text) != null))
                                ))
                            {
                                await BotBase.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
                                Thread the = new Thread(() =>
                                {
                                    try
                                    {
                                        sy.doFunc.DynamicInvoke(ms, Parent, ArgC.getArgs(ms.Text));
                                    } catch (Exception ex) { Parent.Exceptions.Add(ex); }
                                });
                                the.Start();
                                break;
                            }
                            try
                            {
                                Parent.SynkCommands.Find(fn => fn.CommandLine.Exists(nf => nf == "Default"))?.doFunc.DynamicInvoke(ms, Parent, ArgC.getArgs(ms.Text));
                            }catch{ }
                            cc++;
                            break;
                        case Telegram.Bot.Types.Enums.MessageType.StickerMessage:

                            break;
                        case Telegram.Bot.Types.Enums.MessageType.ServiceMessage:
                            foreach(SynkCommand sy in Parent.SynkCommands.Where(fn=>fn.Type == TypeOfCommand.Service))
                            {
                                Thread the = new Thread(() =>
                                {
                                    try
                                    {
                                        sy.doFunc.DynamicInvoke(Up, Parent);
                                    } catch (Exception ex) { Parent.Exceptions.Add(ex); }
                                });
                                the.Start();
                            }
                            break;
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQueryUpdate:
                    CallbackQuery qq = Up.CallbackQuery;
                    foreach (SynkCommand sy in Parent.SynkCommands.Where(
                        fn => fn.Type == TypeOfCommand.Query
                        && fn.CommandLine.Exists(nf => Up.CallbackQuery.Data.Contains(nf))))
                    {
                        Thread the = new Thread(() =>
                        {
                            try
                            {
                                sy.doFunc.DynamicInvoke(qq, Parent);
                            }
                            catch(Exception ex) { Parent.Exceptions.Add(ex); }
                        });
                        the.Start();
                        break;
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.InlineQueryUpdate:
                    var query = Up.InlineQuery;
                    foreach (SynkCommand sy in Parent.SynkCommands.Where(fn => fn.Type == TypeOfCommand.InlineQuery))
                    {
                        try
                        {
                            sy.doFunc.DynamicInvoke(query, Parent);
                        } catch (Exception ex) { Parent.Exceptions.Add(ex); }
                    }
                    break;
            }
            foreach (ISynkCommand sn in Parent.SynkCommands.Where(fn => fn.Type == TypeOfCommand.AllwaysInWebHook))
            {
                List<ArgC> arg = null;
                if(Up.Message != null)
                    arg = ArgC.getArgs(Up.Message.Text);
                sn.doFunc.DynamicInvoke(Up,Parent, arg);
            }
            Parent.OnWebHoockUpdated();
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
        private string tryToParseNameBotCommand(string s)
        {
            try
            {
                return s.Split('@').First();
            }
            catch { return null; }
        }
    }
}
