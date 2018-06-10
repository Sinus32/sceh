using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using s32.Sceh.DataModel;

namespace s32.Sceh.WinApp.BBCodeWriters
{
    public class TextColorWriter : TextFormatWriter
    {
        internal override string GetEndTag()
        {
            return "[/c]";
        }

        internal override string GetStartTag()
        {
            return "[c]";
        }
    }
}