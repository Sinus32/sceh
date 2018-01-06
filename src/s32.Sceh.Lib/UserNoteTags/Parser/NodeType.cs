using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.UserNoteTags.Parser
{
    public enum NodeType
    {
        Unknown,
        Root,
        Text,
        TagStart,
        TagEnd,
    }
}
