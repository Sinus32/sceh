using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.BBCode;

namespace s32.Sceh.UserNoteTags.Factories
{
    public class DateTimeTagFactory : IUserNoteTagFactory
    {
        public IUserNoteTag CreateTag(IBBTagNode node)
        {
            if (!String.Equals(DateTimeTag.TagName, node.TagName, StringComparison.CurrentCultureIgnoreCase))
                return null;

            if (node.Content.Count > 0 && node.Content[0].NodeType == BBNodeType.Text)
            {
                DateTime value;
                var text = node.Content[0].GetText();
                var formats = new string[] { DateTimeTag.DateTimeFormat, DateTimeTag.DateFormat };
                if (DateTime.TryParseExact(text, formats, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out value))
                    return new DateTimeTag(value);
            }

            return null;
        }
    }
}