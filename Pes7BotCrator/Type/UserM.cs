﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Type
{
    public class UserM : User
    {
        public int MessageCount { get; set; }
        public UserM(User us, int i = 0) : base()
        {
            this.Username = us.Username;
            this.LastName = us.LastName;
            this.Id = us.Id;
            this.FirstName = us.FirstName;
            MessageCount = i;
        }
        public static string nameGet(User us)
        {
            return $"{us.FirstName} {us.LastName}";
        }
        public static string usernameGet(User us)
        {
            return us.Username != null ? us.Username : $"{us.FirstName} {us.LastName}";
        }
    }
}