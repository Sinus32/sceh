﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.BBCode
{
    public interface IBBTagNode : IBBNode
    {
        bool IsEndTag { get; }

        bool IsSelfClosed { get; }

        bool IsStartTag { get; }

        bool IsValid { get; }

        IBBTagNode SecondTag { get; }

        string TagName { get; }

        string TagParam { get; }
    }
}