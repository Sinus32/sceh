using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.BBCode
{
    public class BBTagEndNode : BBNode, IBBTagNode
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

        public override BBNodeType NodeType
        {
            get { return BBNodeType.TagEnd; }
        }

        public IBBTagNode OpenTag { get; internal set; }

        public string TagName
        {
            get { return _tagName; }
        }

        bool IBBTagNode.IsEndTag
        {
            get { return true; }
        }

        bool IBBTagNode.IsStartTag
        {
            get { return false; }
        }

        string IBBTagNode.TagParam
        {
            get { return null; }
        }

        public override void AddToken(BBToken token)
        {
            switch (token.TokenType)
            {
                case BBTokenType.EndTag:
                    break;

                case BBTokenType.TagName:
                    _tagName = token.Content;
                    break;

                case BBTokenType.Separator:
                case BBTokenType.TagParam:
                    _isInvalid = true;
                    break;

                case BBTokenType.TagClose:
                    if (_tagName != null)
                        _tagName = _tagName.Trim();
                    _isCompleted = true;
                    break;

                default:
                    throw new InvalidOperationException(String.Format("Tag end node cannot have tokens of type {0}", token.TokenType));
            }

            _tokens.Add(token);
        }
    }
}