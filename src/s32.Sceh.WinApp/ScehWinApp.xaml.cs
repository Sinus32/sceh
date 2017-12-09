﻿using System;
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
using s32.Sceh.WinApp.Code;

namespace s32.Sceh.WinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class ScehWinApp : Application
    {
        private ImageDownloader.Worker[] _imageDownloaderWorker;
        private DispatcherTimer _timer;

        private void _timer_Tick(object sender, EventArgs e)
        {
            DataManager.SaveFile();
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
                foreach (var wrk in _imageDownloaderWorker)
                    if (wrk != null)
                        wrk.Stop();

                foreach (var wrk in _imageDownloaderWorker)
                    if (wrk != null)
                        wrk.StopAndJoin(30000);
            }

            DataManager.SaveFile();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DataManager.Initialize();
            _imageDownloaderWorker = new ImageDownloader.Worker[3];
            _imageDownloaderWorker[0] = new ImageDownloader.Worker(ImageLoadNotifier.FileIsReady);
            _imageDownloaderWorker[0].Start();
            _imageDownloaderWorker[1] = new ImageDownloader.Worker(ImageLoadNotifier.FileIsReady);
            _imageDownloaderWorker[1].Start();
            _imageDownloaderWorker[2] = new ImageDownloader.Worker(ImageLoadNotifier.FileIsReady);
            _imageDownloaderWorker[2].Start();
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 1, 0);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }
    }
}