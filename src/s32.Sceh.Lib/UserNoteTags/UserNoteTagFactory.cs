using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.BBCode;

namespace s32.Sceh.UserNoteTags
{
    public class UserNoteTagFactory
    {
        public static readonly UserNoteTagFactory Instance = new UserNoteTagFactory();

        public IUserNoteTag Create(IBBTagNode node)
        {
            throw new NotImplementedException();
        }
    }
}
