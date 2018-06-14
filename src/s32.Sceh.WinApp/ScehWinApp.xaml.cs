using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Threading;
using s32.Sceh.Code;
using s32.Sceh.WinApp.Code;
using s32.Sceh.WinApp.Translations;

namespace s32.Sceh.WinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class ScehWinApp : Application
    {
        private DispatcherTimer _autoSaveTimer;
        private ImageDownloader.Worker[] _imageDownloaderWorker;

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (_autoSaveTimer != null)
            {
                _autoSaveTimer.Stop();
                _autoSaveTimer.Tick -= AutoSaveTimer_Tick;
                _autoSaveTimer = null;
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
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            SetCulture();

            DataManager.Initialize();
            _imageDownloaderWorker = new ImageDownloader.Worker[30];
            for (int i = 0; i < _imageDownloaderWorker.Length; ++i)
            {
                _imageDownloaderWorker[i] = new ImageDownloader.Worker(ImageLoadNotifier.FileIsReady);
                _imageDownloaderWorker[i].Start();
            }
            _autoSaveTimer = new DispatcherTimer();
            _autoSaveTimer.Interval = new TimeSpan(0, 1, 0);
            _autoSaveTimer.Tick += AutoSaveTimer_Tick;
            _autoSaveTimer.Start();

            ThreadPool.QueueUserWorkItem(LoadSceData);
            ThreadPool.QueueUserWorkItem(LoadStData);

            bool openLoginWindow = true;
            if (DataManager.AutoLogIn)
            {
                var lastProfile = DataManager.LastSteamProfile;
                if (lastProfile != null && lastProfile.SteamId > 0L)
                {
                    var cmpWindow = new InvCompareWindow();
                    cmpWindow.OwnerProfile = lastProfile;
                    cmpWindow.Show();
                    openLoginWindow = false;
                }
            }

            if (openLoginWindow)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
            }
        }

        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            DataManager.SaveFile();
        }

        private void LoadSceData(object state)
        {
            if (!DataManager.SceData.LoadSceData())
            {
                var msg = DataManager.SceData.ErrorMessage;
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(msg, Strings.SceErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private void LoadStData(object state)
        {
            if (!DataManager.StData.LoadStData())
            {
                var msg = DataManager.StData.ErrorMessage;
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(msg, Strings.StErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private void SetCulture()
        {
            var lang = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);
            FrameworkContentElement.LanguageProperty.OverrideMetadata(typeof(TextElement), new FrameworkPropertyMetadata(lang));
            FrameworkContentElement.LanguageProperty.OverrideMetadata(typeof(DefinitionBase), new FrameworkPropertyMetadata(lang));
            FrameworkContentElement.LanguageProperty.OverrideMetadata(typeof(FixedDocument), new FrameworkPropertyMetadata(lang));
            FrameworkContentElement.LanguageProperty.OverrideMetadata(typeof(FixedDocumentSequence), new FrameworkPropertyMetadata(lang));
            FrameworkContentElement.LanguageProperty.OverrideMetadata(typeof(FlowDocument), new FrameworkPropertyMetadata(lang));
            FrameworkContentElement.LanguageProperty.OverrideMetadata(typeof(TableColumn), new FrameworkPropertyMetadata(lang));
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(lang));
        }
    }
}
