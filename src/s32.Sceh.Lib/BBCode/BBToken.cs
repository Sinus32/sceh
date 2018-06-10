using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.BBCode
{
    public class BBToken
    {
        public BBToken(BBTokenType tokenType, string content)
        {
            TokenType = tokenType;
            Content = content;
        }

        public string Content { get; set; }

        public BBTokenType TokenType { get; set; }

        public override string ToString()
        {
            char c;
            switch (TokenType)
            {
                case BBTokenType.Text:
                    c = 't';
                    break;

                case BBTokenType.BeginTag:
                    c = 'b';
                    break;

                case BBTokenType.EndTag:
                    c = 'e';
                    break;

                case BBTokenType.TagClose:
                    c = 'c';
                    break;

                case BBTokenType.TagName:
                    c = 'n';
                    break;

                case BBTokenType.Separator:
                    c = 's';
                    break;

                case BBTokenType.TagParam:
                    c = 'p';
                    break;

                default:
                    c = 'u';
                    break;
            }
            return String.Concat(c, '{', Content, '}');
        }
    }
}