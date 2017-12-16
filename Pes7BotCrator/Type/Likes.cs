using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    [Serializable]
    public class Likes
    {
        public string MessageId { get; set; }
        public long ChatId { get; set; }
        public List<long> LikeId { get; set; }
        public List<long> DisLikeId { get; set; }
        public enum TypeOfLike { Common, Instant };
        public TypeOfLike Type { get; set; }
        public Likes(long messageid, long chatid)
        {
            ChatId = chatid;
            MessageId = messageid.ToString();
            Type = TypeOfLike.Common;
            LikeId = new List<long>();
            DisLikeId = new List<long>();
        }
        public Likes(string chatinstantid, long instid)
        {
            ChatId = instid;
            MessageId = chatinstantid;
            Type = TypeOfLike.Instant;
            LikeId = new List<long>();
            DisLikeId = new List<long>();
        }
    }
}
