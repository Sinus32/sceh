using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.UserNoteTags.Lexer;

namespace s32.Sceh.UserNoteTags.Parser
{
    public class TagStartNode : Node, ITagNode
    {
        private bool _isCompleted;
        private string _tagName;
        private string _tagParam;
        public ITagNode CloseTag { get; set; }

        public bool IsClosed
        {
            get { return CloseTag != null; }
        }

        public bool IsSelfClosed
        {
            get { return CloseTag == this; }
        }

        public bool IsValid
        {
            get { return _isCompleted && !String.IsNullOrEmpty(_tagName); }
        }

        public override NodeType NodeType
        {
            get { return NodeType.TagStart; }
        }

        public string TagName
        {
            get { return _tagName; }
        }

        public string TagParam
        {
            get { return _tagParam; }
        }

        public override void AddToken(Token token)
        {
            switch (token.TokenType)
            {
                case TokenType.TagName:
                    _tagName = token.Content;
                    break;

                case TokenType.TagParam:
                    _tagParam = token.Content;
                    break;

                case TokenType.BeginTag:
                case TokenType.Separator:
                    break;

                case TokenType.TagClose:
                    _isCompleted = true;
                    break;

                default:
                    throw new InvalidOperationException(String.Format("Tag start node cannot have tokens of type {0}", token.TokenType));
            }

            _tokens.Add(token);
        }
    }
}