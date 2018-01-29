using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using s32.Sceh.DataModel;
using s32.Sceh.UserNoteTags;

namespace s32.Sceh.WinApp.BBCodeWriters
{
    public class DateWriter : IBBCodeWriter
    {
        public virtual bool CanWrite(List<SteamApp> steamApps)
        {
            return true;
        }

        public void Write(TextBox tbEditor, List<SteamApp> steamApps)
        {
            var text = new DateTimeTag(DateTime.Today).BuildSourceText();
            tbEditor.SelectedText = text + " ";
            tbEditor.CaretIndex = tbEditor.SelectionStart + tbEditor.SelectionLength;
        }
    }
}