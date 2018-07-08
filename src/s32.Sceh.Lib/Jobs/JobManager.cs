using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Jobs
{
    public class JobManager
    {
        public const int MAX_JOBS = 0x10000;
        private readonly StatefulJob[] _jobs;
        private readonly ReadOnlyCollection<StatefulJob> _jobsStorage;
        private long _currPriority;
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

        public StatefulJob PutJob<TJob>(string id, Func<string, TJob> jobBuilder)
            where TJob : StatefulJob
        {
            if (id == null)
                throw new ArgumentNullException("id");

            if (jobBuilder == null)
                throw new ArgumentNullException("jobBuilder");

            lock (this)
            {
                StatefulJob job;
                for (int i = 0; i < _jobCount; ++i)
                {
                    job = _jobs[i];
                    if (String.Equals(job.Id, id, StringComparison.Ordinal))
                    {
                        MoveTop(job);
                        IncreasePriority(job);
                        return job;
                    }
                }

                if (_jobCount >= _jobs.Length)
                    return null;

                job = jobBuilder(id);

                _jobs[_jobCount] = job;
                job.Position = ++_jobCount;

                MoveTop(job);
                IncreasePriority(job);

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

                var lastJob = _jobs[--_jobCount];
                _jobs[_jobCount] = null;

                if (_jobCount > 0 && jobIndex < _jobCount)
                {
                    _jobs[jobIndex] = lastJob;
                    lastJob.Position = jobIndex + 1;

                    MoveTop(lastJob);
                    BubbleDown(lastJob);
                }

                job.Dispose();
            }
        }

        private void BubbleDown(StatefulJob job)
        {
            var pos = job.Position;
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

                if (subJob.Priority <= job.Priority)
                    break;

                subJob.Position = pos;
                _jobs[pos - 1] = subJob;

                pos = subPos;
            }

            if (job.Position != pos)
            {
                job.Position = pos;
                _jobs[pos - 1] = job;
            }
        }

        private void IncreasePriority(StatefulJob job)
        {
            if (job.Priority != _currPriority)
                job.Priority = ++_currPriority;
        }

        private void MoveTop(StatefulJob job)
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
        }
    }
}
