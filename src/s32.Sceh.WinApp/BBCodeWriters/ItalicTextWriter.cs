using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.WinApp.BBCodeWriters
{
    public class ItalicTextWriter : TextFormatWriter
    {
        internal override string GetEndTag()
        {
            return "[/i]";
        }

        internal override string GetStartTag()
        {
            return "[i]";
        }
    }
}