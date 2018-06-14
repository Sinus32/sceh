using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.BBCode
{
    public class BBTextNode : BBNode
    {
        public override BBNodeType NodeType
        {
            get { return BBNodeType.Text; }
        }

        public override void AddToken(BBToken token)
        {
            if (token.TokenType != BBTokenType.Text)
                throw new InvalidOperationException("Text node cannot have any tokens other than text tokens");
            _tokens.Add(token);
        }
    }
}