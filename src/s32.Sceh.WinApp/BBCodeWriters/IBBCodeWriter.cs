using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using s32.Sceh.DataModel;

namespace s32.Sceh.WinApp.BBCodeWriters
{
    public interface IBBCodeWriter
    {
        bool CanWrite(List<SteamApp> steamApps);

        void Write(TextBox tbEditor, List<SteamApp> steamApps);
    }
}