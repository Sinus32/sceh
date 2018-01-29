using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.WinApp.BBCodeWriters
{
    public class UnderlineTextWriter : TextFormatWriter
    {
        internal override string GetEndTag()
        {
            return "[/u]";
        }

        internal override string GetStartTag()
        {
            return "[u]";
        }
    }
}
