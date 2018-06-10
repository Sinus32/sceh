using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.BBCode;

namespace s32.Sceh.UserNoteTags.Factories
{
    public interface IUserNoteTagFactory
    {
        IUserNoteTag CreateTag(IBBTagNode node);
    }
}