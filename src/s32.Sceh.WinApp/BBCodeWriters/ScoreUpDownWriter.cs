using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using s32.Sceh.DataModel;

namespace s32.Sceh.WinApp.BBCodeWriters
{
    public class ScoreUpDownWriter : IBBCodeWriter
    {
        public int Score { get; set; }

        public bool CanWrite(List<SteamApp> steamApps)
        {
            return false;
        }

        public void Write(TextBox tbEditor, List<SteamApp> steamApps)
        {
            throw new NotImplementedException();
        }
    }
}