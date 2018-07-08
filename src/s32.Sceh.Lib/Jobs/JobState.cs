﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Jobs
{
    public enum JobState
    {
        NotStarted,

        InProgress,

        Canceled,

        Finished,

        Exception,

        Aborted,
    }
}
