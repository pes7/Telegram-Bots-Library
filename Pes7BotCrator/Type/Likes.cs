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
        public int MessageId { get; set; }
        public int InstantId { get; set; }
        public int ChatInstanceId { get; set; }
        public long ChatId { get; set; }
        public List<long> LikeId { get; set; }
        public List<long> DisLikeId { get; set; }
        public enum TypeOfLike { Common, Instant };
        public TypeOfLike Type { get; set; }
        public Likes(int messageid, long chatid)
        {
            ChatId = chatid;
            MessageId = messageid;
            Type = TypeOfLike.Common;
            LikeId = new List<long>();
            DisLikeId = new List<long>();
        }
        public Likes(int instid, int chatinstantid)
        {
            InstantId = instid;
            ChatInstanceId = chatinstantid;
            Type = TypeOfLike.Instant;
            LikeId = new List<long>();
            DisLikeId = new List<long>();
        }
    }
}
