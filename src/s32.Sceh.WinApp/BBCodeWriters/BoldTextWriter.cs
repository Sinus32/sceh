using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.WinApp.BBCodeWriters
{
    public class BoldTextWriter : TextFormatWriter
    {
        internal override string GetEndTag()
        {
            return "[/b]";
        }

        internal override string GetStartTag()
        {
            return "[b]";
        }
    }
}