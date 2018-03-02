using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator;
using Pes7BotCrator.Modules;
using Pes7BotCrator.Modules.LikeDislikeModule;
using Pes7BotCrator.Modules.Types;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace GuchiBot
{
    class BotLogic
    {
        delegate void Error(Bot parent);
        public async void GetGachiImageLogic(Message ms, IBot Parent, List<ArgC> args)
        {
            Bot PBot = Parent as Bot;
            Error error = async delegate (Bot parent) { await parent.Client.SendTextMessageAsync(ms.Chat.Id, "Sorry, but my creator dont have Gachi Photoes."); };
            if (PBot.GachiImage != null)
            {
                try
                {
                    GetLocalGachi(ms.Chat.Id, PBot);
                }
                catch (Exception ex)
                {
                    Parent.Exceptions.Add(ex);
                    error(PBot);
                }
            }
            else error(PBot);
        }

        public void ArgMessage(Telegram.Bot.Types.Message ms, IBot bot, List<ArgC> args)
        {
            string message = "";
            if (args != null)
            {
                ArgC ag = args.Find(fs => fs.Name == "id");
                ArgC text = args.Find(fs => fs.Name == "text");
                if (ag != null && text != null)
                {
                    message = $"@{ag.Arg} {text.Arg}";
                }
                bot.Client.SendTextMessageAsync(ms.Chat.Id, message);
            }
        }

        public void AutoDelMessage(Telegram.Bot.Types.Message ms, IBot Parent, List<ArgC> args)
        {
            if (args != null)
            {
                ArgC time = args.Find(fs => fs.Name == "time");
                ArgC text = args.Find(fs => fs.Name == "text");
                if (text != null && time != null)
                    Parent.GetModule<TRM>().SendTimeRelayMessageAsynkAsync(ms.Chat.Id, text.Arg, int.Parse(time.Arg));
            }
        }

        public void InlineMenu(InlineQuery query, IBot Parent)
        {
            if (Parent.Modules.Exists(fn => fn.Name == "_2chModule") && query.Query.Contains("2ch"))
            {
                Webm webm = Parent.GetModule<_2chModule>().WebmsSent.Find(fn => fn.Path == query.Query);
                if (webm != null)
                {
                    var msg = new Telegram.Bot.Types.InputMessageContents.InputTextMessageContent
                    {
                        MessageText = $"<a href=\"{ webm.Thumbnail }\">&#8204;</a>{webm.Path}",
                        ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html,
                    };

                    Telegram.Bot.Types.InlineQueryResults.InlineQueryResult[] results = {
                            new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle{
                                Id = "0",
                                InputMessageContent = msg,
                                ReplyMarkup = LikeDislikeModule.getKeyBoard(),
                                Title = "WEBM",
                                Description = "POST"
                            }
                    };
                    Parent.Client.AnswerInlineQueryAsync(query.Id, results);
                }
            }
            else if (query.Query.Contains("opros"))
            {
                string id = query.Query.Split('-').Last();
                try
                {
                    var Oprs = Parent.GetModule<VoteModule>().Opros.Find(fn => fn.Id == int.Parse(id));
                    if (Oprs != null)
                    {
                        var msg = new Telegram.Bot.Types.InputMessageContents.InputTextMessageContent
                        {
                            MessageText = $"{Oprs.About}",
                            ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html
                        };

                        Pes7BotCrator.Modules.Types.VoteModule.Likes ll = null;
                        try
                        {
                            ll = Parent.GetModule<VoteModule>().LLikes.Find(fn => fn.ParentO.Id == int.Parse(id));
                        }
                        catch { }
                        Telegram.Bot.Types.InlineQueryResults.InlineQueryResult[] results = {
                            new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle{
                                Id = "0",
                                InputMessageContent = msg,
                                ReplyMarkup = VoteModule.getKeyBoard(ll.LikeId.Count, ll.DisLikeId.Count, Oprs,Oprs.Query),
                                Title = "Opros",
                                Description = "POST"
                            }
                    };
                    Parent.Client.AnswerInlineQueryAsync(query.Id, results);
                    }
                }
                catch {}
            }
        }

        private void GetLocalGachi(long chatid, Bot Parent)
        {
            String[] files = Directory.GetFiles(Parent.GachiImage, "*");
            int index = Parent.Rand.Next(0, files.Length);
            Parent.Client.SendPhotoAsync(chatid, new FileToSend(Path.GetFileName(files[index]), System.IO.File.Open(files[index], FileMode.Open)));
        }

        private static bool GachiAttakTrigger = false;
        private void GuchiAttak(long chatid, Bot Parent)
        {
            for (int i = 0; i < 5; i++)
            {
                Parent.Client.SendTextMessageAsync(chatid, i.ToString());
                Thread.Sleep(1000);
            }
            Parent.Client.SendTextMessageAsync(chatid, "Start!!!");
            for (int i = 0; i < 20; i++)
            {
                Parent.Client.SendTextMessageAsync(chatid, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                Thread.Sleep(700);
            }
            Parent.Client.SendTextMessageAsync(chatid, "Attak ended.");
            GachiAttakTrigger = false;
        }

        public async void GachiAttakSynk(Message ms, IBot Parent, List<ArgC> args)
        {
            await Parent.Client.SendTextMessageAsync(ms.Chat.Id,$"Sorry, but it is to strong weapon for u. @{ms.From.Username}");
            return;
            if (!GachiAttakTrigger)
            {
                await Parent.Client.SendTextMessageAsync(ms.Chat.Id, "Prepare your NyanuS.");
                Thread th = new Thread(() =>
                {
                    GuchiAttak(ms.Chat.Id, Parent as Bot);
                });
                th.Start();
                GachiAttakTrigger = true;
            }
        }

        public async void GetArgkSynk(Message ms, IBot Parent, List<ArgC> args)
        {
            string te = ms.Text.Split('@')?.First();
            if (te != null)
                ms.Text = te;
            Parent.ActionCommands.Add(new Command(ms.Text, ms.From.Id, ms.Chat.Id));
            await Parent.Client.SendTextMessageAsync(ms.Chat.Id, "Say somethink");
        }

        public void DefaultSynk(Message ms, IBot Parent, List<ArgC> args)
        {
            CommandAdd(ms, Parent);
            CommandSynk(Parent);
        }

        private async void CommandAdd(Message ms, IBot Parent)
        {
            Command cm = Parent.ActionCommands.Find(fn => fn.ChatId == ms.Chat.Id && fn.UserId == ms.From.Id);
            if (cm != null)
            {
                cm.AddArgc(ms.Text);
                await Bot.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
            }
        }
        private void CommandSynk(IBot Parent)
        {
            Command[] arg = Parent.ActionCommands.Where(fn => fn.cArgs.Count > 0).ToArray();
            for (int i = 0; i < arg.Length; i++)
            {
                switch (arg[i].cCommand)
                {
                    case "/testmemory":
                        Parent.Client.SendTextMessageAsync(arg[i].ChatId, $"You said: {arg[i].cArgs[0]}");
                        Parent.ActionCommands.Remove(arg[i]);
                        break;
                }
            }
        }
    }
}
