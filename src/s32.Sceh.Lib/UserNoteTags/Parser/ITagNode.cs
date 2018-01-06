using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.UserNoteTags.Parser
{
    public interface ITagNode
    {
        bool IsClosed { get; }
        bool IsSelfClosed { get; }
        bool IsValid { get; }
        string TagName { get; }
    }
}