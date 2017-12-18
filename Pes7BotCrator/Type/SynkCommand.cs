﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Type
{
    public class SynkCommand : ISynkCommand
    {
        public TypeOfCommand Type { get; set; }
        public List<string> CommandLine { get; set; }
        public Delegate doFunc { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Common Synk Command
        /// </summary>
        /// <param name="act">Action</param>
        /// <param name="cm">Command</param>
        /// <param name="descr">Description</param>
        public SynkCommand(Action<Message, IBotBase, List<ArgC>> act, List<string> cm = null, string descr = null)
        {
            Description = descr;
            Type = TypeOfCommand.Standart;
            Incialize(act, cm);
        }

        /// <summary>
        /// Query Synk Command for Inline Messages
        /// </summary>
        /// <param name="act">Action</param>
        /// <param name="cm">Command</param>
        /// <param name="descr">Description</param>
        public SynkCommand(Action<InlineQuery, IBotBase> act, List<string> cm = null, string descr = null)
        {
            Description = descr;
            Type = TypeOfCommand.InlineQuery;
            Incialize(act, cm);
        }

        /// <summary>
        /// Query Synk Command for Buttons
        /// </summary>
        /// <param name="act">Action</param>
        /// <param name="cm">Command</param>
        /// <param name="descr">Description</param>
        public SynkCommand(Action<CallbackQuery, IBotBase> act, List<string> cm = null, string descr = null)
        {
            Description = descr;
            Type = TypeOfCommand.Query;
            Incialize(act, cm);
        }

        /// <summary>
        /// Allways in WebHook
        /// </summary>
        /// <param name="act">Action</param>
        /// <param name="cm">Command</param>
        /// <param name="descr">Description</param>
        public SynkCommand(Action<Update, IBotBase, List<ArgC>> act)
        {
            Type = TypeOfCommand.AllwaysInWebHook;
            Description = null;
            Incialize(act,null);
        }

        /// <summary>
        /// Service Synk Command
        /// </summary>
        /// <param name="act"></param>
        public SynkCommand(Action<Update, IBotBase> act, string descr = null)
        {
            Type = TypeOfCommand.Service;
            Description = descr;
            Incialize(act, null);
        }

        private void Incialize(dynamic ds, List<string> cm = null)
        {
            if (cm == null)
                CommandLine = new List<string>();
            else CommandLine = cm;
            doFunc = ds;
        }
    }
}
