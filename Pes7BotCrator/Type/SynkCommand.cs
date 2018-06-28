using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot.Types;

namespace Pes7BotCrator.Type
{
    public class SynkCommand : ISynkCommand
    {
        public TypeOfCommand Type { get; set; }
        public TypeOfAccess TypeOfAccess { get; set; }
        public List<string> CommandLine { get; set; }
        public string CommandName { get; set; }
        public Delegate doFunc { get; set; }
        public string Description { get; set; }
        public bool ClearCommand { get; set; } = true;

        /// <summary>
        /// Common Synk Command
        /// </summary>
        /// <param name="act">Action</param>
        /// <param name="cm">Command</param>
        /// <param name="descr">Description</param>
        public SynkCommand(Action<Telegram.Bot.Types.Message, IBot, List<ArgC>> act, List<string> cm = null, TypeOfAccess access = TypeOfAccess.Public, string commandName = null, string descr = null, bool clearcommand = true)
        {
            Description = descr;
            TypeOfAccess = access;
            CommandName = commandName;
            Type = TypeOfCommand.Standart;
            ClearCommand = clearcommand;
            Incialize(act, cm);
        }

        /// <summary>
        /// Query Synk Command for Inline Messages
        /// </summary>
        /// <param name="act">Action</param>
        /// <param name="cm">Command</param>
        /// <param name="descr">Description</param>
        public SynkCommand(Action<InlineQuery, IBot> act, List<string> cm = null, string descr = null)
        {
            Description = descr;
            Type = TypeOfCommand.InlineQuery;
            TypeOfAccess = TypeOfAccess.Public;
            Incialize(act, cm);
        }

        /// <summary>
        /// Query Synk Command for Buttons
        /// </summary>
        /// <param name="act">Action</param>
        /// <param name="cm">Command</param>
        /// <param name="descr">Description</param>
        public SynkCommand(Action<CallbackQuery, IBot> act, List<string> cm = null, string descr = null)
        {
            Description = descr;
            Type = TypeOfCommand.Query;
            TypeOfAccess = TypeOfAccess.Public;
            Incialize(act, cm);
        }

        /// <summary>
        /// Allways in WebHook
        /// </summary>
        /// <param name="act">Action</param>
        /// <param name="cm">Command</param>
        /// <param name="descr">Description</param>
        public SynkCommand(Action<Update, IBot, List<ArgC>> act)
        {
            Type = TypeOfCommand.AllwaysInWebHook;
            TypeOfAccess = TypeOfAccess.Public;
            Description = null;
            Incialize(act,null);
        }

        /// <summary>
        /// Service Synk Command
        /// </summary>
        /// <param name="act">Action</param>
        public SynkCommand(Action<Update, IBot> act, string descr = null)
        {
            Type = TypeOfCommand.Service;
            TypeOfAccess = TypeOfAccess.Public;
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
