using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.UserNoteTags.Lexer;

namespace s32.Sceh.UserNoteTags.Parser
{
    public class RootNode : Node
    {
        public override NodeType NodeType
        {
            get { return NodeType.Root; }
        }

        public override void AddToken(Token token)
        {
            throw new InvalidOperationException("Root node cannot have any tokens");
        }
    }
}
