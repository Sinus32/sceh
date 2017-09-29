using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using s32.Sceh.Data;

namespace s32.Sceh.WinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class ScehWinApp : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ScehData.SaveFile();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ScehData.Load();
        }
    }
}