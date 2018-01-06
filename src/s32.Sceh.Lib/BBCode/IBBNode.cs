using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.BBCode
{
    public interface IBBNode
    {
        IReadOnlyList<BBNode> Content { get; }

        BBNodeType NodeType { get; }

        string GetText();
    }
}