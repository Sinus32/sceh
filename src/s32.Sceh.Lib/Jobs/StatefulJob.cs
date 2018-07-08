using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Jobs
{
    public abstract class StatefulJob : IDisposable
    {
        private readonly string _id;
        private bool _disposedValue = false;
        private int _position = 0;
        private long _priority = -1;

        protected StatefulJob(string id)
        {
            _id = id;
        }

        public string Id
        {
            get { return _id; }
        }

        public DateTime NextRun { get; protected set; }

        /// <summary>
        /// Position in 1-indexed array
        /// </summary>
        public int Position
        {
            get { return _position; }
            internal set { _position = value; }
        }

        public long Priority
        {
            get { return _priority; }
            internal set { _priority = value; }
        }

        public JobState State { get; protected set; }

        public void Dispose()
        {
            Dispose(true);
        }

        public abstract void DoStep();

        public override string ToString()
        {
            return String.Format("{0:D4}:'{1}', p:{2}", Position, Id, Priority);
        }

        protected abstract void Cleanup();

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Cleanup();
                }

                _disposedValue = true;
            }
        }
    }
}
