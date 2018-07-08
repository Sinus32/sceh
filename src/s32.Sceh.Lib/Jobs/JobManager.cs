using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Jobs
{
    public class JobManager : IDisposable
    {
        public const int MAX_JOBS = 0x10000;
        private readonly StatefulJob[] _jobs;
        private readonly ReadOnlyCollection<StatefulJob> _jobsStorage;
        private long _currPriority;
        private bool _disposedValue = false;
        private int _jobCount;

        public JobManager(int maxJobs)
        {
            if (maxJobs < 1)
                throw new ArgumentOutOfRangeException("maxJobs", "Maximum number of jobs is too low (should be at least 1)");
            if (maxJobs > MAX_JOBS)
                throw new ArgumentOutOfRangeException("maxJobs", "Maximum number of jobs is too high (should be at most " + MAX_JOBS + ")");
            _jobs = new StatefulJob[maxJobs];
            _jobsStorage = new ReadOnlyCollection<StatefulJob>(_jobs);
        }

        public int JobCount
        {
            get { return _jobCount; }
        }

        public ReadOnlyCollection<StatefulJob> JobsStorage
        {
            get { return _jobsStorage; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void ProcessJobs(int maxJobsToStart)
        {
            for (int i = 0; i < _jobCount; ++i)
            {
                var job = _jobs[i];

                if (job.State == JobState.InProgress || job.State == JobState.NotStarted && i < maxJobsToStart)
                    DoSafeStep(job);

                if (job.State == JobState.Finished)
                    RemoveJob(job);
            }
        }

        private void DoSafeStep(StatefulJob job)
        {
            throw new NotImplementedException();
        }

        public StatefulJob FindJobById(string id)
        {
            StatefulJob job;
            for (int i = 0; i < _jobCount; ++i)
            {
                job = _jobs[i];
                if (String.Equals(job.Id, id, StringComparison.Ordinal))
                    return job;
            }

            return null;
        }

        public StatefulJob PutJob<TJob>(string id, Func<string, TJob> jobBuilder)
            where TJob : StatefulJob
        {
            if (id == null)
                throw new ArgumentNullException("id");

            if (jobBuilder == null)
                throw new ArgumentNullException("jobBuilder");

            lock (this)
            {
                var job = FindJobById(id);

                if (job == null)
                {
                    if (_jobCount >= _jobs.Length)
                        return null;

                    job = jobBuilder(id);

                    _jobs[_jobCount] = job;
                    job.Position = ++_jobCount;
                }

                MoveJobToTop(job);

                return job;
            }
        }

        public void RemoveJob(StatefulJob job)
        {
            if (job == null)
                throw new ArgumentNullException("job");

            lock (this)
            {
                var jobIndex = job.Position - 1;
                if (jobIndex < 0 || jobIndex >= _jobCount || !Object.ReferenceEquals(job, _jobs[jobIndex]))
                    throw new ArgumentException("The job does not belong to this job manager", "job");

                RemoveJobAtIndex(jobIndex);

                job.Position = 0;
                job.Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    lock (this)
                    {
                        StatefulJob job;
                        for (int i = 0; i < _jobs.Length; ++i)
                        {
                            job = _jobs[i];
                            if (job != null)
                            {
                                job.Dispose();
                                _jobs[i] = null;
                            }
                        }
                        _jobCount = 0;
                        _disposedValue = true;
                    }
                }

                _disposedValue = true;
            }
        }

        private void MoveJobToTop(StatefulJob job)
        {
            int pos = job.Position;
            while (pos > 1)
            {
                var parentPos = pos / 2;
                var parent = _jobs[parentPos - 1];

                parent.Position = pos;
                _jobs[pos - 1] = parent;

                pos = parentPos;
            }

            job.Position = 1;
            _jobs[0] = job;

            if (job.Priority != _currPriority)
                job.Priority = ++_currPriority;
        }

        private void RemoveJobAtIndex(int jobIndex)
        {
            var lastJob = _jobs[--_jobCount];
            _jobs[_jobCount] = null;

            if (_jobCount > 0 && jobIndex < _jobCount)
            {
                _jobs[jobIndex] = lastJob;
                int pos = jobIndex + 1;

                var doBubbleDown = true;
                var parentPos = pos >> 1;
                while (parentPos >= 1)
                {
                    var parentIndex = parentPos - 1;
                    var parent = _jobs[parentIndex];

                    if (parent.Priority >= lastJob.Priority)
                        break;

                    parent.Position = pos;
                    _jobs[jobIndex] = parent;
                    doBubbleDown = false;

                    pos = parentPos;
                    jobIndex = pos - 1;
                    parentPos >>= 1;
                }

                if (doBubbleDown)
                {
                    while (true)
                    {
                        var subPos = pos * 2;
                        if (subPos > _jobCount)
                            break;

                        var subJob = _jobs[subPos - 1];

                        var rightPos = subPos + 1;
                        if (rightPos <= _jobCount)
                        {
                            var rightJob = _jobs[rightPos - 1];
                            if (rightJob.Priority > subJob.Priority)
                            {
                                subJob = rightJob;
                                subPos = rightPos;
                            }
                        }

                        if (subJob.Priority <= lastJob.Priority)
                            break;

                        subJob.Position = pos;
                        _jobs[pos - 1] = subJob;

                        pos = subPos;
                    }
                }

                if (lastJob.Position != pos)
                {
                    lastJob.Position = pos;
                    _jobs[pos - 1] = lastJob;
                }
            }
        }
    }
}
