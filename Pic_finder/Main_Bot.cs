using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator;
using Pes7BotCrator.Commands;
using Pes7BotCrator.Modules;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Concurrent;

namespace Pic_finder
{
    public class BotGetsWrongException : Exception //Виключення для ьота, коли він входить в незадовільну ситуацію.
    {
        public BotGetsWrongException() : base() { }

        public BotGetsWrongException(string message) : base(message) { }

        public BotGetsWrongException(string message, Exception inner) : base(message, inner) { }
    }

    public class Main_Bot : BotBase //Основний 
    {
        public SqlConnection DBconn = null; //Підключення до БД.
        private List<SqlCommand> GetCommands = new List<SqlCommand>() //Команди для почергового зчитування таблиць.
        {
            new SqlCommand("Select * from Chat"),
            new SqlCommand("Select * from Users"),
            new SqlCommand("Select * from TMessages"),
            new SqlCommand("Select * from Files")
        };
        private Thread MsgSilentWriter = null; //Потік для "тихого" запису в БД.
        private ConcurrentQueue<List<Message>> ToWrite = null; //Черга на запис.
        public Main_Bot(
            System.String api_key,
            System.String name,
            System.String shortName,
            System.String creatorName,
            SqlConnection dbconn,
            List<IModule> mods = null
            ) : base(
                api_key,
                name,
                shortName,
                modules: mods,
                usernameofcreator: creatorName)
        {
            this.DBconn = dbconn;
            if (this.DBconn != null)
            {
                if (this.DBconn.State == ConnectionState.Open)
                {
                    this.ToWrite = new ConcurrentQueue<List<Message>>();
                    this.MsgSilentWriter = new Thread(() =>
                    {
                        while (true)
                        {
                            Thread.Sleep(new TimeSpan(0, 5, 0)); //Пауза на хвилину задля економії енергії джерела.
                            this.AbsorbToDb();
                        }
                    });
                    this.GetCommands.ForEach(delegate (SqlCommand command) //Надання підключення до БД, для команд зчитування.
                    {
                        command.Connection = this.DBconn;
                    });
                    this.MsgSilentWriter.Name = "AniPic silent message2db writer";
                    this.MsgSilentWriter.Priority = ThreadPriority.Highest;
                    this.MsgSilentWriter.IsBackground = true;
                    this.MsgSilentWriter.Start(); //Запуск "тихого" записувача.
                    this.OnWebHoockUpdated += this.AddedMsg;
                    this.Client.OnMessage += delegate (object sender, Telegram.Bot.Args.MessageEventArgs messageEvent) //Непрацюючий запис повідомлень від бота.
                    {
                        this.ToWrite.Enqueue(new List<Message>() { messageEvent.Message });
                    };
                }
            }
        }

