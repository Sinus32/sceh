using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.BBCode
{
    public class BBCodeLexer
    {
        private const int TEXT = 0, TAG_START = 1, TAG_END = 2, TAG_NAME = 3, TAG_SEPARATOR = 4, TAG_PARAM = 5, TAG_CLOSE = 6;
        private List<BBToken> _result;
        private StringBuilder _sb = new StringBuilder();
        private int _state;

        public List<BBToken> Result
        {
            get { return _result; }
        }

        public List<BBToken> Finish()
        {
            switch (_state)
            {
                case TEXT: FinishToken(BBTokenType.Text); break;
                case TAG_START: FinishToken(BBTokenType.BeginTag); break;
                case TAG_END: FinishToken(BBTokenType.EndTag); break;
                case TAG_NAME: FinishToken(BBTokenType.TagName); break;
                case TAG_SEPARATOR: FinishToken(BBTokenType.Separator); break;
                case TAG_PARAM: FinishToken(BBTokenType.TagParam); break;
                case TAG_CLOSE: FinishToken(BBTokenType.TagClose); break;
            }
            return _result;
        }

        public BBCodeLexer Parse(IEnumerable<char> characters)
        {
            foreach (char c in characters)
                ParseChar(c);
            return this;
        }

        public BBCodeLexer Parse(char c)
        {
            ParseChar(c);
            return this;
        }

        public List<BBToken> ParseString(string text)
        {
            return Reset().Parse(text).Finish();
        }

        public BBCodeLexer Reset()
        {
            _sb.Clear();
            _result = new List<BBToken>();
            _state = TEXT;
            return this;
        }

        private void FinishToken(BBTokenType tokenType)
        {
            if (_sb.Length > 0)
            {
                _result.Add(new BBToken(tokenType, _sb.ToString()));
                _sb.Clear();
            }
        }

        private void ParseChar(char c)
        {
            switch (_state)
            {
                case TEXT:
                    if (c == '[')
                    {
                        FinishToken(BBTokenType.Text);
                        _state = TAG_START;
                    }
                    break;

                case TAG_START:
                    if (c == '/')
                    {
                        _state = TAG_END;
                    }
                    else
                    {
                        FinishToken(BBTokenType.BeginTag);
                        switch (c)
                        {
                            case '[': break;
                            case '=': _state = TAG_SEPARATOR; break;
                            case ']': _state = TAG_CLOSE; break;
                            default: _state = TAG_NAME; break;
                        }
                    }
                    break;

                case TAG_END:
                    FinishToken(BBTokenType.EndTag);
                    switch (c)
                    {
                        case '[': _state = TAG_START; break;
                        case '=': _state = TAG_SEPARATOR; break;
                        case ']': _state = TAG_CLOSE; break;
                        default: _state = TAG_NAME; break;
                    }
                    break;

                case TAG_NAME:
                    switch (c)
                    {
                        case '[': FinishToken(BBTokenType.TagName); _state = TAG_START; break;
                        case '=': FinishToken(BBTokenType.TagName); _state = TAG_SEPARATOR; break;
                        case ']': FinishToken(BBTokenType.TagName); _state = TAG_CLOSE; break;
                    }
                    break;

                case TAG_SEPARATOR:
                    FinishToken(BBTokenType.Separator);
                    switch (c)
                    {
                        case '[': _state = TAG_START; break;
                        case ']': _state = TAG_CLOSE; break;
                        default: _state = TAG_PARAM; break;
                    }
                    break;

                case TAG_PARAM:
                    switch (c)
                    {
                        case '[': FinishToken(BBTokenType.TagParam); _state = TAG_START; break;
                        case ']': FinishToken(BBTokenType.TagParam); _state = TAG_CLOSE; break;
                    }
                    break;

                case TAG_CLOSE:
                    FinishToken(BBTokenType.TagClose);
                    _state = c == '[' ? TAG_START : TEXT;
                    break;
            }

            _sb.Append(c);
        }
    }
}