using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Jobs
{
    public class JobManager
    {
        private readonly StatefulJob[] _jobs;
        private int _jobCount;
        private long _currPriority;

        public JobManager(int maxJobs)
        {
            _jobs = new StatefulJob[maxJobs];
        }

        public StatefulJob PutJob<TJob>(string id, Func<string, TJob> jobBuilder)
            where TJob : StatefulJob
        {
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
            lock (this)
            {
                if (!Object.ReferenceEquals(job, _jobs[job.Position - 1]))
                    throw new InvalidOperationException("The job does not belong to this job manager");

                MoveTop(job);

                var lastJob = _jobs[--_jobCount];
                _jobs[_jobCount] = null;

                BubbleDown(lastJob);
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
