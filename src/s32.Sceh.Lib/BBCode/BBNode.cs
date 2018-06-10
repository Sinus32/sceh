using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.BBCode
{
    public abstract class BBNode : IBBNode
    {
        protected List<BBNode> _content;
        protected List<BBToken> _tokens;

        public BBNode()
        {
            _content = new List<BBNode>();
            _tokens = new List<BBToken>();
        }

        public List<BBNode> Content
        {
            get { return _content; }
        }

        public abstract BBNodeType NodeType { get; }

        public IReadOnlyList<BBToken> Tokens
        {
            get { return _tokens; }
        }

        IReadOnlyList<BBNode> IBBNode.Content
        {
            get { return _content; }
        }

        public abstract void AddToken(BBToken token);

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

        public override string ToString()
        {
            char c;
            switch (NodeType)
            {
                case BBNodeType.Root:
                    c = 'r';
                    break;

                case BBNodeType.Text:
                    c = 't';
                    break;

                case BBNodeType.TagStart:
                    c = 's';
                    break;

                case BBNodeType.TagEnd:
                    c = 'e';
                    break;

                default:
                    c = 'u';
                    break;
            }
            return String.Concat(c, '{', GetText(), '}');
        }
    }
}