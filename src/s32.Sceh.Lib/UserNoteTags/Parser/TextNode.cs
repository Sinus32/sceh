using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.UserNoteTags.Lexer;

namespace s32.Sceh.UserNoteTags.Parser
{
    public class TextNode : Node
    {
        public override NodeType NodeType
        {
            get { return NodeType.Text; }
        }

        public override void AddToken(Token token)
        {
            if (token.TokenType != TokenType.Text)
                throw new InvalidOperationException("Text node cannot have any tokens other than text tokens");
            _tokens.Add(token);
        }
    }
}
