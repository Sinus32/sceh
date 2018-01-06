﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.UserNoteTags.Lexer
{
    public class NoteLexer
    {
        private const int TEXT = 0, TAG_START = 1, TAG_END = 2, TAG_NAME = 3, TAG_SEPARATOR = 4, TAG_PARAM = 5, TAG_CLOSE = 6;
        private List<Token> _result;
        private StringBuilder _sb = new StringBuilder();
        private int _state;

        public List<Token> Result
        {
            get { return _result; }
        }

        public List<Token> Finish()
        {
            switch (_state)
            {
                case TEXT: FinishToken(TokenType.Text); break;
                case TAG_START: FinishToken(TokenType.BeginTag); break;
                case TAG_END: FinishToken(TokenType.EndTag); break;
                case TAG_NAME: FinishToken(TokenType.TagName); break;
                case TAG_SEPARATOR: FinishToken(TokenType.Separator); break;
                case TAG_PARAM: FinishToken(TokenType.TagParam); break;
                case TAG_CLOSE: FinishToken(TokenType.TagClose); break;
            }
            return _result;
        }

        public NoteLexer Parse(IEnumerable<char> characters)
        {
            foreach (char c in characters)
                ParseChar(c);
            return this;
        }

        public NoteLexer Parse(char c)
        {
            ParseChar(c);
            return this;
        }

        public List<Token> ParseString(string text)
        {
            return Reset().Parse(text).Finish();
        }

        public NoteLexer Reset()
        {
            _sb.Clear();
            _result = new List<Token>();
            _state = TEXT;
            return this;
        }

        private void FinishToken(TokenType tokenType)
        {
            if (_sb.Length > 0)
            {
                _result.Add(new Token(tokenType, _sb.ToString()));
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
                        FinishToken(TokenType.Text);
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
                        FinishToken(TokenType.BeginTag);
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
                    FinishToken(TokenType.EndTag);
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
                        case '[': FinishToken(TokenType.TagName); _state = TAG_START; break;
                        case '=': FinishToken(TokenType.TagName); _state = TAG_SEPARATOR; break;
                        case ']': FinishToken(TokenType.TagName); _state = TAG_CLOSE; break;
                    }
                    break;

                case TAG_SEPARATOR:
                    FinishToken(TokenType.Separator);
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
                        case '[': FinishToken(TokenType.TagParam); _state = TAG_START; break;
                        case ']': FinishToken(TokenType.TagParam); _state = TAG_CLOSE; break;
                    }
                    break;

                case TAG_CLOSE:
                    FinishToken(TokenType.TagClose);
                    _state = c == '[' ? TAG_START : TEXT;
                    break;
            }

            _sb.Append(c);
        }
    }
}