using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    public class GList<T> : List<T>
    {
        private IBot Parent { get; set; }
        public GList(IBot parent) : base()
        {
            Parent = parent;
        }
        public new void Add(T item)
        {
            base.Add(item);
            //if (Parent.WebHook != null)
                //Parent.WebHook.PreDefineAllwaysInWebHook();
        }
    }
}
