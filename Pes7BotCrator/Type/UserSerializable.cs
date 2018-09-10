using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Type
{
    [Serializable]
    public class UserSerializable
    {
        public string PhotoPath { get; set; } = null;
        public int MessageCount { get; set; }
        public List<long> FromChat { get; set; }
        public string Username { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public UserSerializable(UserM us)
        {
            this.FromChat = us.FromChat;
            this.Username = us.Username;
            this.LastName = us.LastName;
            this.Id = us.Id;
            this.FirstName = us.FirstName;
            this.PhotoPath = us.PhotoPath;
            this.MessageCount = us.MessageCount;
        }
        public UserSerializable(User us)
        {
            this.FromChat = new List<long>();
            this.Username = us.Username;
            this.LastName = us.LastName;
            this.Id = us.Id;
            this.FirstName = us.FirstName;
        }
        public UserM GetUserM()
        {
            var user = new User();
            user.Id = Id;
            user.Username = Username;
            user.FirstName = FirstName;
            user.LastName = LastName;
            var m = new UserM(user);
            m.PhotoPath = PhotoPath;
            m.MessageCount = MessageCount;
            m.FromChat = FromChat;
            return m;
        }
        public User GetUser()
        {
            var user = new User();
            user.Id = Id;
            user.Username = Username;
            user.FirstName = FirstName;
            user.LastName = LastName;
            return user;
        }
    }
}
