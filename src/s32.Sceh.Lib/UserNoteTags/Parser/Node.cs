using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.UserNoteTags.Lexer;

namespace s32.Sceh.UserNoteTags.Parser
{
    public abstract class Node
    {
        protected List<Node> _content;
        protected List<Token> _tokens;

        public Node()
        {
            _content = new List<Node>();
            _tokens = new List<Token>();
        }

        public List<Node> Content
        {
            get { return _content; }
        }

        public abstract NodeType NodeType { get; }

        public IReadOnlyList<Token> Tokens
        {
            get { return _tokens; }
        }

        public abstract void AddToken(Token token);

        public string GetText()
        {
            var totalLength = 0;
            foreach (var t in _tokens)
                totalLength += t.Content.Length;
            var sb = new StringBuilder(totalLength);
            foreach (var t in _tokens)
                sb.Append(t.Content);
            return sb.ToString();
        }
    }
}