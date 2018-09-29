using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Jobs
{
    public struct JobResult
    {
        public readonly TimeSpan Delay;

        public readonly Exception Error;

        public readonly object Result;

        public readonly JobState State;

        public JobResult(JobState state, object result = null, TimeSpan? delay = null, Exception error = null)
        {
            State = state;
            Result = result;
            Delay = delay ?? TimeSpan.Zero;
            Error = error;
        }

        public static JobResult Aborted()
        {
            return new JobResult(JobState.Aborted);
        }

        public static JobResult Canceled()
        {
            return new JobResult(JobState.Canceled);
        }

        public static JobResult Exception(WebException error)
        {
            return new JobResult(JobState.Exception, error: error);
        }

        public static JobResult Finished(object result)
        {
            return new JobResult(JobState.Finished, result: result);
        }

        public static JobResult InProgress()
        {
            return new JobResult(JobState.InProgress);
        }

        public static JobResult InProgress(TimeSpan delay)
        {
            return new JobResult(JobState.InProgress, delay: delay);
        }

        public static JobResult NotStarted()
        {
            return new JobResult(JobState.NotStarted);
        }
    }
}
