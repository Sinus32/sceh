using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using s32.Sceh.DataModel;
using s32.Sceh.WinApp.Code;

namespace s32.Sceh.WinApp.BBCodeWriters
{
    public class TextColorWriter : TextFormatWriter
    {
        public TextColorWriter()
        { }

        public TextColorWriter(BrushName brushName)
        {
            BrushName = brushName;
        }

        public BrushName BrushName { get; set; }

        internal override string GetEndTag()
        {
            return "[/c]";
        }

        internal override string GetStartTag()
        {
            return String.Concat("[c=", BrushName.Name, "]");
        }
    }
}
