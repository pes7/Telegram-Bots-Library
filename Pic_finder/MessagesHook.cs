using Pes7BotCrator.Type;
using System;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace Pic_finder
{
    public class MessagesHook : Module
    {
        public MessagesHook() : base("Messages Hook", typeof(MessagesHook)) { }
        public Dictionary<int, Message> OrigMsgs = new Dictionary<int, Message>();
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
                this.OrigMsgs.Clear();
            }
            catch (Exception ex)
            {
                serving.Exceptions.Add(ex);
            }
        }

        public async void UpdateReceiver(Update update, IBot serving, List<ArgC> args)
        {
            try
            {
                if (this.hooking && update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
                {
                    Message message = update.Message;
                    if (message.Chat.Id != this.Hooker.Identifier)
                    {
                        Message new_msg= await serving.Client.ForwardMessageAsync(
                            this.Hooker,
                            message.Chat.Id,
                            message.MessageId);
                        this.OrigMsgs.Add(new_msg.MessageId, message);
                    }
                    else if (message.ReplyToMessage != null && message.ReplyToMessage?.ForwardFromMessageId != null)
                    {
                        ChatId chatId = this.OrigMsgs[message.ReplyToMessage.MessageId].Chat.Id;
                        switch (message.Type)
                        {

                            case Telegram.Bot.Types.Enums.MessageType.Audio:
                                await serving.Client.SendAudioAsync(
                                    chatId,
                                    message.Audio.FileId, message.Caption);
                                break;
                            case Telegram.Bot.Types.Enums.MessageType.Contact:
                                await serving.Client.SendContactAsync(
                                    chatId, message.Contact.PhoneNumber, message.Contact.FirstName);
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
            {
                serving.Exceptions.Add(ex);
            }
        }
    }
}