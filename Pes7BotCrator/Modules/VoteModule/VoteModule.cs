using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pes7BotCrator.Modules.Types;
using Pes7BotCrator.Modules.Types.VoteModule;
using Pes7BotCrator.Type;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Pes7BotCrator.Modules
{
    public class VoteModule : Module
    {
        /*
         * Можно в опросе хранить и мессадж и каллбек кюджери с другой беседы, тогда будет синхронное лайкование. 
         * Нужно постарать как то сделать много виборочный опрос
         */
        public List<Opros> Opros { get; set; }
        public List<Likes> LLikes { get; set; }
        public VoteSynkCommand QueryCommand { get; set; }
        public VoteCreateSynkCommand CreateCommand { get; set; }
        private string FileNameVotes { get; set; }
        private string FileNameLikes { get; set; }
        public VoteModule(string fileNameVotes,string fileNameLikes) : base("VoteModule", typeof(VoteModule))
        {
            Opros = new List<Opros>();
            QueryCommand = new VoteSynkCommand();
            CreateCommand = new VoteCreateSynkCommand();
            LLikes = new List<Likes>();
            FileNameVotes = fileNameVotes;
            FileNameLikes = fileNameLikes;

            if (System.IO.File.Exists(fileNameVotes))
            {
                Opros.AddRange(LoadOpros());
            }
            if (System.IO.File.Exists(fileNameLikes))
            {
                LLikes.AddRange(LoadLikes());
            }
        }

        public class VoteSynkCommand : SynkCommand
        {
            public VoteSynkCommand() : base(Act, new List<string> { "v0", "v1" }) { }
        }

        public class VoteCreateSynkCommand : SynkCommand
        {
            public VoteCreateSynkCommand() : base((Message ms, IBot Parent, List<ArgC> arg) => {
                if (arg != null)
                {
                    ArgC text = arg.Where(fn => fn.Name.Contains("text")).First();
                    ArgC first = null, second = null;
                    if (arg.Count > 2)
                    {
                        first = arg.Where(fn => fn.Name.Contains("answ1")).First();
                        second = arg.Where(fn => fn.Name.Contains("answ2")).First();
                    }  
                    if (text != null) {
                        Opros op;
                        int id = Parent.GetModule<VoteModule>().Opros.Count;
                        if (first != null && second != null)
                        {
                            op = new Opros(text.Arg, id,first.Arg,second.Arg);
                            op.Query = $"opros-{id}";
                            var it = Parent.Client.SendTextMessageAsync(ms.Chat.Id, $"{text.Arg}", replyMarkup: getKeyBoard(op, op.Query));
                            Parent.GetModule<VoteModule>().Opros.Add(op);
                        }
                        else
                        {
                            op = new Opros(text.Arg, id);
                            op.Query = $"opros-{id}";
                            var it = Parent.Client.SendTextMessageAsync(ms.Chat.Id, $"{text.Arg}", replyMarkup: getKeyBoard(op, op.Query));
                            Parent.GetModule<VoteModule>().Opros.Add(op);
                        }
                    }
                }
            }, new List<string> { "/vcreate" }, "You can create ur own Vote; Args: `text` and optional `answ1` `answ2`") { }
        }

        /*как вариант 
         new [] {
            new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"{opr.First} 0",$"id:{opr.Id}:type:v1")
        },
        new []{
            new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"{opr.Second} 0",$"id:{opr.Id}:type:v0")
        }
        */
        public static InlineKeyboardMarkup getKeyBoard(Opros opr,string query = null)
        {
            InlineKeyboardMarkup keyboard = null;
            if (query == null)
            {
                keyboard = new InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"{opr.First} 0",$"id:{opr.Id}:type:v1"),
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"{opr.Second} 0",$"id:{opr.Id}:type:v0")
                    }
                });
            }
            else
            {
                keyboard = new InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"{opr.First} 0",$"id:{opr.Id}:type:v1"),
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"{opr.Second} 0",$"id:{opr.Id}:type:v0")
                    },
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardSwitchInlineQueryButton("Share",query)
                    }
                });
            }
            return keyboard;
        }

        public static InlineKeyboardMarkup getKeyBoard(int l, int d, Opros opr, string query = null)
        {
            InlineKeyboardMarkup keyboard = null;
            if (query == null)
            {
                keyboard = new InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"{opr.First} {l}",$"id:{opr.Id}:type:v1"),
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"{opr.Second} {d}",$"id:{opr.Id}:type:v0")
                    }
                });
            }
            else
            {
                keyboard = new InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][] {
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"{opr.First} {l}",$"id:{opr.Id}:type:v1"),
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton($"{opr.Second} {d}",$"id:{opr.Id}:type:v0")
                    },
                    new [] {
                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardSwitchInlineQueryButton("Share",query)
                    }
                });
            }
            return keyboard;
        }

        public static void Act(CallbackQuery re, IBot Parent)
        {
            Message ms = re?.Message;
            long id;
            if (re.Message != null)
            {
                id = re.Message.MessageId;
            }
            else
                id = long.Parse(re.Id);

            VoteModule LDModule = Parent.GetModule<VoteModule>();
            string[] Dd = re.Data.Split(':');
            Opros thisOpr = null;
            try
            {
                thisOpr = Parent.GetModule<VoteModule>().Opros.Find(fn => fn.Id == int.Parse(Dd[1]));//
            }
            catch { Parent.Exceptions.Add(new Exception($"Opros[{int.Parse(Dd[1])}] not faund.")); return; }
            if (thisOpr == null)
            {
                Parent.Exceptions.Add(new Exception($"Opros[{int.Parse(Dd[1])}] not faund."));
                return;
            }

            Likes ll;
            try
            {
                if (ms != null)
                    ll = LDModule.LLikes.Find(ff => ff.ParentO.Id == thisOpr.Id);
                else
                    ll = LDModule.LLikes.Find(ff => ff.ParentO.Id == thisOpr.Id);
            }
            catch { Parent.Exceptions.Add(new Exception($"Likes for Opros not faund")); ll = null; }

            if (ll == null)
            {
                if (ms != null)
                    LDModule.LLikes.Add(new Likes(thisOpr, ms.MessageId, ms.Chat.Id));
                else
                    LDModule.LLikes.Add(new Likes(thisOpr, re.InlineMessageId, id));
                ll = LDModule.LLikes.Last();
            }

            if (Dd[3] == "v1")
            {
                long li = ll.LikeId.Find(dd => dd == re.From.Id);
                long ld = ll.DisLikeId.Find(dd => dd == re.From.Id);
                if (li == 0 && ld == 0)
                    ll.LikeId.Add(re.From.Id);
                else if (li == 0 && ld != 0)
                {
                    ll.LikeId.Add(ld);
                    ll.DisLikeId.Remove(ld);
                }
                else
                    ll.LikeId.Remove(li);
            }
            else if (Dd[3] == "v0")
            {
                long li = ll.LikeId.Find(dd => dd == re.From.Id);
                long ld = ll.DisLikeId.Find(dd => dd == re.From.Id);
                if (li == 0 && ld == 0)
                    ll.DisLikeId.Add(re.From.Id);
                else if (li != 0 && ld == 0)
                {
                    ll.DisLikeId.Add(li);
                    ll.LikeId.Remove(li);
                }
                else
                    ll.DisLikeId.Remove(ld);
            }

            InlineKeyboardMarkup keyboard = null;
            if (thisOpr.Query != null)
                keyboard = getKeyBoard(ll.LikeId.Count, ll.DisLikeId.Count, thisOpr, thisOpr.Query);
            else
                keyboard = getKeyBoard(ll.LikeId.Count, ll.DisLikeId.Count, thisOpr);
            Thread th = new Thread(async () =>
            {
                try
                {
                    if(re.Message != null)
                        await Parent.Client.EditMessageReplyMarkupAsync(ms.Chat.Id, ms.MessageId, keyboard);
                    else
                        await Parent.Client.EditInlineMessageReplyMarkupAsync(re.InlineMessageId, keyboard);
                }
                catch { };
            });
            th.Start();
        }

        public void Save()
        {
            if (LLikes.Count > 0)
            {
                List<Likes> ls = LLikes;
                SaveLoadModule.SaveSomething(ls, FileNameLikes);
            }
            if (Opros.Count > 0)
            {
                List<Opros> op = Opros;
                SaveLoadModule.SaveSomething(op, FileNameVotes);
            }
        }

        private List<Likes> LoadLikes()
        {
            return SaveLoadModule.LoadSomething<List<Likes>>(FileNameLikes);
        }

        private List<Opros> LoadOpros()
        {
            return SaveLoadModule.LoadSomething<List<Opros>>(FileNameVotes);
        }
    }
}
