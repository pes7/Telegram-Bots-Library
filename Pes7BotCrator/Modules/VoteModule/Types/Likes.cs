using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pes7BotCrator.Modules.Types.VoteModule
{
    [Serializable]
    public class Likes
    {
        public List<string> MessageId { get; set; }
        public long ChatId { get; set; }
        public List<long> LikeId { get; set; }
        public List<long> DisLikeId { get; set; }
        public Opros ParentO { get; set; }
        public Likes(Opros op, long messageid, long chatid)
        {
            ChatId = chatid;
            MessageId = new List<string>();
            MessageId.Add(messageid.ToString());
            LikeId = new List<long>();
            DisLikeId = new List<long>();
            ParentO = op;
        }
        public Likes(Opros op, string chatinstantid, long instid)
        {
            ChatId = instid;
            MessageId = new List<string>();
            MessageId.Add(chatinstantid);
            LikeId = new List<long>();
            DisLikeId = new List<long>();
            ParentO = op;
        }
    }
}
