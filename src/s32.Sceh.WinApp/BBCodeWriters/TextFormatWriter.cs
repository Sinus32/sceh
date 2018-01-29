using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using s32.Sceh.DataModel;

namespace s32.Sceh.WinApp.BBCodeWriters
{
    public abstract class TextFormatWriter : IBBCodeWriter
    {
        public bool CanWrite(List<SteamApp> steamApps)
        {
            return true;
        }

        public void Write(TextBox tbEditor, List<SteamApp> steamApps)
        {
            var startTag = GetStartTag();
            var endTag = GetEndTag();

            if (String.IsNullOrEmpty(tbEditor.SelectedText))
            {
                tbEditor.SelectedText = String.Concat(startTag, endTag);
                tbEditor.CaretIndex = tbEditor.SelectionStart + startTag.Length;
            }
            else
            {
                tbEditor.SelectedText = String.Concat(startTag, tbEditor.SelectedText, endTag);
                tbEditor.CaretIndex = tbEditor.SelectionStart + tbEditor.SelectionLength;
            }
        }

        internal abstract string GetEndTag();

        internal abstract string GetStartTag();
    }
}