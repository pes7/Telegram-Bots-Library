using Pes7BotCrator.Type;
using System;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Pic_finder
{
    public class MessagesHook : Module
    {
        public MessagesHook() : base("Messages Hook", typeof(MessagesHook)) { }
        public bool Hooking { get { return this.hooking; } }
        private bool hooking;
        private ChatId Hooker = null;

        public async void StartHooking(Message msg, IBot serving, List<ArgC> args)
        {
            try
            {
                this.Hooker = msg.Chat.Id;
                this.hooking = true;
            }
            catch (Exception ex)
            {
                serving.Exceptions.Add(ex);
            }
        }

        public async void StopHooking(Message msg, IBot serving, List<ArgC> args)
        {
            try
            {
                this.hooking = false;
                this.Hooker = null;
            }
            catch (Exception ex)
            {
                serving.Exceptions.Add(ex);
            }
        }

        public async void WatchPrevUpdate(Message message, IBot bot, List<ArgC> args)
        {
            args = bot.GetModule<micro_logic>().NormalizeArgs(message, bot, args);
            this.Hooker = message.Chat.Id;
            int offset = 0, limit = 1;
            try
            { offset = Convert.ToInt32(ArgC.GetArg(args, "offset").Arg); }
            catch (NullReferenceException) { }
            catch (Exception ex) { bot.Exceptions.Add(ex); }
            try
            { limit = Convert.ToInt32(ArgC.GetArg(args, "limit").Arg); }
            catch (NullReferenceException) { }
            catch (Exception ex) { bot.Exceptions.Add(ex); }
            Update[] updates = await bot.Client.GetUpdatesAsync(offset: offset, limit: limit);
            foreach (Update update in updates)
            {
                switch(update.Type)
                {
                    case Telegram.Bot.Types.Enums.UpdateType.Message:
                        this.SpForw(update.Message, bot);
                        break;
                }
            }
        }

        private async void SpForw(Message msg, IBot serving)
        {
            try
            {
                if (msg.Chat.Id == this.Hooker.Identifier) return;
                await serving.Client.ForwardMessageAsync(this.Hooker, msg.Chat.Id, msg.MessageId);
                await serving.Client.SendTextMessageAsync(this.Hooker, "update_instance ChatId=" + msg.Chat.Id.ToString() + " MsgId=" + msg.MessageId.ToString() + " UserId=" + msg.From.Id.ToString());
            }
            catch (Exception ex)
            { serving.Exceptions.Add(ex); }
        }

        public async void UpdateReceiver(Update update, IBot serving, List<ArgC> args)
        {
            try
            {
                if (this.hooking && update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
                {
                    Message message = update.Message;
                    if (message.Chat.Id != this.Hooker.Identifier) this.SpForw(message, serving);
                    else if (message.ReplyToMessage != null)
                    {
                        System.String mn_txt = message.ReplyToMessage.Text ?? System.String.Empty;
                        mn_txt += message.ReplyToMessage.Caption ?? System.String.Empty;
                        if (!mn_txt.Contains("update_instance")) return;
                        List<ArgC> n_args = new List<ArgC>();
                        foreach (System.String a in mn_txt.Split(' '))
                        {
                            System.String[] a_s = a.Split('=');
                            n_args.Add(new ArgC(a_s[0], a_s.Length == 2 ? a_s[1] : null));
                        }
                        ChatId chatId = Convert.ToInt64(ArgC.GetArg(n_args, "ChatId").Arg);
                        switch (message.Type)
                        {

                            case Telegram.Bot.Types.Enums.MessageType.Audio:
                                await serving.Client.SendAudioAsync(chatId, message.Audio.FileId, message.Caption);
                                break;
                            case Telegram.Bot.Types.Enums.MessageType.Contact:
                                await serving.Client.SendContactAsync(chatId, message.Contact.PhoneNumber, message.Contact.FirstName);
                                break;
                            case Telegram.Bot.Types.Enums.MessageType.Document:
                                await serving.Client.SendDocumentAsync(chatId, message.Document.FileId);
                                break;
                            case Telegram.Bot.Types.Enums.MessageType.Game:
                                await serving.Client.SendGameAsync(chatId.Identifier, message.Game.Title);
                                break;
                            case Telegram.Bot.Types.Enums.MessageType.Location:
                                await serving.Client.SendLocationAsync(chatId.Identifier, message.Location.Latitude, message.Location.Longitude);
                                break;
                            case Telegram.Bot.Types.Enums.MessageType.Photo:
                                await serving.Client.SendPhotoAsync(chatId.Identifier, message.Photo[1].FileId, message.Caption);
                                break;
                            case Telegram.Bot.Types.Enums.MessageType.Sticker:
                                await serving.Client.SendStickerAsync(chatId.Identifier, message.Sticker.FileId);
                                break;
                            case Telegram.Bot.Types.Enums.MessageType.Text:
                                await serving.Client.SendTextMessageAsync(chatId.Identifier, message.Text);
                                break;
                            case Telegram.Bot.Types.Enums.MessageType.Video:
                                await serving.Client.SendVideoAsync(chatId.Identifier, message.Video.FileId, caption: message.Caption);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            { serving.Exceptions.Add(ex); }
        }
    }
}