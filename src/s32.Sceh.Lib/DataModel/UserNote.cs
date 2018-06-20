using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.DataModel
{
    public class UserNote : INotifyPropertyChanged
    {
        private WeakReference<UserNotes> _notes;
        private int _score;
        private string _text;

        public UserNote()
        {
        }

        public UserNote(int score, string text)
        {
            _score = score;
            _text = text;
        }

        internal UserNote(UserNotes notes)
        {
            _notes = new WeakReference<UserNotes>(notes);
        }

        internal UserNote(UserNotes notes, int score, string text)
        {
            _notes = new WeakReference<UserNotes>(notes);
            _score = score;
            _text = text;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsNegative
        {
            get { return _score < 0; }
        }

        public bool IsNeutral
        {
            get { return _score == 0; }
        }

        public bool IsPositive
        {
            get { return _score > 0; }
        }

        public UserNotes Notes
        {
            get
            {
                UserNotes notes;
                if (_notes != null && _notes.TryGetTarget(out notes))
                    return notes;
                return null;
            }
        }

        public int Score
        {
            get { return _score; }
            set
            {
                if (_score != value)
                {
                    var oldScore = _score;
                    _score = value;
                    NotifyPropertyChanged();
                    if (oldScore == 0 || value == 0)
                        NotifyPropertyChanged("IsNeutral");
                    if (oldScore > 0 != value > 0)
                    {
                        NotifyPropertyChanged("IsPositive");
                        NotifyPropertyChanged("IsNegative");
                    }
                    UserNotes notes;
                    if (_notes != null && _notes.TryGetTarget(out notes))
                        notes.ScoreChanged(oldScore > 0 || value > 0, oldScore < 0 || value < 0);
                }
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    NotifyPropertyChanged();
                    UserNotes notes;
                    if (_notes != null && _notes.TryGetTarget(out notes))
                        notes.TextChanged();
                }
            }
        }

        internal void SetParent(UserNotes notes)
        {
            if (notes == null)
            {
                _notes = null;
            }
            else
            {
                UserNotes old;
                if (_notes != null && _notes.TryGetTarget(out old))
                {
                    if (Object.ReferenceEquals(notes, old))
                        return;
                    throw new InvalidOperationException("This note is already attached to another UserNotes object");
                }
                _notes = new WeakReference<UserNotes>(notes);
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
