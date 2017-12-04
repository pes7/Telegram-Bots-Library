using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator;
using Pes7BotCrator.Modules;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;

namespace GuchiBot
{
    class BotLogic
    {
        delegate void Error(Bot parent);
        public async void GetGachiImageLogic(Message ms, IBotBase Parent, List<ArgC> args)
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

        public void Oprosic(Message ms, IBotBase Parent, List<ArgC> args)
        {
            string te = ms.Text.Split('@')?.First();
            if (te != null)
                ms.Text = te;
            Parent.CommandsSynk.Add(new Command(ms.Text, ms.From.Id, ms.Chat.Id));
            Parent.Client.SendTextMessageAsync(ms.Chat.Id, "What yor opros about?");
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

        public async void GachiAttakSynk(Message ms, IBotBase Parent, List<ArgC> args)
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

        public async void GetArgkSynk(Message ms, IBotBase Parent, List<ArgC> args)
        {
            string te = ms.Text.Split('@')?.First();
            if (te != null)
                ms.Text = te;
            Parent.CommandsSynk.Add(new Command(ms.Text, ms.From.Id, ms.Chat.Id));
            await Parent.Client.SendTextMessageAsync(ms.Chat.Id, "Say somethink");
        }

        public void DefaultSynk(Message ms, IBotBase Parent, List<ArgC> args)
        {
            CommandAdd(ms, Parent);
            CommandSynk(Parent);
        }

        private async void CommandAdd(Message ms, IBotBase Parent)
        {
            Command cm = Parent.CommandsSynk.Find(fn => fn.ChatId == ms.Chat.Id && fn.UserId == ms.From.Id);
            if (cm != null)
            {
                cm.AddArgc(ms.Text);
                await Bot.ClearCommandAsync(ms.Chat.Id, ms.MessageId, Parent);
            }
        }
        private void CommandSynk(IBotBase Parent)
        {
            Command[] arg = Parent.CommandsSynk.Where(fn => fn.cArgs.Count > 0).ToArray();
            for (int i = 0; i < arg.Length; i++)
            {
                switch (arg[i].cCommand)
                {
                    case "/testmemory":
                        Parent.Client.SendTextMessageAsync(arg[i].ChatId, $"You said: {arg[i].cArgs[0]}");
                        Parent.CommandsSynk.Remove(arg[i]);
                        break;
                    case "/opros":
                        //Нужно контролировать все исходящие сообщения. Для этого надо добавить колекшен их в Основной БОт
                        var it = Parent.Client.SendTextMessageAsync(arg[i].ChatId, $"{arg[i].cArgs[0]}", replyMarkup: LikeDislikeModule.getKeyBoard($"opros-{(Parent as Bot).Opros.Count}"));
                        Parent.CommandsSynk.Remove(arg[i]);
                        (Parent as Bot).Opros.Add(new Opros(arg[i].cArgs[0], (Parent as Bot).Opros.Count,it.Result));
                        break;
                }
            }
        }
    }
}
