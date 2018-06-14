using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.BBCode
{
    public enum BBNodeType
    {
        Unknown,
        Root,
        Text,
        TagStart,
        TagEnd,
    }
}