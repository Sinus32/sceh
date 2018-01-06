using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.UserNoteTags.Lexer
{
    public class Token
    {
        public Token(TokenType tokenType, string content)
        {
            TokenType = tokenType;
            Content = content;
        }

        public string Content { get; set; }

        public TokenType TokenType { get; set; }
    }
}