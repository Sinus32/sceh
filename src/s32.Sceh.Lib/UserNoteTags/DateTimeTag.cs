using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.UserNoteTags
{
    public class DateTimeTag : IUserNoteTag
    {
        public const string DateFormat = "yyyy'-'MM'-'dd";
        public const string DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";

        public DateTimeTag()
        { }

        public DateTimeTag(DateTime dateTime)
        {
            Value = dateTime;
        }

        public DateTime Value { get; set; }

        public string BuildTag()
        {
            string format = Value.TimeOfDay == TimeSpan.Zero ? DateFormat : DateTimeFormat;
            return String.Concat("[date]", Value.ToString(format), "[/date]");
        }

        public override string ToString()
        {
            return BuildTag();
        }
    }
}