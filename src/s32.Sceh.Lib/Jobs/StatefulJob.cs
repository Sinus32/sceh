using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace s32.Sceh.Jobs
{
    public abstract class StatefulJob : IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly string _id;
        private bool _disposedValue = false;
        private Exception _exception;
        private DateTime _nextRun;
        private int _position = 0;
        private long _priority = -1;
        private object _result;
        private JobState _state = JobState.NotStarted;

        protected StatefulJob(string id)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentException("Job id cannot be empty", "id");

            _cts = new CancellationTokenSource();
            _id = id;
        }

        public string Id
        {
            get { return _id; }
        }

        public DateTime NextRun { get; private set; }

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

        public JobState State { get; private set; }

        //public event JobFinishedEventHandler JobFinished;

        //protected void OnJobFinished(JobFinishedEventArgs e)
        //{
        //    if (JobFinished != null)
        //        JobFinished(this, e);
        //}

        protected CancellationToken CancellationToken
        {
            get { return _cts.Token; }
        }

        protected bool IsCancellationRequested
        {
            get { return _cts.IsCancellationRequested; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool Process()
        {
            if (_disposedValue)
                throw new ObjectDisposedException("StatefulJob", "The job is already disposed");

            if (_state != JobState.NotStarted && _state != JobState.InProgress)
                throw new InvalidOperationException("Job already finished");

            if (_cts.IsCancellationRequested)
            {
                _state = JobState.Canceled;
                return true;
            }

            JobResult result;
            try
            {
                result = DoWork();
            }
            catch (Exception ex)
            {
                _exception = ex;
                _state = JobState.Exception;
                return true;
            }

            switch (result.State)
            {
                case JobState.NotStarted:
                    _state = JobState.NotStarted;
                    break;

                case JobState.InProgress:
                    _state = JobState.InProgress;
                    if (result.Delay > TimeSpan.Zero)
                        _nextRun = DateTime.Now + result.Delay;
                    break;

                case JobState.Canceled:
                    _state = JobState.Canceled;
                    _cts.Cancel();
                    return true;

                case JobState.Finished:
                    _state = JobState.Finished;
                    _result = result.Result;
                    return true;

                case JobState.Exception:
                    _state = JobState.Exception;
                    _exception = result.Error;
                    return true;

                case JobState.Aborted:
                    _state = JobState.Aborted;
                    return true;
            }

            if (_cts.IsCancellationRequested)
            {
                _state = JobState.Canceled;
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return String.Format("{0:D4}:'{1}', p:{2}", Position, Id, Priority);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    switch (_state)
                    {
                        case JobState.NotStarted:
                            _state = JobState.Aborted;
                            break;

                        case JobState.InProgress:
                            try
                            {
                                _cts.Cancel();
                            }
                            catch (Exception) { }
                            _state = JobState.Aborted;
                            break;
                    }

                    _cts.Dispose();

                    _disposedValue = true;
                }
            }
        }

        protected abstract JobResult DoWork();
    }
}
