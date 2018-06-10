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
        public const string TagName = "date";

        public DateTimeTag()
        { }

        public DateTimeTag(DateTime dateTime)
        {
            Value = dateTime;
        }

        public string Name
        {
            get { return TagName; }
        }

        public DateTime Value { get; set; }

        public string BuildSourceText()
        {
            string format = Value.TimeOfDay == TimeSpan.Zero ? DateFormat : DateTimeFormat;
            return String.Concat("[date]", Value.ToString(format), "[/date]");
        }

        public string GetFormatedText()
        {
            return Value.ToString(Value.TimeOfDay == TimeSpan.Zero ? "d" : "g");
        }

        public override string ToString()
        {
            return BuildSourceText();
        }
    }
}
