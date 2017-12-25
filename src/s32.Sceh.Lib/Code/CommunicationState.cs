using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Code
{
    public class CommunicationState : INotifyPropertyChanged
    {
        public static readonly CommunicationState Instance = new CommunicationState();

        private bool _isInProgress, _isRepeating;
        private int _requestCount, _imagesToDownload, _imagesDownloaded, _imagesNotModified;

        private CommunicationState()
        { }

        public event PropertyChangedEventHandler PropertyChanged;

        public int ImagesDownloaded
        {
            get { return _imagesDownloaded; }
            set
            {
                if (_imagesDownloaded != value)
                {
                    _imagesDownloaded = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int ImagesNotModified
        {
            get { return _imagesNotModified; }
            set
            {
                if (_imagesNotModified != value)
                {
                    _imagesNotModified = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int ImagesToDownload
        {
            get { return _imagesToDownload; }
            set
            {
                if (_imagesToDownload != value)
                {
                    _imagesToDownload = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsInProgress
        {
            get { return _isInProgress; }
            set
            {
                if (_isInProgress != value)
                {
                    _isInProgress = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsRepeating
        {
            get { return _isRepeating; }
            set
            {
                if (_isRepeating != value)
                {
                    _isRepeating = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int RequestCount
        {
            get { return _requestCount; }
            set
            {
                if (_requestCount != value)
                {
                    _requestCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}