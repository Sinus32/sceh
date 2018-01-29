using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.BBCode;

namespace s32.Sceh.UserNoteTags.Factories
{
    public class SteamAppTagFactory : IUserNoteTagFactory
    {
        public IUserNoteTag CreateTag(IBBTagNode node)
        {
            if (!String.Equals(SteamAppTag.TagName, node.TagName, StringComparison.CurrentCultureIgnoreCase))
                return null;

            if (node.Content.Count > 0 && node.Content[0].NodeType == BBNodeType.Text)
            {
                long appId;
                if (node.TagParam == null || !Int64.TryParse(node.TagParam, NumberStyles.Integer, CultureInfo.CurrentCulture, out appId))
                    appId = 0L;

                return new SteamAppTag(appId, node.Content[0].GetText());
            }

            return null;
        }
    }
}