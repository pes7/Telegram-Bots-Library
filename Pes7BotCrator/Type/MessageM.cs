using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

[Serializable]
public class MessageM 
{
    public string Message { get; set; }
    public int Id { get; set; }
    public long ChatId { get; set; }
    public From From { get; set; }
    public MessageM(Message ms)
    {
        Message = ms.Text;
        Id = ms.MessageId;
        ChatId = ms.Chat.Id;
        From = new From(ms.From);
    }
}

[Serializable]
public class From
{
    int id { get; set; }
    string username { get; set; }
    string first { get; set; }
    string last { get; set; }
    public From(User ms)
    {
        id = ms.Id;
        username = ms.Username;
        first = ms.FirstName;
        last = ms.LastName;
    }
    public string getName()
    {
        return username != null ? username : $"{first} {last}";
    }
}