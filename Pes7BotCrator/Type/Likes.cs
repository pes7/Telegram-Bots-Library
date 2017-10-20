using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    public class Likes
    {
        public int MessageId { get; set; }
        public List<long> LikeId { get; set; }
        public List<long> DisLikeId { get; set; }
        public Likes(int messageid)
        {
            MessageId = messageid;
            LikeId = new List<long>();
            DisLikeId = new List<long>();
        }
    }
}
