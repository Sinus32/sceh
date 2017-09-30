using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using s32.Sceh.Code;
using s32.Sceh.Data;
using s32.Sceh.WinApp.Code;

namespace s32.Sceh.WinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class ScehWinApp : Application
    {
        private BackgroundWorker _imageDownloaderWorker;
        private ManualResetEvent _terminateEvent = new ManualResetEvent(false);
        private DispatcherTimer _timer;
        private ManualResetEvent _workDoneEvent = new ManualResetEvent(false);

        private void _imageDownloaderWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _workDoneEvent.Reset();
            var threadWait = 5000;

            while (!_terminateEvent.WaitOne(threadWait))
            {
                ImageFile image;
                string imagePath;
                if (ImageDownloader.DownloadNext(out image, out imagePath))
                {
                    ((BackgroundWorker)sender).ReportProgress(0, imagePath);
                    threadWait = 50;
                }
                else
                {
                    threadWait = 250;
                }
            }
            _workDoneEvent.Set();
        }

        private void _imageDownloaderWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var imagePath = (string)e.UserState;
            ImageLoadNotifier.FileIsReady(imagePath);
        }

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

            _terminateEvent.Set();

            if (_imageDownloaderWorker != null && _imageDownloaderWorker.IsBusy)
                _workDoneEvent.WaitOne(30000);

            ScehData.SaveFile();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ScehData.Load();
            _imageDownloaderWorker = new BackgroundWorker();
            _imageDownloaderWorker.WorkerReportsProgress = true;
            _imageDownloaderWorker.DoWork += _imageDownloaderWorker_DoWork;
            _imageDownloaderWorker.ProgressChanged += _imageDownloaderWorker_ProgressChanged;
            _imageDownloaderWorker.RunWorkerAsync();
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 1, 0);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }
    }
}