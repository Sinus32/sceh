using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.BBCode
{
    public enum BBTokenType
    {
        Unknown,
        Text,
        BeginTag,
        EndTag,
        TagClose,
        TagName,
        Separator,
        TagParam,
    }
}