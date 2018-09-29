using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Jobs
{
    public class JobFinishedEventArgs : EventArgs
    {
        public JobFinishedEventArgs(object result)
        {
            Result = result;
            IsSuccess = true;
        }

        public JobFinishedEventArgs(Exception exception)
        {
            Exception = exception;
            IsSuccess = false;
        }

        public Exception Exception { get; private set; }
        public bool IsSuccess { get; private set; }
        public object Result { get; private set; }

        public static JobFinishedEventArgs Canceled()
        {
            throw new NotImplementedException();
        }
    }
}
