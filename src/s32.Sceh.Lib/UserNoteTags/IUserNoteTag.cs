﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.UserNoteTags
{
    public interface IUserNoteTag
    {
        string BuildSourceText();

        string Name { get; }

        string GetFormatedText();
    }
}