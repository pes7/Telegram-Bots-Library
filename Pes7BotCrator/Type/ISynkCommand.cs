using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pes7BotCrator.Type
{
    public interface ISynkCommand
    {
        TypeOfCommand Type { get; set; }
        TypeOfAccess TypeOfAccess { get; set; }
        List<string> CommandLine { get; set; }
        Delegate doFunc { get; set; }
        string Description { get; set; }
    }

    public enum TypeOfAccess
    {
        Public,
        Hide,
        Admin,
        Named
    }

    public enum TypeOfCommand {
        Standart,
        Query,
        InlineQuery,
        AllwaysInWebHook,
        Service,
        Photo
    }
}
