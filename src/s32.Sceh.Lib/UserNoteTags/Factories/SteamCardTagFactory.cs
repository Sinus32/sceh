using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.BBCode;

namespace s32.Sceh.UserNoteTags.Factories
{
    public class SteamCardTagFactory : IUserNoteTagFactory
    {
        public IUserNoteTag CreateTag(IBBTagNode node)
        {
            if (!String.Equals(SteamCardTag.TagName, node.TagName, StringComparison.CurrentCultureIgnoreCase))
                return null;

            if (node.Content.Count > 0 && node.Content[0].NodeType == BBNodeType.Text)
                return new SteamCardTag(node.TagParam, node.Content[0].GetText());

            return null;
        }
    }
}
