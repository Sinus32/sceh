using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using s32.Sceh.Code;
using s32.Sceh.Data;

namespace s32.Sceh.WinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class ScehWinApp : Application
    {
        private ImageDownloader.Worker _imageDownloaderWorker;
        private Thread _imageDownloaderWorkerThread;
        private DispatcherTimer _timer;

        private void _timer_Tick(object sender, EventArgs e)
        {
            ScehData.SaveFile();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= _timer_Tick;
                _timer = null;
            }

            if (_imageDownloaderWorker != null)
            {
                _imageDownloaderWorker.Terminate();
                if (_imageDownloaderWorkerThread != null)
                    if (!_imageDownloaderWorkerThread.Join(3000))
                        _imageDownloaderWorkerThread.Abort();
            }

            ScehData.SaveFile();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ScehData.Load();
            _imageDownloaderWorker = new ImageDownloader.Worker();
            _imageDownloaderWorkerThread = new Thread(_imageDownloaderWorker.ThreadStart);
            _imageDownloaderWorkerThread.Start();
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 1, 0);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }
    }
}