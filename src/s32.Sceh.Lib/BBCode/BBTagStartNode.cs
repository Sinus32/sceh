using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.BBCode
{
    public class BBTagStartNode : BBNode, IBBTagNode
    {
        private bool _isCompleted;
        private string _tagName;
        private string _tagParam;

        public IBBTagNode CloseTag { get; internal set; }

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

        public override BBNodeType NodeType
        {
            get { return BBNodeType.TagStart; }
        }

        public string TagName
        {
            get { return _tagName; }
        }

        public string TagParam
        {
            get { return _tagParam; }
        }

        bool IBBTagNode.IsEndTag
        {
            get { return false; }
        }

        bool IBBTagNode.IsStartTag
        {
            get { return true; }
        }

        public override void AddToken(BBToken token)
        {
            switch (token.TokenType)
            {
                case BBTokenType.BeginTag:
                case BBTokenType.Separator:
                    break;

                case BBTokenType.TagName:
                    _tagName = token.Content;
                    break;

                case BBTokenType.TagParam:
                    _tagParam = token.Content;
                    break;

                case BBTokenType.TagClose:
                    if (_tagName != null)
                        _tagName = _tagName.Trim();
                    if (_tagParam != null)
                        _tagParam = _tagParam.Trim();
                    _isCompleted = true;
                    break;

                default:
                    throw new InvalidOperationException(String.Format("Tag start node cannot have tokens of type {0}", token.TokenType));
            }

            _tokens.Add(token);
        }
    }
}