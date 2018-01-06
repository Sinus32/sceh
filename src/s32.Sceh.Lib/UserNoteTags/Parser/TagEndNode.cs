using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.UserNoteTags.Lexer;

namespace s32.Sceh.UserNoteTags.Parser
{
    public class TagEndNode : Node, ITagNode
    {
        private bool _isCompleted, _isInvalid;
        private string _tagName;

        public bool IsClosed
        {
            get { return OpenTag != null; }
        }

        public bool IsSelfClosed
        {
            get { return OpenTag == this; }
        }

        public bool IsValid
        {
            get { return _isCompleted && !_isInvalid && !String.IsNullOrEmpty(_tagName); }
        }

        public override NodeType NodeType
        {
            get { return NodeType.TagEnd; }
        }

        public ITagNode OpenTag { get; set; }

        public string TagName
        {
            get { return _tagName; }
        }

        public override void AddToken(Token token)
        {
            switch (token.TokenType)
            {
                case TokenType.TagName:
                    _tagName = token.Content;
                    break;

                case TokenType.EndTag:
                    break;

                case TokenType.Separator:
                case TokenType.TagParam:
                    _isInvalid = true;
                    break;

                case TokenType.TagClose:
                    _isCompleted = true;
                    break;

                default:
                    throw new InvalidOperationException(String.Format("Tag end node cannot have tokens of type {0}", token.TokenType));
            }

            _tokens.Add(token);
        }
    }
}