        private void AddToDB(List<Message> messages) //Запис повідомлень, та їх супутніх данинх, в БД.
        {
            if (this.DBconn.State == ConnectionState.Open && messages != null)
            {
                Dictionary<Message, ulong> msgs_db_ids = new Dictionary<Message, ulong>();
                for (UInt16 i = 0; i < this.GetCommands.Count; i++) //По-табличний запис.
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(this.GetCommands[i]); //"Підключення" поточної таблиці.
                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter); //Створення команд для змінни даних
                    commandBuilder.GetDeleteCommand();
                    commandBuilder.GetInsertCommand();
                    commandBuilder.GetUpdateCommand();
                    DataSet dataSet = new DataSet();
                    dataAdapter.Fill(dataSet); //Заповнення DataSet, для полегшення запису даних.
                    SqlTransaction transaction = this.DBconn.BeginTransaction(); //Запуск транзакції для запису даних, та присвоєння її до команд.
                    commandBuilder.GetDeleteCommand().Transaction = transaction;
                    commandBuilder.GetInsertCommand().Transaction = transaction;
                    commandBuilder.GetUpdateCommand().Transaction = transaction;
                    dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();
                    dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
                    dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                    List<Chat> d_chats = new List<Chat>(); //Списки для уникнення дубльованних операцій.
                    List<User> d_users = new List<User>();
                    foreach (Message msg in messages)
                    {

                        if (msg is null) continue;
                        switch (i)
                        {
                            case 0:
                                if (!d_chats.Contains(msg.Chat)) d_chats.Add(msg.Chat);
                                break;
                            case 1:
                                if (!d_users.Contains(msg.From)) d_users.Add(msg.From);
                                break;
                            case 2:
                                DataRow[] msgs_indb = dataSet.Tables[0].Select("MsgId=" + Convert.ToString(msg.MessageId) + "AND ChatId=" + Convert.ToString(msg.Chat.Id));
                                DataRow msg_indb = msgs_indb.Count() > 0 ? msgs_indb[0] : null;
                                System.String sMsg = JsonConvert.SerializeObject(msg);
                                if (msg_indb != null)
                                {
                                    if (msg_indb["Serialized"] != sMsg)
                                    {
                                        msg_indb["Caption"] = msg.Caption;
                                        msg_indb["Document"] = msg.Document?.FileId;
                                        msg_indb["FromUser"] = msg.From.Id;
                                        msg_indb["DateSent"] = msg.Date;
                                        if (msg.ForwardFrom?.Id == null) msg_indb["ForwardFrom"] = DBNull.Value;
                                        else msg_indb["ForwardFrom"] = msg.ForwardFrom.Id;
                                        if (msg.ForwardFromChat?.Id == null) msg_indb["ForwardFrom"] = DBNull.Value;
                                        else msg_indb["ForwardFromChat"] = msg.ForwardFromChat.Id;
                                        msg_indb["ForwardSignature"] = msg.ForwardSignature;
                                        msg_indb["MsgText"] = msg.Text;
                                        if (msg.ReplyToMessage != null) msg_indb["ReplyToMessage"] = msg.ReplyToMessage.MessageId;
                                        else msg_indb["ReplyToMessage"] = DBNull.Value;
                                        msg_indb["MessageType"] = msg.Type;
                                        msg_indb["Serialized"] = sMsg;
                                    }
                                }
                                else
                                {
                                    DataRow nMsg = dataSet.Tables[0].NewRow();
                                    nMsg["MsgId"] = msg.MessageId;
                                    nMsg["ChatId"] = msg.Chat.Id;
                                    nMsg["Caption"] = msg.Caption;
                                    nMsg["Document"] = msg.Document?.FileId;
                                    nMsg["FromUser"] = msg.From.Id;
                                    nMsg["DateSent"] = msg.Date;
                                    if (msg.ForwardFrom?.Id == null) nMsg["ForwardFrom"] = DBNull.Value;
                                    else nMsg["ForwardFrom"] = msg.ForwardFrom.Id;
                                    if (msg.ForwardFromChat?.Id == null) nMsg["ForwardFrom"] = DBNull.Value;
                                    else nMsg["ForwardFromChat"] = msg.ForwardFromChat.Id;
                                    nMsg["ForwardSignature"] = msg.ForwardSignature;
                                    nMsg["MsgText"] = msg.Text;
                                    if (msg.ReplyToMessage != null) nMsg["ReplyToMessage"] = msg.ReplyToMessage.MessageId;
                                    else nMsg["ReplyToMessage"] = DBNull.Value;
                                    nMsg["MessageType"] = msg.Type;
                                    nMsg["Serialized"] = sMsg;
                                    dataSet.Tables[0].Rows.Add(nMsg);
                                    ulong msgid = 0;
                                    UInt64.TryParse(nMsg["Id"].ToString(), out msgid);
                                    msgs_db_ids.Add(msg, msgid);
                                }
                                break;
                            case 4:
                                List<FileBase> files = new List<FileBase>();
                                if (msg.Audio != null) files.Add(msg.Audio);
                                if (msg.Document != null) files.Add(msg.Document);
                                if (msg.Photo != null) files.AddRange(msg.Photo);
                                if (msg.Video != null) files.Add(msg.Video);
                                if (msg.VideoNote != null) files.Add(msg.VideoNote);
                                files.ForEach(delegate (FileBase fb)
                                {
                                    if (fb == null) return;
                                    if (dataSet.Tables[0].Select("FileId=\'" + fb.FileId + "\'").Count() == 0)
                                    {
                                        DataRow nFile = dataSet.Tables[0].NewRow();
                                        nFile["MessageId"] = msgs_db_ids[msg];
                                        nFile["FileId"] = fb.FileId;
                                        nFile["FileSize"] = fb.FileSize;
                                        try
                                        {
                                            nFile["MimeType"] = ((dynamic)fb)?.MimeType;
                                        }
                                        catch (RuntimeBinderException)
                                        {
                                            nFile["MimeType"] = DBNull.Value;
                                        }
                                        dataSet.Tables[0].Rows.Add(nFile);
                                    }
                                });
                                break;
                        }
                    }
                    if (i == 0) foreach (Chat chat in d_chats)
                        {
                            DataRow[] chats = dataSet.Tables[0].Select("Id=" + Convert.ToString(chat.Id));
                            DataRow chat_tb = chats.Count() > 0 ? chats[0] : null; //Чи існує запис в БД.
                            System.String sChat = JsonConvert.SerializeObject(chat);
                            if (chat_tb != null)
                            {
                                if (chat_tb["Serialized"].ToString() != sChat) //У випадку невідповідності об'єктів з повідомлення та БД, здійснюється оновлення його запису в БД. І так далі, в інших подібних конструкціях.
                                {
                                    chat_tb["ChatType"] = chat.Type;
                                    chat_tb["Title"] = chat.Title;
                                    chat_tb["Username"] = chat.Username;
                                    chat_tb["ChatDescription"] = chat.Description;
                                    chat_tb["InviteLink"] = chat.InviteLink;
                                    chat_tb["Serialized"] = sChat;
                                }
                            }
                            else //У випадку відсутності об'єкта в БД, здійснююється його запис в БД.
                            {
                                DataRow nChat = dataSet.Tables[0].NewRow();
                                nChat["Id"] = chat.Id;
                                nChat["ChatType"] = chat.Type;
                                nChat["Title"] = chat.Title;
                                nChat["Username"] = chat.Username;
                                nChat["ChatDescription"] = chat.Description;
                                nChat["InviteLink"] = chat.InviteLink;
                                nChat["Serialized"] = sChat;
                                dataSet.Tables[0].Rows.Add(nChat);
                                /*
                                try
                                { this.msgsData.Tables[0].ImportRow(nChat); }
                                catch { }*/
                            }
                        }
                    if (i == 1) foreach (User user in d_users) //Запис користувачів.
                        {

                            DataRow[] users = dataSet.Tables[0].Select("Id=" + Convert.ToString(user.Id));
                            DataRow user_tb = users.Count() > 0 ? users[0] : null;
                            System.String sUser = JsonConvert.SerializeObject(user);
                            if (user_tb != null)
                            {
                                if (user_tb["Serialize"] != sUser)
                                {
                                    user_tb["IsBot"] = user.IsBot;
                                    user_tb["FirstName"] = user.FirstName;
                                    user_tb["LastName"] = user.LastName;
                                    user_tb["Username"] = user.Username;
                                    user_tb["LanguageCode"] = user.LanguageCode;
                                    user_tb["Serialize"] = sUser;
                                }
                            }
                            else
                            {
                                DataRow nUser = dataSet.Tables[0].NewRow();
                                nUser["Id"] = user.Id;
                                nUser["IsBot"] = user.IsBot;
                                nUser["FirstName"] = user.FirstName;
                                nUser["LastName"] = user.LastName;
                                nUser["Username"] = user.Username;
                                nUser["LanguageCode"] = user.LanguageCode;
                                nUser["Serialize"] = sUser;
                                dataSet.Tables[0].Rows.Add(nUser);
                            }
                        }
                    try
                    {
                        dataAdapter.Update(dataSet); //Оновлення поточної таблиці в БД.
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        this.Exceptions.Add(ex); //Запис виключення в лог.
                        transaction.Rollback(); //Скасування транзакції.
                    }
                }
            }
        }

        public void AddedMsg() //Подія на нові повідомленя.
        {
            this.ToWrite.Enqueue(this.MessagesLast.ToList());
        }

        public void AbsorbToDb() //Метод оновлення БД.
        {
            if (this.DBconn == null) return;
            List<Message> to_write = new List<Message>();
            while (this.ToWrite.Count > 0) //Збір списків з черги.
            {
                List<Message> messages;
                if (this.ToWrite.TryDequeue(out messages)) to_write.AddRange(messages);
            }
            this.AddToDB(to_write); //Запис списку в БД.
        }

        ~Main_Bot()
        {
            if (this.DBconn?.State == ConnectionState.Open)
            {
                this.AbsorbToDb(); //Критичний запис даних перед закриттям програми.
                this.DBconn.Close();
            }
            this.DBconn?.Dispose();
        }
    }
}
