using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.DataModel
{
    public class UserNotes : IList<UserNote>, INotifyPropertyChanged
    {
        private List<UserNote> _notes;
        private Uri _tradeUrl;

        public UserNotes()
        {
            _notes = new List<UserNote>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count
        {
            get { return _notes.Count; }
        }

        public bool HasNote
        {
            get { return _notes.Count > 0; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int NegativeScore
        {
            get
            {
                var result = 0;
                foreach (var note in _notes)
                    if (note.Score < 0)
                        result += note.Score;
                return result;
            }
        }

        public int PositiveScore
        {
            get
            {
                var result = 0;
                foreach (var note in _notes)
                    if (note.Score > 0)
                        result += note.Score;
                return result;
            }
        }

        public int TotalScore
        {
            get
            {
                var result = 0;
                foreach (var note in _notes)
                    result += note.Score;
                return result;
            }
        }

        public Uri TradeUrl
        {
            get { return _tradeUrl; }
            set
            {
                if (_tradeUrl != value)
                {
                    _tradeUrl = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public UserNote this[int index]
        {
            get
            {
                return _notes[index];
            }
            set
            {
                var oldItem = _notes[index];
                if (Object.ReferenceEquals(oldItem, value))
                    return;
                _notes[index] = value;
                value.SetParent(this);
                if (oldItem.Score != value.Score)
                    ScoreChanged(oldItem.Score > 0 || value.Score > 0, oldItem.Score < 0 || value.Score < 0);
                if (_notes.IndexOf(oldItem) == -1)
                    oldItem.SetParent(null);
            }
        }

        public void Add(UserNote item)
        {
            _notes.Add(item);
            item.SetParent(this);
            NotifyPropertyChanged("Count");
            if (_notes.Count == 1)
                NotifyPropertyChanged("HasNote");
            if (item.Score != 0)
                ScoreChanged(item.Score > 0, item.Score < 0);
        }

        public void Add(int score, string text)
        {
            var item = new UserNote(this, score, text);
            _notes.Add(item);
            NotifyPropertyChanged("Count");
            if (_notes.Count == 1)
                NotifyPropertyChanged("HasNote");
            if (item.Score != 0)
                ScoreChanged(item.Score > 0, item.Score < 0);
        }

        public void Clear()
        {
            if (_notes.Count == 0)
                return;
            bool positive = false, negative = false;
            foreach (var item in _notes)
            {
                if (item.Score > 0)
                    positive = true;
                if (item.Score < 0)
                    negative = true;
                item.SetParent(null);
            }
            _notes.Clear();
            NotifyPropertyChanged("Count");
            NotifyPropertyChanged("HasNote");
            if (positive || negative)
                ScoreChanged(positive, negative);
        }

        public bool Contains(UserNote item)
        {
            return _notes.Contains(item);
        }

        public void CopyTo(UserNote[] array, int arrayIndex)
        {
            _notes.CopyTo(array, arrayIndex);
        }

        public IEnumerator<UserNote> GetEnumerator()
        {
            return _notes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _notes.GetEnumerator();
        }

        public int IndexOf(UserNote item)
        {
            return _notes.IndexOf(item);
        }

        public void Insert(int index, UserNote item)
        {
            _notes.Insert(index, item);
            item.SetParent(this);
            NotifyPropertyChanged("Count");
            if (_notes.Count == 1)
                NotifyPropertyChanged("HasNote");
            if (item.Score != 0)
                ScoreChanged(item.Score > 0, item.Score < 0);
        }

        public bool Remove(UserNote item)
        {
            var result = _notes.Remove(item);
            if (result)
            {
                NotifyPropertyChanged("Count");
                if (_notes.Count == 0)
                {
                    NotifyPropertyChanged("HasNote");
                    item.SetParent(null);
                }
                else if (_notes.IndexOf(item) == -1)
                {
                    item.SetParent(null);
                }
                if (item.Score != 0)
                    ScoreChanged(item.Score > 0, item.Score < 0);
            }
            return result;
        }

        public void RemoveAt(int index)
        {
            var item = _notes[index];
            _notes.RemoveAt(index);
            NotifyPropertyChanged("Count");
            if (_notes.Count == 0)
            {
                NotifyPropertyChanged("HasNote");
                item.SetParent(null);
            }
            else if (_notes.IndexOf(item) == -1)
            {
                item.SetParent(null);
            }
            if (item.Score != 0)
                ScoreChanged(item.Score > 0, item.Score < 0);
        }

        internal void ScoreChanged(bool positive, bool negative)
        {
            NotifyPropertyChanged("TotalScore");
            if (positive)
                NotifyPropertyChanged("PositiveScore");
            if (negative)
                NotifyPropertyChanged("NegativeScore");
        }

        internal void TextChanged()
        {
            NotifyPropertyChanged("AllTexts");
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}