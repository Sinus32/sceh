using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Jobs.Impl
{
    public enum ImageDownloadResult
    {
        Downloaded = 1,

        NotModified = 2,

        Failure = 3,
    }
}
