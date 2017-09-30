using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace s32.Sceh.WinApp
{
    internal static class Program
    {
        private static bool _appLoaded = false;

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (_appLoaded)
            {
            }
            else
            {
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (_appLoaded)
            {
            }
            else
            {
            }
        }

        private static void form_Load(object sender, EventArgs e)
        {
            _appLoaded = true;
        }

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var form = new IntroForm();
            form.Load += form_Load;
            Application.Run(form);
        }
    }
}